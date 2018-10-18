using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;

using WooferGame.Systems.Physics;
using WooferGame.Systems.Pulse;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Environment
{
    class BreakableGlassEntity : Entity
    {
        public BreakableGlassEntity(Rectangle bounds, Rectangle textureSource, Vector2D textureOffset)
        {
            Components.Add(new Spatial(bounds.X, bounds.Y));
            Components.Add(new Physical() { GravityMultiplier = 0 });
            Components.Add(new RigidBody(new CollisionBox[] { new CollisionBox(0, 0, bounds.Width, bounds.Height) }));
            Components.Add(new PulseReceiverPhysical());
            Components.Add(new Renderable(new Sprite("lab_objects", new Rectangle(textureOffset, textureSource.Size), textureSource)));

            Components.Add(new LevelRenderable());

            Components.Add(new BreakableGlassComponent());
        }
    }
}
