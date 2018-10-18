using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace EntityComponentSystem.Saves.Json.Objects
{
    public class TagInt : ITagPrimitive
    {
        internal const byte TypeId = 4;
        public readonly int Value;

        public TagInt() : this(0) { }

        public TagInt(int value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Value);
            return 5;
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagInt(reader.ReadInt32());
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
