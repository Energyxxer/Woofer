using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Converter;
using EntityComponentSystem.Saves.Json.Converter.DefaultConverters;
using EntityComponentSystem.Saves.Json.Objects;
using EntityComponentSystem.Scenes;
using GameInterfaces.Controller;

namespace EntityComponentSystem.Saves
{
    public class LoadOperation
    {
        private readonly IGameController Controller;
        private readonly string Path;
        private List<ITagConverter> Converters = new List<ITagConverter>();

        public LoadOperation(IGameController controller, string path)
        {
            Controller = controller;
            Path = path;
        }

        public void AddConverter(ITagConverter converter)
        {
            Converters.Add(converter);
        }

        public Scene Load()
        {
            if (!File.Exists(Path))
            {
                Console.WriteLine("Tried to load scene at '" + Path + "', does not exist");
                return null;
            }
            TagMaster tagMaster = new TagMaster();
            
            tagMaster.RegisterConverter(new ListConverter<long>());
            tagMaster.RegisterConverter(new ListConverter<int>());

            tagMaster.RegisterConverter(new NumberConverter<byte>());
            tagMaster.RegisterConverter(new NumberConverter<short>());
            tagMaster.RegisterConverter(new NumberConverter<int>());
            tagMaster.RegisterConverter(new NumberConverter<float>());
            tagMaster.RegisterConverter(new NumberConverter<long>());
            tagMaster.RegisterConverter(new NumberConverter<double>());

            tagMaster.RegisterConverter(new StringConverter());
            tagMaster.RegisterConverter(new BooleanConverter());
            foreach (ITagConverter converter in Converters)
            {
                tagMaster.RegisterConverter(converter);
            }

            BinaryReader reader = new BinaryReader(new FileStream(Path, FileMode.Open));

            TagCompound root = tagMaster.Read(reader);

            reader.Close();
            reader.Dispose();

            Scene scene = new Scene(Controller);

            TagCompound sceneRoot = root.Get<TagCompound>("scene");

            scene.Name = sceneRoot.Get<string>(tagMaster, "name") ?? "";

            scene.CurrentViewport = sceneRoot.Get<CameraView>(tagMaster, "viewport");

            foreach(ITag rawEntity in sceneRoot.Get<TagList>("entities"))
            {
                TagCompound obj = rawEntity as TagCompound;
                Entity entity = new Entity
                {
                    _id = obj.Get<long>(tagMaster, "id"),
                    _is_id_set = true,
                    Name = obj.Get<TagString>("name").Value,
                    Active = obj.Get<TagBoolean>("active").Value
                };

                foreach (KeyValuePair<string, ITag> rawComponent in obj.Get<TagCompound>("components"))
                {
                    Type componentType = Component.TypeForIdentifier(rawComponent.Key);
                    Component component = tagMaster.ConvertFromValue(rawComponent.Value, componentType) as Component;
                    entity.Components.Add(component);
                }
                scene.Entities.Add(entity);
            }

            foreach(ITag rawName in sceneRoot.Get<TagList>("systems"))
            {
                TagString name = rawName as TagString;
                ComponentSystem system = ComponentSystem.TypeForIdentifier(name.Value).GetConstructor(new Type[0]).Invoke(new object[0]) as ComponentSystem;
                scene.Systems.Add(system);
            }

            return scene;
        }
    }
}
