using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Camera
{
    [Component("camera_region")]
    class CameraRegionComponent : Component
    {
        [PersistentProperty]
        public Rectangle Area { get; set; }
        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Focus { get; set; }

        [PersistentProperty]
        public double Easing { get; set; } = 0;
        [PersistentProperty]
        public double EasingStep { get; set; } = 0.01;
        [PersistentProperty]
        public double MaxEasing { get; set; } = 1;

        public CameraRegionComponent()
        {
        }

        public CameraRegionComponent(Rectangle area) : this(area, area.Center)
        {

        }

        public CameraRegionComponent(Rectangle area, Vector2D focus)
        {
            this.Area = area;
            this.Focus = focus;
        }
    }
}
