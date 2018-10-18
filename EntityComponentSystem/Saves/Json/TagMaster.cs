using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves.Json.Converter;
using EntityComponentSystem.Saves.Json.Objects;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Saves.Json
{
    public class TagMaster
    {
        private readonly Dictionary<Type, ITagConverter> Converters = new Dictionary<Type, ITagConverter>();
        private readonly ITagConverter PersistentObjectConverter = new PersistentObjectConverter();

        public int Write(TagCompound root, BinaryWriter writer)
        {
            return root.Write(this, writer);
        }

        public TagCompound Read(BinaryReader reader)
        {
            if (TagCompound.IsValidStart(reader.ReadByte())) return TagCompound.Parse(reader) as TagCompound;
            else return null;
        }

        public ITag ConvertToValue(object value)
        {
            if(value.GetType().GetCustomAttributes(typeof(PersistentObjectAttribute), true).Length > 0)
            {
                return PersistentObjectConverter.ToJson(this, value);
            }
            foreach(KeyValuePair<Type, ITagConverter> converterPair in Converters)
            {
                if(converterPair.Key.IsAssignableFrom(value.GetType()))
                {
                    ITagConverter converter = converterPair.Value;
                    ITag converted = converter.ToJson(this, value);
                    if (converted is TagCustom)
                    {
                        throw new InvalidOperationException("Converter for type " + converterPair.Key.FullName + " does not convert to a valid value");
                    }
                    return converted;
                }
            }
            throw new ArgumentException("No converter available for such type: " + value.GetType().FullName);
        }

        public T ConvertFromValue<T>(ITag property)
        {
            if (typeof(T).GetCustomAttributes(typeof(PersistentObjectAttribute), true).Length > 0)
            {
                return PersistentObjectConverter.FromJson<T>(this, property);
            }
            foreach(KeyValuePair<Type, ITagConverter> converterPair in Converters)
            {
                if (converterPair.Key.IsAssignableFrom(typeof(T)))
                {
                    ITagConverter converter = converterPair.Value;
                    T converted = converter.FromJson<T>(this, property);
                    if (converted is TagCustom)
                    {
                        throw new InvalidOperationException("Converter for type " + converterPair.Key.FullName + " does not convert to a valid value");
                    }
                    return converted;
                }
            }
            throw new ArgumentException("No converter available for such type: " + typeof(T).FullName);
        }
        public object ConvertFromValue(ITag value, Type expectedType)
        {
            if (expectedType.IsValueType || expectedType == typeof(string) || expectedType.GetConstructor(new Type[0]) != null)
            {
                MethodInfo method = typeof(TagMaster).GetMethod("ConvertFromValue", new Type[] { typeof(ITag) });
                MethodInfo genericMethod = method.MakeGenericMethod(expectedType);
                return genericMethod.Invoke(this, new object[] { value });
            }
            else throw new Exception("Type '" + expectedType + " doesn't have a public parameterless constructor");
        }

        public void RegisterConverter(ITagConverter converter) => Converters[converter.GetWorkingType()] = converter;

        internal static ITag ParseValue(BinaryReader reader)
        {
            return ParseValue(reader, reader.ReadByte());
        } 

        internal static ITag ParseValue(BinaryReader reader, byte id)
        {
            switch(id)
            {
                case TagBoolean.TypeId: return TagBoolean.Parse(reader);
                case TagByte.TypeId: return TagByte.Parse(reader);
                case TagShort.TypeId: return TagShort.Parse(reader);
                case TagInt.TypeId: return TagInt.Parse(reader);
                case TagFloat.TypeId: return TagFloat.Parse(reader);
                case TagLong.TypeId: return TagLong.Parse(reader);
                case TagDouble.TypeId: return TagDouble.Parse(reader);
                case TagString.TypeId: return TagString.Parse(reader);
                case TagCompound.TypeId: return TagCompound.Parse(reader);
                case TagList.TypeId: return TagList.Parse(reader);
                default: throw new FormatException("Invalid type ID: " + id + ". At file position " + reader.BaseStream.Position);
            }
        }

        internal static int WriteString(string str, BinaryWriter writer)
        {
            writer.Write(str.Length);
            writer.Write(str.ToCharArray());
            return 2 + str.Length * 2;
        }

        internal static string ReadString(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            return new string(reader.ReadChars(length));
        }
    }
}
