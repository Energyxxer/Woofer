using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Converter;
using EntityComponentSystem.Saves.Json.Objects;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Saves
{
    class PersistentObjectConverter : ITagConverter
    {
        public Type GetWorkingType() => typeof(object);
        public T FromJson<T>(TagMaster json, ITag value)
        {
            TagCompound obj = (TagCompound)value;
            T instance;
            if(typeof(T).IsValueType)
            {
                instance = default(T);
            } else
            {
                ConstructorInfo constructor = typeof(T).GetConstructor(new Type[0]);
                if (constructor == null) throw new MissingMethodException("Missing parameterless constructor for type '" + typeof(T) + "'");
                instance = (T) constructor.Invoke(new object[0]);
            }

            object boxed = instance;

            foreach(PropertyInfo property in typeof(T).GetProperties())
            {
                PersistentPropertyAttribute attr = property.GetCustomAttribute(typeof(PersistentPropertyAttribute), false) as PersistentPropertyAttribute;
                if (attr == null) continue;
                string key = attr.KeyName ?? property.Name.ToSnakeCase();

                if (!property.CanWrite) throw new MissingMethodException("Property " + property.Name + " of type '" + typeof(T).Name + "' can't be written to.");

                bool containsKey = obj.ContainsKey(key);
                if (!containsKey) continue;

                object newValue = typeof(TagCompound).GetMethod("Get", new Type[] { typeof(TagMaster), typeof(string) }).MakeGenericMethod(property.PropertyType).Invoke(obj, new object[] { json, key });

                property.SetValue(boxed, newValue);
            }

            foreach(FieldInfo field in typeof(T).GetFields())
            {
                PersistentPropertyAttribute attr = field.GetCustomAttribute(typeof(PersistentPropertyAttribute), false) as PersistentPropertyAttribute;
                if (attr == null) continue;
                string key = attr.KeyName ?? field.Name.ToSnakeCase();

                //if (!field.) throw new MissingMethodException("Field" + field.Name + " can't be written to.");

                bool containsKey = obj.ContainsKey(key);
                if (!containsKey) continue;

                object newValue = typeof(TagCompound).GetMethod("Get", new Type[] { typeof(TagMaster), typeof(string) }).MakeGenericMethod(field.FieldType).Invoke(obj, new object[] { json, key });

                if(newValue != null) field.SetValue(boxed, newValue);
            }

            return (T)boxed;
        }
        public ITag ToJson(TagMaster json, object raw)
        {
            TagCompound obj = new TagCompound();

            foreach (PropertyInfo property in raw.GetType().GetProperties())
            {
                PersistentPropertyAttribute attr = property.GetCustomAttribute(typeof(PersistentPropertyAttribute), false) as PersistentPropertyAttribute;
                if (attr == null) continue;
                string name = attr.KeyName ?? property.Name.ToSnakeCase();
                object value = property.GetValue(raw);
                if(value != null) obj.AddProperty(name, value);
            }

            foreach (FieldInfo field in raw.GetType().GetFields())
            {
                PersistentPropertyAttribute attr = field.GetCustomAttribute(typeof(PersistentPropertyAttribute), false) as PersistentPropertyAttribute;
                if (attr == null) continue;
                string name = attr.KeyName ?? field.Name.ToSnakeCase();
                object value = field.GetValue(raw);
                if(value != null) obj.AddProperty(name, value);
            }

            return obj;
        }
    }
}
