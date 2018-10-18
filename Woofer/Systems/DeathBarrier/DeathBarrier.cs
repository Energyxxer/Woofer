using EntityComponentSystem.Entities;

namespace WooferGame.Systems.DeathBarrier
{
    class DeathBarrier : Entity
    {
        public DeathBarrier(double y)
        {
            this.Components.Add(new DeathBarrierComponent(y));
        }
    }
}
