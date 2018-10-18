using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems
{
    [Component("spatial")]
    public class Spatial : Component
    {
        [PersistentProperty("pos")] [Inspector(InspectorEditType.Position)]
        public Vector2D Position = new Vector2D();

        public Spatial() { }

        public Spatial(double x, double y) : this()
        {
            this.Position = new Vector2D(x, y);
        }

        public Spatial(Vector2D position)
        {
            this.Position = position;
        }
    }
}
