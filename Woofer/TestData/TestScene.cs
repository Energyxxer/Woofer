using EntityComponentSystem.Entities;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;

using WooferGame.Systems.Camera;
using WooferGame.Systems.Movement;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player;
using WooferGame.Systems.Pulse;
using WooferGame.Systems.Visual;
using WooferGame.Test_Data;

namespace WooferGame.Scenes
{
    class TestScene : Scene
    {
        //TestEntity a;

        Entity player;

        public TestScene() : base(Woofer.Controller)
        {
            //Entities.Add(new TestEntity(0, 0));
            //Entities.Add(new TestEntity(10, 10));
            //Entities.Add(new TestEntity(137+10, 125));
            //CurrentViewport.X = 16;
            /*for(int i = 0; i < 100; i++)
            {
                Entities.Add(new TestEntity(0, i));
            }*/
            //CurrentViewport.Scale = 0.5f;
            CurrentViewport.Y = 64;

            //Entities.Add(new TestEntity(0, 0));

            /*SpriteSheet sprites0 = new SpriteSheet(Properties.Resources.sprites0);
            sprites0.AddRegion("grass", 0, 0, 16, 16);
            sprites0.AddRegion("brick", 16, 0, 16, 16);
            sprites0.AddRegion("brick_slope_right", 16, 16, 16, 16);
            sprites0.AddRegion("brick_slope_left", 16, 32, 16, 16);
            sprites0.AddRegion("brick_slope_upside_down", 16, 48, 16, 16);
            Sprites.AddSpriteSheet(sprites0);

            SpriteSheet note = new SpriteSheet(Properties.Resources.notes);
            note.AddRegion("note", 0, 0, 32, 32);
            Sprites.AddSpriteSheet(note);*/

            //Entities.Add(new TileEntity("grass", 16, 8));
            Entities.Add(new Room0(0, 128));

            /*for (int x = -1; x <= 100; x++)
            {
                int h = (int) Math.Round(5*Math.Sin(x / Math.PI));
                for(int y = 0; y <= h+5; y++)
                {
                    Entities.Add(new TileEntity("brick", x, y-5));
                }
            }*/
            /*Slope sl;
            Entities.Add(sl = new Slope("brick_slope_right", 3, 5));
            Entities.Add(sl = new Slope("brick_slope_right", 2, 4));
            Entities.Add(sl = new Slope("brick_slope_right", 1, 3));
            Entities.Add(sl = new Slope("brick_slope_right", 0, 1));
            Entities.Add(sl = new Slope("brick_slope_right", -1, -1));*/
            //sl.Components.Get<Physical>().Velocity = new Vector2D(8, -2);

            for (int x = -1; x <= 20; x++)
            {
                Entities.Add(new TileEntity("brick", x, 0));


                //Entities.Add(new TileEntity("brick", x, 8));

                //if (x < 2 || x > 18) Entities.Add(new TileEntity("brick", x, 1));
                //else if(x < 10) Entities.Add(new Box(x * 16, 2));
            }
            for (int y = 1; y <= 5; y++)
            {
                //Entities.Add(new TileEntity("brick", 7, y));
            }
            //Entities.Add(new TileEntity("brick", 15, 1));
            Entities.Add(new TileEntity("brick", 0, 128/16));
            

            Entities.Add(player = new PlayerEntity(-24, 64));

            //Entities.Add(new Box(0f, 16f));
            //Entities.Add(new Slope(0, 1, true));
            //Entities.Add(new Slope(1, 1, false));

            //Input
            Systems.Add(new PlayerMovement());
            Systems.Add(new PlayerOrientationSystem());

            //Tick
            Systems.Add(new PhysicsSystem());
            Systems.Add(new PulseSystem());
            Systems.Add(new CameraSystem());

            //Rendering
            Systems.Add(new LevelRenderer());
        }


        protected override void Update()
        {
            /*System.Console.WriteLine($"DeltaTime: \t{DeltaTime}");
            System.Console.WriteLine($"FixedDeltaTime: {FixedDeltaTime}");
            System.Console.WriteLine();*/


            var boxrb = player.Components.Get<Physical>();
            //var boxrb = (box.Components["rigidbody"]) as RigidBody;
            if (boxrb.Position.Y < -1000)
            {
                boxrb.Position = new Vector2D(32.001, 64);
                boxrb.Velocity = new Vector2D();
            }
            //CurrentViewport.Y = (float) (16d*Math.Sin(2*time));
            //Console.WriteLine("Ticking");
            
            //CurrentViewport.Scale = 2;
            //CurrentViewport.Scale += (float)DeltaTime * 1.1f;
            //CurrentViewport.Scale += (float)DeltaTime * change;
            //if (CurrentViewport.Scale >= 2) change = -0.1f;
            //else if (CurrentViewport.Scale <= 0.1) change = 0.1f;

            //(a.Components["physical"] as Physical).X += 10 * DeltaTime;
        }
    }
}
