using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;

using GameInterfaces.Controller;
using WooferGame.Systems;
using WooferGame.Systems.DeathBarrier;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Pulse;
using WooferGame.Systems.Visual;

namespace WooferGame.Test_Data
{
    class Box : Entity
    {
        public Box(Vector2D position)
        {
            Components.Add(new Spatial(position));
            Components.Add(new Renderable("grass", new Rectangle(-8, -8, 16, 16)));
            Components.Add(new Physical());
            Components.Add(new SoftBody(new CollisionBox(-8, -8, 16, 16), 8f));
            Components.Add(new PulsePushable());
            Components.Add(new LevelRenderable());
            Components.Add(new RemoveOnBarrierComponent());
        }

        public override string ToString() => "Box{Position=" + (Components["spatial"] as Spatial).Position + "}";
    }

    class TileEntity : Entity
    {
        public TileEntity(string texture, double tileX, double tileY)
        {
            Components.Add(new Spatial(16 * tileX, 16 * tileY));
            Components.Add(new Renderable(texture, new Rectangle(0, 0, 16, 16)));
            Components.Add(new Physical() { GravityMultiplier = 0 });
            Components.Add(new RigidBody(new CollisionBox[] {new CollisionBox(0, 0, 16, 16)
            {
                TopFaceProperties = new CollisionFaceProperties(true, 0.3)/*,
                LeftFaceProperties = new CollisionFaceProperties(),
                BottomFaceProperties = new CollisionFaceProperties(),
                RightFaceProperties = new CollisionFaceProperties()*/
            }}));
            Components.Add(new LevelRenderable());
        }

        public override string ToString() => "TileEntity{Position=" + (Components["spatial"] as Spatial).Position + "}";
    }

    class Slope : Entity
    {
        public Slope(string texture, double tileX, double tileY)
        {
            Components.Add(new Spatial(tileX, tileY));
            Components.Add(new Renderable(texture, new Rectangle(-8, -8, 16, 16)));
            Components.Add(new Physical() { GravityMultiplier = 0 });
            Components.Add(new RigidBody(new CollisionBox[] {
                new CollisionBox(-8, -8, 16, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                },
                new CollisionBox(-6, -6, 14, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                },
                new CollisionBox(-4, -4, 12, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                },
                new CollisionBox(-2, -2, 10, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                },
                new CollisionBox(0, 0, 8, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                },
                new CollisionBox(2, 2, 6, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                },
                new CollisionBox(4, 4, 4, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                },
                new CollisionBox(6, 6, 2, 2)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, 0.3, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                }
            }));
            Components.Add(new LevelRenderable());
        }

        public override string ToString() => "Slab{Position=" + (Components["spatial"] as Spatial).Position + "}";
    }

    [Component("level_tile")]
    class LevelTile : Component
    {
    }

    [ComponentSystem("fpscounter",
        ProcessingCycles.Update | ProcessingCycles.Render
        )]
    public class FramerateCounter : ComponentSystem
    {
        private float _elapsed = 0;
        private int frames = 0;
        public int Framerate { get; private set; }

        //private Font font;

        public override void Update()
        {
            frames++;
            _elapsed += Owner.DeltaTime;
            if (_elapsed >= 1)
            {
                _elapsed--;
                Framerate = frames;
                frames = 0;
            }
        }
        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            /*r.BeginRenderingSection(false);

            Surface textSurface = font.Render($"{Framerate} fps", Color.Yellow);
            r.View.Blit(textSurface, new Point(0, 0));
            textSurface.Dispose();
            //g.DrawString(, new Font("Consolas", 12), new SolidBrush(Color.Yellow), 0, 0);

            r.EndRenderingSection();*/
        }
    }
}
