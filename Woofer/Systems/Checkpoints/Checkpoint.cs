using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Checkpoints
{
    class Checkpoint : Entity
    {
        public Checkpoint(double x, double y, Rectangle bounds) : this(x, y, bounds, false)
        {

        }

        public Checkpoint(double x, double y, Rectangle bounds, bool selected)
        {
            this.Components.Add(new Spatial(x, y));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new SoftBody(new CollisionBox(bounds.X, bounds.Y, bounds.Width, bounds.Height), 0f) { Movable = false });
            this.Components.Add(new CheckpointComponent() { Selected = selected });
            //this.Components.Add(new Renderable("grass", bounds));
            //this.Components.Add(new LevelRenderable());
        }
    }
}
