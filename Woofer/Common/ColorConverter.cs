using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json;
using EntityComponentSystem.Saves.Json.Converter;
using EntityComponentSystem.Saves.Json.Objects;

namespace WooferGame.Common
{
    class ColorConverter : ITagConverter
    {
        public Type GetWorkingType() => typeof(Color);
        public T FromJson<T>(TagMaster json, ITag value)
        {
            TagList list = (TagList)value;
            byte r = ((TagByte)list[0]).Value;
            byte g = ((TagByte)list[1]).Value;
            byte b = ((TagByte)list[2]).Value;
            byte a = ((TagByte)list[3]).Value;

            return (T)Convert.ChangeType(Color.FromArgb(a, r, g, b), typeof(T));
        }
        public ITag ToJson(TagMaster json, object obj)
        {
            Color color = (Color)obj;

            TagList list = new TagList();
            list.Add(new TagByte(color.R));
            list.Add(new TagByte(color.G));
            list.Add(new TagByte(color.B));
            list.Add(new TagByte(color.A));
            return list;
        }
    }
}
