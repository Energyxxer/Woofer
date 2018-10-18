using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json.Objects;

namespace EntityComponentSystem.Saves.Json.Converter.DefaultConverters
{
    public class EnumConverter<T> : ITagConverter
    {
        public Type GetWorkingType() => typeof(T);
        public K FromJson<K>(TagMaster json, ITag value)
        {
            TagString str = value as TagString;
            foreach(T o in typeof(T).GetEnumValues())
            {
                if (typeof(T).GetEnumName(o) == str.Value) return (K)Convert.ChangeType(o, typeof(K));
            }
            return default(K);
        }
        public ITag ToJson(TagMaster json, object obj)
        {
            return new TagString(typeof(T).GetEnumName(obj));
        }
    }
}
