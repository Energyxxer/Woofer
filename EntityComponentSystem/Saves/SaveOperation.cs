using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Converter;
using EntityComponentSystem.Saves.Json.Converter.DefaultConverters;
using EntityComponentSystem.Saves.Json.Objects;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Saves
{
    public class SaveOperation
    {
        private readonly Scene Scene;
        private readonly string Path;
        private List<ITagConverter> Converters = new List<ITagConverter>();

        public SaveOperation(Scene scene, string path)
        {
            Scene = scene;
            Path = path;
        }

        public void AddConverter(ITagConverter converter)
        {
            Converters.Add(converter);
        }

        public void Save()
        {
            TagMaster tagMaster = new TagMaster();

            tagMaster.RegisterConverter(new NumberConverter<byte>());
            tagMaster.RegisterConverter(new NumberConverter<short>());
            tagMaster.RegisterConverter(new NumberConverter<int>());
            tagMaster.RegisterConverter(new NumberConverter<float>());
            tagMaster.RegisterConverter(new NumberConverter<long>());
            tagMaster.RegisterConverter(new NumberConverter<double>());
            tagMaster.RegisterConverter(new StringConverter());

            tagMaster.RegisterConverter(new ListConverter<long>());
            tagMaster.RegisterConverter(new ListConverter<int>());
            foreach (ITagConverter converter in Converters)
            {
                tagMaster.RegisterConverter(converter);
            }

            TagCompound root = new TagCompound();

            TagCompound sceneRoot = new TagCompound();
            root.AddProperty("scene", sceneRoot);

            sceneRoot.AddProperty("name", Scene.Name);

            sceneRoot.AddProperty("viewport", Scene.CurrentViewport);
            
            {
                TagList entities = new TagList();
                sceneRoot.AddProperty("entities", entities);
                foreach(Entity entity in Scene.Entities)
                {
                    TagCompound entityObj = new TagCompound();
                    entities.Add(entityObj);
                    entityObj.AddProperty("name", entity.Name);
                    entityObj.AddProperty("id", entity.Id);
                    entityObj.AddProperty("active", entity.Active);
                    TagCompound components = new TagCompound();
                    entityObj.AddProperty("components", components);
                    foreach (Component component in entity.Components)
                    {
                        components.AddProperty(component.ComponentName, component);
                    }
                }
            }
            
            {
                TagList systems = new TagList();
                sceneRoot.AddProperty("systems", systems);
                foreach(ComponentSystem system in Scene.Systems)
                {
                    if(system.ShouldSave) systems.Add(system.SystemName);
                }
            }

            BinaryWriter writer = new BinaryWriter(new FileStream(Path, FileMode.Create));
            tagMaster.Write(root, writer);
            writer.Close();
            writer.Dispose();
        }
    }
}
