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
    class Shelf : Entity
    {
        public Shelf(Vector2D pos, HorizontalDirection direction)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });

            double width = 80;
            double height = 8;

            Rectangle platformBounds = new Rectangle(direction == HorizontalDirection.Right ? 0 : -width, 0, width, height);

            this.Components.Add(new RigidBody(new CollisionBox(platformBounds.X, platformBounds.Y, platformBounds.Width, platformBounds.Height)
            {
                LeftFaceProperties = new CollisionFaceProperties(),
                RightFaceProperties = new CollisionFaceProperties(),
                BottomFaceProperties = new CollisionFaceProperties()
            }));

            Rectangle platformVisibleBounds = platformBounds - new Vector2D(0, 9);
            platformVisibleBounds.Height += 9;

            this.Components.Add(new Renderable(new Sprite("lab_objects", platformVisibleBounds, platformVisibleBounds + new Vector2D(80, 73))));
            this.Components.Add(new LevelRenderable());
        }
    }
}
