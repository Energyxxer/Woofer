using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Physics
{
    class RaycastIntersection
    {
        public Vector2D Point { get; set; }
        public FreeVector2D Side { get; set; }
        public CollisionBox Box { get; set; }
        public Component Component { get; set; }
        public CollisionFaceProperties FaceProperties { get; set; }

        public RaycastIntersection(Vector2D point, FreeVector2D side, CollisionBox box, Component component, CollisionFaceProperties faceProperties)
        {
            Point = point;
            Side = side;
            Box = box;
            Component = component;
            FaceProperties = faceProperties;
        }

        public override string ToString() => $"RaycastIntersection:[Point={Point},Side={Side},Box={Box},Component={Component},FaceProperties={FaceProperties}]";
    }
}
