using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using GameInterfaces.Controller;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Generators
{
    [ComponentSystem("tiling_generation", ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Watching(typeof(TilingGenerator))]
    class TilingGenerationSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach(TilingGenerator gen in WatchedComponents)
            {
                if(gen.Generate)
                {
                    gen.Generate = false;
                    if (gen.TileBounds.Width == 0)
                    {
                        Console.WriteLine("TileBounds width must not be zero");
                        continue;
                    }
                    if (gen.TileBounds.Height == 0)
                    {
                        Console.WriteLine("TileBounds height must not be zero");
                        continue;
                    }
                    if(gen.Sprites.Count == 0)
                    {
                        Console.WriteLine("TilingGenerator must have at least one sprite");
                        continue;
                    }

                    Renderable renderable = gen.Owner.Components.Get<Renderable>();
                    if (renderable == null) continue;

                    Random random = new Random();

                    List<Sprite> newSprites = new List<Sprite>();

                    for(double x = gen.MaxBounds.X; x < gen.MaxBounds.Right; x += gen.TileBounds.Width)
                    {
                        for(double y = gen.MaxBounds.Y; y < gen.MaxBounds.Top; y += gen.TileBounds.Height)
                        {
                            double possibleWidth = Math.Min((gen.MaxBounds.Right - x) / gen.TileBounds.Width, 1);
                            double possibleHeight = Math.Min((gen.MaxBounds.Top - y) / gen.TileBounds.Height, 1);
                            Sprite selected = gen.Sprites[random.Next(gen.Sprites.Count)].Clone();
                            selected.Destination.X = x;
                            selected.Destination.Y = y;
                            selected.Destination.Width *= possibleWidth;
                            selected.Destination.Height *= possibleHeight;
                            //selected.Source.X += selected.Source.Width * (1 - possibleWidth);
                            selected.Source.Y += selected.Source.Height * (1 - possibleHeight);
                            selected.Source.Width *= possibleWidth;
                            selected.Source.Height *= possibleHeight;
                            newSprites.Add(selected);
                        }
                    }

                    renderable.Sprites.Clear();
                    renderable.Sprites.AddRange(newSprites);
                }
            }
        }
        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var layer = r.GetLayerGraphics("level");
            foreach(TilingGenerator gen in WatchedComponents)
            {
                if(gen.Preview)
                {
                    Renderable.Render(layer, Owner.CurrentViewport, r, gen.Sprites, gen.Owner.Components.Get<Spatial>());
                }
            }
        }
    }
}
