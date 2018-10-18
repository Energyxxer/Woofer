using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public interface ITag
    {
        int Write(TagMaster json, BinaryWriter writer);
    }
}
