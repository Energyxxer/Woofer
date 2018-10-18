using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json.Objects;

namespace EntityComponentSystem.Saves.Json.Converter.DefaultConverters
{
    public class ListConverter<T> : ITagConverter
    {
        public Type GetWorkingType() => typeof(IEnumerable<T>);
        public K FromJson<K>(TagMaster json, ITag value)
        {
            TagList arr = value as TagList;
            List<T> list = new List<T>();
            foreach(ITag element in arr)
            {
                list.Add(json.ConvertFromValue<T>(element));
            }
            if (typeof(K).IsArray) return (K)Convert.ChangeType(list.ToArray(), typeof(K));
            else return (K)Convert.ChangeType(list, typeof(K));
        }
        public ITag ToJson(TagMaster json, object rawList)
        {
            IEnumerable<T> list = (IEnumerable<T>)rawList;
            TagList arr = new TagList();
            foreach(T t in list)
            {
                arr.Add(t);
            }
            return arr;
        }
    }
}
