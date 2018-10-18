using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Generators
{
    [Component("room_builder")]
    class RoomBuilder : Component
    {
        [PersistentProperty]
        public string Spritesheet { get; set; } = "lab_tileset";

        [PersistentProperty]
        public int Width { get; set; } = 64;
        [PersistentProperty]
        public int Height { get; set; } = 64;

        public bool Refresh { get; set; } = false;
        
        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Offset { get; set; }

        [PersistentProperty]
        public bool[,] Map { get; set; } = new bool[0, 0];

        public bool Build { get; set; } = false;

        public bool OutputCollision { get; set; } = true;
        public bool OutputSprites { get; set; } = true;

        public int RandomSeed { get; set; } = 0;


        [PersistentProperty]
        public bool SecondaryTilesetEnabled { get; set; } = true;
        [PersistentProperty]
        public Vector2D SecondaryTilesetOffset { get; set; } = new Vector2D(0, 256);

        [PersistentProperty]
        public int PrimaryTilesetRandomTiles { get; set; } = 0;
        [PersistentProperty]
        public float PrimaryTilesetRandomChance { get; set; } = 0.15f;

        [PersistentProperty]
        public int SecondaryTilesetRandomTiles { get; set; } = 0;
        [PersistentProperty]
        public float SecondaryTilesetRandomChance { get; set; } = 0.15f;
        [PersistentProperty]
        public bool Overwrite { get; set; } = true;
    }
}
