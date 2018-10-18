using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Generators
{
    [ComponentSystem("room_builder_system", ProcessingCycles.Update, ProcessingFlags.Pause),
        Watching(typeof(RoomBuilder))]
    class RoomBuilderSystem : ComponentSystem
    {
        private int Width;
        private int Height;
        private RoomTileRaw[,] TileMap;

        private RoomTileRaw this[int x, int y] => (x >= 0 && y >= 0 && x < Width && y < Height) ? TileMap[x, y] : RoomTileRaw.CreateOutOfBounds();

        private RoomBuilder Current;

        private Random Random;

        public override void Update()
        {
            foreach(RoomBuilder rb in WatchedComponents)
            {
                if (rb.Build && (rb.Width != rb.Map.GetLength(0) || rb.Height != rb.Map.GetLength(1))) rb.Refresh = true;
                if(rb.Refresh)
                {
                    bool[,] old = rb.Map;
                    rb.Map = new bool[rb.Width, rb.Height];
                    for(int i = 0; i < Math.Min(old.GetLength(0), rb.Width); i++)
                    {
                        for(int j = 0; j < Math.Min(old.GetLength(1), rb.Height); j++)
                        {
                            rb.Map[i, j] = old[i, j];
                        }
                    }
                    rb.Refresh = false;
                }
                if(rb.Build)
                {
                    Width = rb.Width;
                    Height = rb.Height;
                    TileMap = new RoomTileRaw[Width, Height];
                    for(int i = 0; i < Width; i++)
                    {
                        for(int j = 0; j < Height; j++)
                        {
                            TileMap[i, j] = new RoomTileRaw(rb.Map[i, j]);
                        }
                    }
                    Current = rb;
                    Random = new Random(rb.RandomSeed);
                    ResolveNeighbors();
                    if (rb.OutputSprites)
                    {
                        List<Sprite> sprites = BuildSprites();
                        Renderable renderable = rb.Owner.Components.Get<Renderable>();
                        if (renderable == null) return;
                        if (rb.Overwrite) {
                            renderable.Sprites = sprites;
                        } else
                        {
                            renderable.Sprites.AddRange(sprites);
                        }
                    }
                    rb.Build = false;
                }
            }
        }

        private void ResolveNeighbors()
        {
            ResolveNeighbors(Vector2D.Empty);

            if(Current.SecondaryTilesetEnabled)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (TileMap[x, y].TileMapOffset == Vector2D.Empty && TileMap[x, y].Neighbors == 0b11111111)
                        {
                            TileMap[x, y].TileMapOffset = Current.SecondaryTilesetOffset;
                            TileMap[x, y].Neighbors = 0;
                            TileMap[x, y].Initialized = false;
                        }
                    }
                }
                ResolveNeighbors(Current.SecondaryTilesetOffset);
            }

        }

        private void ResolveNeighbors(Vector2D tilesetOffset)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!TileMap[x, y].Initialized && TileMap[x, y].Enabled && TileMap[x, y].TileMapOffset == tilesetOffset)
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

        public List<Sprite> BuildSprites()
        {
            List<Sprite> sprites = new List<Sprite>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (TileMap[x, y].Enabled)
                    {
                        Rectangle source = new Rectangle(TileMap[x, y].GetPrimaryNeighbors() * 16, 0, 16, 16);
                        source += TileMap[x, y].TileMapOffset;
                        byte significantSecondary = TileMap[x, y].GetSignificantSecondaryNeighbors();
                        source.Y += significantSecondary * 16;

                        if (Current.PrimaryTilesetRandomTiles > 0 && source.Y == 0)
                        {
                            if (Random.NextDouble() <= Current.PrimaryTilesetRandomChance)
                            {
                                source.X += 256;
                                source.Y += Random.Next(Current.PrimaryTilesetRandomTiles) * 16;
                            }
                        } else if(Current.SecondaryTilesetEnabled && Current.SecondaryTilesetRandomTiles > 0 && source.Y == Current.SecondaryTilesetOffset.Y)
                        {
                            if (Random.NextDouble() <= Current.PrimaryTilesetRandomChance)
                            {
                                source.X += 256;
                                source.Y += Random.Next(Current.SecondaryTilesetRandomTiles) * 16;
                            }
                        }

                        sprites.Add(new Sprite(Current.Spritesheet, new Rectangle(x * 16, y * 16, 16, 16) + Current.Offset, source));
                    }
                }
            }
            return sprites;
        }

        public List<CollisionBox> BuildCollision()
        {
            return null;
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
            return (byte)(Neighbors & 0b00001111);
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

            for (int i = 0b1000; i > 0; i >>= 1)
            {
                int otherPrimaryNeighbor = i << 1;
                if (otherPrimaryNeighbor > 0b1000) otherPrimaryNeighbor = 1;
                if ((primaryNeighbors & i) != 0 && (primaryNeighbors & otherPrimaryNeighbor) != 0)
                {
                    significant |= (byte)(((secondaryNeighbors & i) == 0 ? 1 : 0) << significantNeighborCount);
                    significantNeighborCount++;
                }
            }

            return significant;
        }

        public bool CanBlend(RoomTileRaw other)
        {
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
