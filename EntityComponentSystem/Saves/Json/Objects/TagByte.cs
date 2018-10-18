using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public class TagByte : ITagPrimitive
    {
        internal const byte TypeId = 2;
        public readonly byte Value;

        public TagByte() : this(0) { }

        public TagByte(byte value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Value);
            return 2;
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagByte(reader.ReadByte());
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
