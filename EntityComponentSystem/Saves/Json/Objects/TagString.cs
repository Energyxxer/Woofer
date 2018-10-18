using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public class TagString : ITagPrimitive
    {
        internal const byte TypeId = 8;
        public readonly string Value;

        public TagString() : this("") { }

        public TagString(string value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            return 1 + TagMaster.WriteString(Value, writer);
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagString(TagMaster.ReadString(reader));
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
