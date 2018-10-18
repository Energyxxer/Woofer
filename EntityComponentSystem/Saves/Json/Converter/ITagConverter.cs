using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json.Objects;

namespace EntityComponentSystem.Saves.Json.Converter
{
    public interface ITagConverter
    {
        Type GetWorkingType();
        ITag ToJson(TagMaster json, object obj);
        T FromJson<T>(TagMaster json, ITag value);
    }
}
