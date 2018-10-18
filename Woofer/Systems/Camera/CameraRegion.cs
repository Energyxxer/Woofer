using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Camera
{
    class CameraRegion : Entity
    {
        public CameraRegion(Rectangle area) : this(area, area.Center)
        {

        }

        public CameraRegion(Rectangle area, Vector2D focus)
        {
            this.Components.Add(new Spatial(focus));
            this.Components.Add(new CameraRegionComponent(area - focus, Vector2D.Empty));
        }
    }
}
