﻿using System;
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
    public class TagShort : ITagPrimitive
    {
        internal const byte TypeId = 3;
        public readonly short Value;

        public TagShort() : this(0) { }

        public TagShort(short value) => Value = value;

        public int Write(TagMaster json, BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Value);
            return 3;
        }

        internal static ITag Parse(BinaryReader reader)
        {
            return new TagShort(reader.ReadInt16());
        }

        internal static bool IsValidStart(byte id)
        {
            return id == TypeId;
        }
    }
}
