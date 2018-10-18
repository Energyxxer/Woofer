using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using WooferGame.Systems.Meta;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Interaction
{
    [ComponentSystem("interaction_icon_system", ProcessingCycles.Update),
        Watching(typeof(InteractionIcon)),
        Listening(typeof(InteractionRangeEnter), typeof(InteractionRangeExit))]
    class InteractionIconSystem : ComponentSystem
    {
        private Interactable Focused = null;

        public override void Update()
        {
            if (Focused == null) return;
            InteractionIcon icon = WatchedComponents.FirstOrDefault() as InteractionIcon;
            if (icon == null) return;
            icon.Owner.Components.Get<Spatial>().Position = Focused.Owner.Components.Get<Spatial>().Position + Focused.IconOffset;
        }

        public override void EventFired(object sender, Event evt)
        {
            InteractionIcon icon = WatchedComponents.FirstOrDefault() as InteractionIcon;
            if (icon == null) return;
            if(evt is InteractionRangeEnter enter)
            {
                if(enter.Interactable.Owner.Components.Has<Spatial>())
                {
                    icon.Owner.Active = true;
                    icon.Owner.Components.Get<Spatial>().Position = enter.Interactable.Owner.Components.Get<Spatial>().Position + enter.Interactable.IconOffset;
                    Focused = enter.Interactable;
                }
            } else if(evt is InteractionRangeExit)
            {
                icon.Owner.Active = false;
                Focused = null;
            }
        }
    }
}
