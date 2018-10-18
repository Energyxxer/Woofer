using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Converter;
using EntityComponentSystem.Saves.Json.Objects;

namespace WooferGame.Systems.Physics
{
    class CollisionBoxConverter : ITagConverter
    {
        public Type GetWorkingType() => typeof(CollisionBox);
        public T FromJson<T>(TagMaster json, ITag value)
        {
            TagCompound obj = value as TagCompound;
            CollisionBox box = new CollisionBox();
            TagList bounds = obj.Get<TagList>("bounds");
            box.X = (bounds[0] as TagDouble).Value;
            box.Y = (bounds[1] as TagDouble).Value;
            box.Width = (bounds[2] as TagDouble).Value;
            box.Height = (bounds[3] as TagDouble).Value;

            TagList faces = obj.Get<TagList>("faces");
            box.TopFaceProperties = FaceFromJson(faces[0] as TagCompound);
            box.RightFaceProperties = FaceFromJson(faces[1] as TagCompound);
            box.BottomFaceProperties= FaceFromJson(faces[2] as TagCompound);
            box.LeftFaceProperties = FaceFromJson(faces[3] as TagCompound);
            return (T)Convert.ChangeType(box, typeof(T));
        }

        private CollisionFaceProperties FaceFromJson(TagCompound obj)
        {
            return new CollisionFaceProperties()
            {
                Enabled = obj.Get<TagBoolean>("enabled").Value,
                Friction = obj.Get<TagDouble>("friction").Value,
                Snap = obj.Get<TagBoolean>("snap").Value
            };
        }

        public ITag ToJson(TagMaster json, object rawBox)
        {
            CollisionBox box = (CollisionBox)rawBox;
            TagCompound obj = new TagCompound();

            TagList rect = new TagList();
            obj.AddProperty("bounds", rect);
            rect.Add(box.X);
            rect.Add(box.Y);
            rect.Add(box.Width);
            rect.Add(box.Height);
            TagList faces = new TagList();
            obj.AddProperty("faces", faces);
            foreach(CollisionFaceProperties face in box.GetFaceProperties())
            {
                TagCompound faceObj = new TagCompound();
                faces.Add(faceObj);
                faceObj.AddProperty("enabled", face.Enabled);
                faceObj.AddProperty("friction", face.Friction);
                faceObj.AddProperty("snap", face.Snap);
            }
            return obj;
        }
    }
}
