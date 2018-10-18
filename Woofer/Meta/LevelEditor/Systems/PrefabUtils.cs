using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Converter.DefaultConverters;
using EntityComponentSystem.Saves.Json.Objects;
using WooferGame.Common;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Generators;
using WooferGame.Systems.Sounds;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;
using WooferGame.Systems.HealthSystems;

namespace WooferGame.Meta.LevelEditor.Systems
{
    static class TagIOUtils
    {
        private static readonly string PrefabDirectory = Path.Combine(Woofer.DirectoryPath, "prefabs");

        private static readonly List<string> PrefabNames = new List<string>();

        public static readonly TagMaster Master = new TagMaster();

        static TagIOUtils()
        {
            Master.RegisterConverter(new ListConverter<long>());
            Master.RegisterConverter(new ListConverter<int>());

            Master.RegisterConverter(new NumberConverter<byte>());
            Master.RegisterConverter(new NumberConverter<short>());
            Master.RegisterConverter(new NumberConverter<int>());
            Master.RegisterConverter(new NumberConverter<float>());
            Master.RegisterConverter(new NumberConverter<long>());
            Master.RegisterConverter(new NumberConverter<double>());

            Master.RegisterConverter(new StringConverter());
            Master.RegisterConverter(new BooleanConverter());

            Master.RegisterConverter(new CollisionBoxConverter());
            Master.RegisterConverter(new ColorConverter());
            Master.RegisterConverter(new ListConverter<CollisionBox>());
            Master.RegisterConverter(new ListConverter<Sound>());
            Master.RegisterConverter(new ListConverter<Sprite>());
            Master.RegisterConverter(new ListConverter<AnimatedSprite>());
            Master.RegisterConverter(new EnumConverter<DrawMode>());
            Master.RegisterConverter(new EnumConverter<DamageFilter>());
            Master.RegisterConverter(new BoolMapConverter());
        }

        public static Entity CloneEntity(Entity entity)
        {
            TagCompound componentRoot = new TagCompound();
            foreach (Component component in entity.Components)
            {
                componentRoot.AddProperty(component.ComponentName, component);
            }

            componentRoot.Resolve(Master);
            
            Entity copy = new Entity
            {
                Name = entity.Name,
                Active = entity.Active
            };

            foreach (KeyValuePair<string, ITag> rawComponent in componentRoot)
            {
                Type componentType = Component.TypeForIdentifier(rawComponent.Key);
                Component component = Master.ConvertFromValue(rawComponent.Value, componentType) as Component;
                copy.Components.Add(component);
            }
            return copy;
        }

        public static void Refresh()
        {
            PrefabNames.Clear();
            if (!Directory.Exists(PrefabDirectory)) Directory.CreateDirectory(PrefabDirectory);

            foreach (string filepath in Directory.GetFiles(PrefabDirectory))
            {
                FileInfo file = new FileInfo(filepath);
                if(file.Extension == ".prefab")
                {
                    string filename = file.Name;
                    PrefabNames.Add(filename.Substring(0, filename.Length - file.Extension.Length));
                }
            }
        }

        public static void SavePrefab(Entity entity, string name)
        {
            FileInfo file = new FileInfo(Path.Combine(PrefabDirectory, name + ".prefab"));

            try
            {
                TagCompound obj = new TagCompound();
                obj.AddProperty("name", entity.Name);
                obj.AddProperty("active", entity.Active);
                TagCompound componentRoot = new TagCompound();
                obj.AddProperty("components", componentRoot);
                foreach(Component component in entity.Components)
                {
                    componentRoot.AddProperty(component.ComponentName, component);
                }

                using (BinaryWriter writer = new BinaryWriter(new FileStream(file.FullName, FileMode.Create)))
                {
                    Master.Write(obj, writer);
                }
                Refresh();
            }
            catch (FormatException x)
            {
                Console.WriteLine(x.Message);
            }
        }

        public static Entity InstantiatePrefab(string name)
        {
            FileInfo file = new FileInfo(Path.Combine(PrefabDirectory, name + ".prefab"));
            if (!file.Exists) return null;

            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(file.FullName, FileMode.Open)))
                {
                    TagCompound obj = Master.Read(reader);
                    Entity entity = new Entity
                    {
                        Name = obj.Get<TagString>("name").Value,
                        Active = obj.Get<TagBoolean>("active").Value
                    };

                    foreach (KeyValuePair<string, ITag> rawComponent in obj.Get<TagCompound>("components"))
                    {
                        Type componentType = Component.TypeForIdentifier(rawComponent.Key);
                        Component component = Master.ConvertFromValue(rawComponent.Value, componentType) as Component;
                        entity.Components.Add(component);
                    }
                    return entity;
                }
            } catch(FormatException x)
            {
                Console.WriteLine(x.Message);
                return null;
            }
        }

        public static List<string> GetPrefabNames() => PrefabNames.ToList();
    }
}
