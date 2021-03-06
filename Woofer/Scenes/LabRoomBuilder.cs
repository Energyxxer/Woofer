﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;

namespace WooferGame.Scenes
{
    class LabRoomBuilder
    {
        public string Spritesheet { get; set; }
        public RoomTileRaw[,] TileMap { get; set; }

        private readonly int Width;
        private readonly int Height;

        private Random Random;

        private RoomTileRaw this[int x, int y] => (x >= 0 && y >= 0 && x < Width && y < Height) ? TileMap[x, y] : RoomTileRaw.CreateOutOfBounds();

        public LabRoomBuilder(int width, int height, string Texture, int seedChange = 0)
        {
            Spritesheet = Texture;
            Width = width;
            Height = height;
            Random = new Random(width * height + seedChange);
            TileMap = new RoomTileRaw[width, height];
        }

        public void Set(int x, int y, bool enabled)
        {
            TileMap[x, y].Enabled = enabled;
        }

        public void Set(int x, int y, RoomTileRaw tileInfo)
        {
            TileMap[x, y] = tileInfo;
        }

        public void Fill(Rectangle rectangle, bool enabled)
        {
            for (int x = (int)rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (int y = (int)rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    Set(x, y, enabled);
                }
            }
        }

        public void Fill(Rectangle rectangle, RoomTileRaw tileInfo)
        {
            for (int x = (int)rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (int y = (int)rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    Set(x, y, tileInfo);
                }
            }
        }

        public void ResolveNeighbors()
        {
            ResolveNeighbors(Vector2D.Empty);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (TileMap[x, y].TileMapOffset == Vector2D.Empty && TileMap[x, y].Neighbors == 0b11111111)
                    {
                        TileMap[x, y].TileMapOffset = new Vector2D(0, 256);
                        TileMap[x, y].Neighbors = 0;
                        TileMap[x, y].Initialized = false;
                    }
                }
            }
            ResolveNeighbors(new Vector2D(0, 256));
        }

        private void ResolveNeighbors(Vector2D offset)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!TileMap[x, y].Initialized && TileMap[x, y].Enabled && TileMap[x, y].TileMapOffset == offset)
                    {
                        RoomTileRaw tile = TileMap[x, y];

                        byte primaryNeighbors =
                            (byte)(
                            ((this[x, y + 1].CanBlend(tile) ? 1 : 0) << 3) +
                            ((this[x + 1, y].CanBlend(tile) ? 1 : 0) << 2) +
                            ((this[x, y - 1].CanBlend(tile) ? 1 : 0) << 1) +
                            ((this[x - 1, y].CanBlend(tile) ? 1 : 0)));
                        byte secondaryNeighbors =
                            (byte)(
                            ((this[x - 1, y + 1].CanBlend(tile) ? 1 : 0) << 3) +
                            ((this[x + 1, y + 1].CanBlend(tile) ? 1 : 0) << 2) +
                            ((this[x + 1, y - 1].CanBlend(tile) ? 1 : 0) << 1) +
                            ((this[x - 1, y - 1].CanBlend(tile) ? 1 : 0)));

                        tile.Neighbors = (byte)(primaryNeighbors + (secondaryNeighbors << 4));
                        tile.Initialized = true;

                        TileMap[x, y] = tile;
                    }
                }
            }
        }

        private delegate RoomTileRaw HandleTile(RoomTileRaw tile);

        public List<Sprite> Build()
        {
            List<Sprite> sprites = new List<Sprite>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if(TileMap[x, y].Enabled)
                    {
                        Rectangle source = new Rectangle(TileMap[x,y].GetPrimaryNeighbors() * 16, 0, 16, 16);
                        source += TileMap[x, y].TileMapOffset;
                        byte significantSecondary = TileMap[x, y].GetSignificantSecondaryNeighbors();
                        source.Y += significantSecondary * 16;

                        if(source.Y == 0)
                        {
                            if(Random.NextDouble() <= 0.15)
                            {
                                source.X += 256;
                                if(Random.NextDouble() <= 0.5)
                                {
                                    source.Y += 16;
                                }
                            }
                        }

                        sprites.Add(new Sprite(Spritesheet, new Rectangle(x*16, y*16, 16, 16), source));
                    }
                }
            }
            return sprites;
        }
    }

    public struct RoomTileRaw
    {
        public bool Enabled;
        public byte Neighbors;
        public bool Initialized;
        public Vector2D TileMapOffset;
        private bool OutOfBounds;

        public RoomTileRaw(bool enabled)
        {
            Enabled = enabled;
            Neighbors = 0b00000000;
            Initialized = false;
            TileMapOffset = default(Vector2D);
            OutOfBounds = false;
        }

        public byte GetPrimaryNeighbors()
        {
            return (byte) (Neighbors & 0b00001111);
        }

        public byte GetSecondaryNeighbors()
        {
            return (byte)((Neighbors & 0b11110000) >> 4);
        }

        public byte GetSignificantSecondaryNeighbors()
        {
            byte primaryNeighbors = GetPrimaryNeighbors();
            byte secondaryNeighbors = GetSecondaryNeighbors();

            byte significant = 0;

            int significantNeighborCount = 0;

            for(int i = 0b1000; i > 0; i >>= 1)
            {
                int otherPrimaryNeighbor = i << 1;
                if (otherPrimaryNeighbor > 0b1000) otherPrimaryNeighbor = 1;
                if((primaryNeighbors & i) != 0 && (primaryNeighbors & otherPrimaryNeighbor) != 0)
                {
                    significant |= (byte) (((secondaryNeighbors & i) == 0 ? 1 : 0) << significantNeighborCount);
                    significantNeighborCount++;
                }
            }

            return significant;
        }
        
        public bool CanBlend(RoomTileRaw other) {
            return (this.Enabled && other.Enabled) && (this.OutOfBounds || other.OutOfBounds || this.TileMapOffset == other.TileMapOffset);
        }

        public static RoomTileRaw CreateOutOfBounds()
        {
            return new RoomTileRaw()
            {
                Enabled = true,
                Neighbors = 0,
                Initialized = false,
                TileMapOffset = default(Vector2D),
                OutOfBounds = true
            };
        }
    }
}
