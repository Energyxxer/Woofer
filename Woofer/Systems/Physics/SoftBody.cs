using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Physics
{
    [Component("softbody")]
    class SoftBody : Component
    {
        [PersistentProperty]
        public CollisionBox Bounds { get; private set; } = new CollisionBox(-8, -8, 16, 16);
        [PersistentProperty]
        public float Mass { get; set; } = 0;
        [PersistentProperty]
        public bool Movable { get; set; } = true;
        [PersistentProperty]
        public bool PassThroughLevel = false;

        public SoftBody()
        {
        }

        public SoftBody(CollisionBox bounds, float mass)
        {
            Bounds = bounds;
            Mass = mass;
        }
    }
}
