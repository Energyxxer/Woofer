using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Controller
{
    public interface IRenderingLayer
    {
        string Name { get; }
        Size LayerSize { get; }
        Rectangle Destination { get; }
    }

    public class RenderingLayer : IRenderingLayer
    {
        public string Name { get; set; }
        public Size LayerSize { get; set; }
        public Rectangle Destination { get; set; }
    }
}
