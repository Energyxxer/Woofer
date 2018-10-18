using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.DeathBarrier
{
    [Component("death_barrier")]
    class DeathBarrierComponent : Component
    {
        [PersistentProperty]
        public double Y { get; private set; }

        public DeathBarrierComponent() : this(0) { }

        public DeathBarrierComponent(double y) => Y = y;
    }
}
