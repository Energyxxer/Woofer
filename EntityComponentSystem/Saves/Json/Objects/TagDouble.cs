using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public class TagDouble : ITagPrimitive
    {
        internal const byte TypeId = 7;
        public readonly double Value;

        public TagDouble() : this(0) { }

        public TagDouble(double value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Value);
            return 9;
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagDouble(reader.ReadDouble());
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
