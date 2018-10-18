using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves;

namespace EntityComponentSystem.Util
{
    [PersistentObject]
    public class Size
    {
        [PersistentProperty]
        public double Width;
        [PersistentProperty]
        public double Height;

        public Size() : this(0, 0)
        {

        }

        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public System.Drawing.Size ToDrawing() => new System.Drawing.Size((int)Width, (int)Height);

        public override string ToString() => $"Size[{Width} x {Height}]";
    }
}
