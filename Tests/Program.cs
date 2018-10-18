using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Objects;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using WooferGame;
using WooferGame.Controller.Game;
using WooferGame.Scenes;
using WooferGame.Systems.Physics;

namespace Tests
{
    class Test : Attribute
    {
    }

    class Solo : Attribute
    {
    }

    class Program
    {
        public delegate void TestDelegate();

        static List<MethodInfo> possibleTests;
        static Dictionary<MethodInfo, Action> tests;

        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            possibleTests = assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(Test), false).Length > 0)
                      .ToList();

            Component.PopulateDictionaryWithAssembly(Assembly.GetAssembly(typeof(Woofer)));
            ComponentSystem.PopulateDictionaryWithAssembly(Assembly.GetAssembly(typeof(Woofer)));

            if (possibleTests.Any(m => m.GetCustomAttributes(typeof(Solo), false).Length > 0))
            {
                possibleTests.RemoveAll(m => m.GetCustomAttributes(typeof(Solo), false).Length <= 0);
            }

            tests = possibleTests.ToDictionary(t => t, t => (Action)t.CreateDelegate(typeof(Action)));

            int passed = 0;
            int failed = 0;
            int total = tests.Count;
            foreach (KeyValuePair<MethodInfo, Action> test in tests)
            {
                try
                {
                    //Console.WriteLine($"Testing '{test.Key}'...");
                    test.Value();
                    passed++;
                }
                catch (AssertionError x)
                {
                    Console.WriteLine("Failed test " + test.Key.Name + ":\n\t" + x.Message);
                    failed++;
                }
            }

            Console.WriteLine($"Finished {total} tests: {passed} passed, {failed} failed");
            Console.ReadLine();
        }

        [Test]
        public static void FloorModTest()
        {
            Assert(GeneralUtil.EuclideanMod(-1, 1), 0d);
            Assert(GeneralUtil.EuclideanMod(-2, 1), 0d);
            Assert(GeneralUtil.EuclideanMod(-1, 2), 1d);
            Assert(GeneralUtil.EuclideanMod(0, 2), 0d);
        }

        [Test]
        public static void AngleDifferenceTest()
        {
            Assert(GeneralUtil.SubtractAngles(Math.PI / 2, -Math.PI / 2), Math.PI);
            Assert(GeneralUtil.SubtractAngles(Math.PI / 2, Math.PI / 2), 0d);
            Assert(GeneralUtil.SubtractAngles(-Math.PI / 2, Math.PI / 2), Math.PI);
            Assert(GeneralUtil.SubtractAngles(Math.PI * 3d / 4, -Math.PI * 3d / 4), Math.PI / 2);
        }

        [Test]
        public static void IsAheadOfNormalTest()
        {
            Assert(PhysicsSystem.IsAheadOfNormal(new CollisionBox(-8, -8, 16, 16), new FreeVector2D(new Vector2D(10, 0), new Vector2D(10, 5))), true);
            Assert(PhysicsSystem.IsAheadOfNormal(new CollisionBox(-8, -8, 16, 16), new FreeVector2D(new Vector2D(7, 0), new Vector2D(7, 5))), false);
        }

        [Test]
        public static void TileNeighborTest()
        {
            RoomTileRaw tile = new RoomTileRaw
            {
                Enabled = true,
                Neighbors = 0b1011_1111
            };
            Assert(Convert.ToString(tile.GetSignificantSecondaryNeighbors(), 2), Convert.ToString(0b0010, 2));
        }
        
        public static void ThreadTest()
        {
            SaveGame sg = new SaveGame("scenes");
            Console.WriteLine("Loading scene");
            new Thread(() =>
            {
                Scene scene = sg.PrepareScene("Tutorial").Result;
                Console.WriteLine("Done loading scene");
            }).Start();
            Console.WriteLine("doing something else");
            Console.WriteLine("done");
        }

        [Test]
        public static void GameDataTest()
        {
            SaveGame sg = new SaveGame("main");
            sg.Data.HasWoofer = true;
            sg.Data.MaxEnergy = 160;
            sg.Save();
            Console.WriteLine(sg.Data);
        }

        public static void Assert(object real, object expected)
        {
            if (!Equals(real, expected))
            {
                throw new AssertionError($"Expected {expected}, instead got {real}");
            }
        }
    }

    [Serializable]
    internal class AssertionError : Exception
    {
        public AssertionError()
        {
        }

        public AssertionError(string message) : base(message)
        {
        }

        public AssertionError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AssertionError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
