using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves.Json.Objects;

namespace EntityComponentSystem.Saves.Json.Converter.DefaultConverters
{
    public class StringConverter : ITagConverter
    {
        public Type GetWorkingType() => typeof(string);
        public K FromJson<K>(TagMaster json, ITag value)
        {
            if (value is TagString str)
            {
                return (K)Convert.ChangeType(str.Value, typeof(K));
            }
            else return default(K);
        }
        public ITag ToJson(TagMaster json, object obj) => throw new NotImplementedException();
    }
}
