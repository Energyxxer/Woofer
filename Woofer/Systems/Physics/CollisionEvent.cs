using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Physics
{
    [Event("rigid_collision")]
    class RigidCollisionEvent : Event
    {
        public Entity Victim { get; private set; }
        public Vector2D Normal { get; private set; }

        public RigidCollisionEvent(Component sender, Entity victim, Vector2D normal) : base(sender)
        {
            this.Victim = victim;
            this.Normal = normal;
        }
    }
    [Event("soft_collision")]
    class SoftCollisionEvent : Event
    {
        public Entity Victim { get; private set; }

        public SoftCollisionEvent(Component sender, Entity victim) : base(sender)
        {
            this.Victim = victim;
        }
    }
}
