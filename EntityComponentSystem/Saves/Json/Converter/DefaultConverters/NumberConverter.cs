using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json.Objects;

namespace EntityComponentSystem.Saves.Json.Converter.DefaultConverters
{
    public class NumberConverter<T> : ITagConverter
    {
        public Type GetWorkingType() => typeof(T);
        public K FromJson<K>(TagMaster json, ITag value) {
            switch(value)
            {
                case TagByte num: return (K)Convert.ChangeType(num.Value, typeof(K));
                case TagShort num: return (K)Convert.ChangeType(num.Value, typeof(K));
                case TagInt num: return (K)Convert.ChangeType(num.Value, typeof(K));
                case TagFloat num: return (K)Convert.ChangeType(num.Value, typeof(K));
                case TagLong num: return (K)Convert.ChangeType(num.Value, typeof(K));
                case TagDouble num: return (K)Convert.ChangeType(num.Value, typeof(K));
                default: return default(K);
            }
        }
        public ITag ToJson(TagMaster json, object obj) => throw new NotImplementedException();
    }
}
