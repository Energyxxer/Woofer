using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Converter;
using EntityComponentSystem.Saves.Json.Objects;

namespace WooferGame.Systems.Generators
{
    class BoolMapConverter : ITagConverter
    {
        public Type GetWorkingType() => typeof(bool[,]);
        public T FromJson<T>(TagMaster json, ITag value)
        {
            TagCompound wrapper = (TagCompound)value;
            int width = wrapper.Get<int>(json, "width");
            int height = wrapper.Get<int>(json, "height");
            TagList outer = wrapper.Get<TagList>("map");
            bool[,] map = new bool[width, height];
            for(int i = 0; i < width; i++)
            {
                TagList inner = (TagList)outer[i];
                for(int j = 0; j < height; j++)
                {
                    map[i,j] = ((TagBoolean)inner[j]).Value;
                }
            }
            return (T)Convert.ChangeType(map, typeof(T));
        }
        public ITag ToJson(TagMaster json, object obj)
        {
            bool[,] map = (bool[,])obj;
            TagCompound wrapper = new TagCompound();
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            wrapper.AddProperty("width", width);
            wrapper.AddProperty("height", height);
            TagList outer = new TagList();
            wrapper.AddProperty("map", outer);

            for(int i = 0; i < width; i++)
            {
                TagList inner = new TagList();
                outer.Add(inner);
                for(int j = 0; j < height; j++)
                {
                    inner.Add(map[i, j]);
                }
            }
            return wrapper;
        }
    }
}
