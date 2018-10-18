using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Puzzles
{
    class Door : Entity
    {
        public Door(Vector2D pos) : this(pos, false)
        {
        }

        public Door(Vector2D pos, bool toggle)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new RigidBody(new CollisionBox(-8, -48, 16, 48)));
            this.Components.Add(new Renderable(new Sprite("lab_objects", new Rectangle(-8, -48, 16, 48), new Rectangle(0, 224, 16, 48))));
            this.Components.Add(new LevelRenderable(-1));

            this.Components.Add(new DoorComponent() { Toggle = toggle });
        }
    }
}
