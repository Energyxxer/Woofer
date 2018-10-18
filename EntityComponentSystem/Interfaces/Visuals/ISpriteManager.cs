using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.GraphicsInterface
{
    public interface ISpriteManager<TSource>
    {
        TSource this[string name] { get; set; }

        void LoadSprite(string name);
    }
}
