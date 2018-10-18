using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public class TagList : ITag, IEnumerable<ITag>
    {
        internal const byte TypeId = 9;
        private readonly List<ITag> Content = new List<ITag>();

        public ITag this[int index] => Content[index];

        public bool Inline { get; set; } = false;
        public int Length => Content.Count;

        public void Add(byte value) => Add(new TagByte(value));
        public void Add(short value) => Add(new TagShort(value));
        public void Add(int value) => Add(new TagInt(value));
        public void Add(double value) => Add(new TagDouble(value));
        public void Add(long value) => Add(new TagLong(value));
        public void Add(string value) => Add(new TagString(value));
        public void Add(bool value) => Add(new TagBoolean(value));
        public void Add(TagCompound value) => Add((object)value);
        public void Add(TagList value) => Add((object)value);
        public void Add(object value)
        {
            if (value is ITag) Content.Add((ITag)value);
            else
            {
                switch (value)
                {
                    case byte i: Content.Add(new TagByte(i)); break;
                    case short i: Content.Add(new TagShort(i)); break;
                    case int i: Content.Add(new TagInt(i)); break;
                    case float i: Content.Add(new TagFloat(i)); break;
                    case double i: Content.Add(new TagDouble(i)); break;
                    case long i: Content.Add(new TagLong(i)); break;
                    case string i: Content.Add(new TagString(i)); break;
                    case bool i: Content.Add(new TagBoolean(i)); break;
                    default: Content.Add(new TagCustom(value)); break;
                }
            }
        }

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            int size = 1;
            foreach (ITag value in Content)
            {
                writer.Write(true);
                size++;
                size += value.Write(json, writer);
            }
            writer.Write(false);
            size++;
            return size;
        }
        
        public void Resolve(TagMaster json)
        {
            for(int i = 0; i < Content.Count; i++)
            {
                ITag tag = Content[i];
                if(tag is TagCustom)
                {
                    Content[i] = tag = json.ConvertToValue(((TagCustom)tag).Value);
                }
                if (tag is TagCompound)
                {
                    ((TagCompound)tag).Resolve(json);
                }
                if (tag is TagList)
                {
                    ((TagList)tag).Resolve(json);
                }
            }
        }

        internal static ITag Parse(BinaryReader reader)
        {
            TagList arr = new TagList();
            while (reader.ReadBoolean())
            {
                ITag value = TagMaster.ParseValue(reader);
                arr.Add(value);
            }
            return arr;
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }

        public IEnumerator<ITag> GetEnumerator() => ((IEnumerable<ITag>)Content).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ITag>)Content).GetEnumerator();
    }
}
