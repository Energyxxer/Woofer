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
    public class TagFloat : ITagPrimitive
    {
        internal const byte TypeId = 5;
        public readonly float Value;

        public TagFloat() : this(0) { }

        public TagFloat(float value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Value);
            return 5;
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagFloat(reader.ReadSingle());
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
