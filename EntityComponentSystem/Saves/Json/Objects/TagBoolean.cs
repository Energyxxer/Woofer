using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public class TagBoolean : ITagPrimitive
    {
        internal const byte TypeId = 1;
        public readonly bool Value;

        public TagBoolean() : this(false) { }

        public TagBoolean(bool value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Value);
            return 2;
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagBoolean(reader.ReadBoolean());
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
