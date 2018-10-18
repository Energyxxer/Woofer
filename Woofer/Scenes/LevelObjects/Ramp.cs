using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;

namespace WooferGame.Scenes.LevelObjects
{
    class Ramp : Entity
    {

        public Ramp(Vector2D from, Vector2D to, double friction, Vector2D spriteOffset)
        {
            this.Components.Add(new Spatial(from));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new RigidBody(CreateCollisionBoxesForSlope(from-from, to-from, friction).ToArray()));

            double minX = Math.Min(from.X, to.X) - 8;
            double width = Math.Abs(from.X - to.X) + 16;
            double minY = Math.Min(from.Y, to.Y);
            double height = Math.Abs(from.Y - to.Y);

            double descent = 22;

            double slope = (to.Y - from.Y) / (to.X - from.X);

            Renderable rend;

            this.Components.Add(rend = new Renderable(
                new Sprite("lab_objects", 
                new Rectangle(minX - from.X + spriteOffset.X, minY - from.Y + spriteOffset.Y - descent, width, height + descent), 
                new Rectangle((slope > 0) ? width : 0, 0, width, height+descent))
                ));
            this.Components.Add(new LevelRenderable());

        }

        public static List<CollisionBox> CreateCollisionBoxesForSlope(Vector2D from, Vector2D to, double friction)
        {
            List<CollisionBox> collisionBoxes = new List<CollisionBox>();

            double minX = Math.Min(from.X, to.X);
            double width = Math.Abs(from.X - to.X);
            double minY = Math.Min(from.Y, to.Y);
            double height = Math.Abs(from.Y - to.Y);

            double slope = (to.Y - from.Y) / (to.X - from.X);

            double stepWidth;
            double stepHeight;

            if (width >= height)
            {
                stepWidth = width / height;
                stepHeight = 1;
            }
            else
            {
                stepWidth = 1;
                stepHeight = height / width;
            }
            if (stepWidth <= 0) throw new ArgumentException("Width cannot be zero");
            if (stepHeight <= 0) throw new ArgumentException("Height cannot be zero");

            double y = (slope < 0) ? minY + height - stepHeight : minY;

            for (double x = minX; x < minX + width; x += stepWidth)
            {
                collisionBoxes.Add(new CollisionBox(x, y, stepWidth, stepHeight)
                {
                    TopFaceProperties = new CollisionFaceProperties(true, friction, true),
                    LeftFaceProperties = new CollisionFaceProperties(),
                    RightFaceProperties = new CollisionFaceProperties()
                });
                collisionBoxes.Add(new CollisionBox(x + (slope < 0 ? -stepWidth : +stepWidth), y, stepWidth, stepHeight));
                y += (slope < 0) ? -stepHeight : stepHeight;
            }

            return collisionBoxes;
        }
    }
}
