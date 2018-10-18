using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace WooferGame.Systems.Interaction
{
    [Event("activation")]
    class ActivationEvent : Event
    {
        public Entity Affected { get; set; }
        public Event InnerEvent { get; set; }

        public ActivationEvent(Component sender, Entity affected, Event innerEvent) : base(sender)
        {
            this.Affected = affected;
            this.InnerEvent = innerEvent;
        }
    }
}
