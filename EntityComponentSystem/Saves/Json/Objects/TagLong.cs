using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public class TagLong : ITagPrimitive
    {
        internal const byte TypeId = 6;
        public readonly long Value;

        public TagLong() : this(0) { }

        public TagLong(long value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Value);
            return 9;
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagLong(reader.ReadInt64());
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
