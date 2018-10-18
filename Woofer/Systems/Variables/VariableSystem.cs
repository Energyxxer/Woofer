using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Variables
{
    [ComponentSystem("variables", ProcessingCycles.Render),
        Listening(typeof(ActivationEvent)),
        Watching(typeof(CounterVariableComponent), typeof(VariableResetComponent))]
    class VariableSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event e)
        {
            if(e is ActivationEvent ae)
            {
                if(ae.Affected.Components.Get<VariableResetComponent>() is VariableResetComponent reset)
                {
                    Entity toReset = GetAffected(ae.Affected, reset.EntityToReset);
                    if(toReset != null)
                    {
                        if(toReset.Components.Get<CounterVariableComponent>() is CounterVariableComponent counterToReset)
                        {
                            counterToReset.Value = counterToReset.StartValue;
                        }
                    }
                }
                if(ae.Affected.Components.Get<CounterVariableComponent>() is CounterVariableComponent counter)
                {
                    if(counter.Value >= counter.EndValue) counter.Value = counter.StartValue;
                    counter.Value += counter.Increment;
                    if(counter.Value >= counter.EndValue)
                    {
                        if(counter.TriggerId != 0)
                        {
                            Entity trigger = Owner.Entities[counter.TriggerId];
                            if (trigger != null) Owner.Events.InvokeEvent(new ActivationEvent(counter, trigger, ae));
                        }
                    }
                }
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            foreach(CounterVariableComponent counter in WatchedComponents.Where(c => c is CounterVariableComponent))
            {
                if(counter.DoRender)
                {
                    Renderable renderable = counter.Owner.Components.Get<Renderable>();
                    if (renderable == null) continue;
                    if(renderable.Sprites.Count == 0)
                    {
                        renderable.Sprites.Add(new Sprite("lab_objects", new Rectangle(-4, -8, 8, 16), new Rectangle(0, 400, 8, 16)));
                        renderable.Sprites.Add(new Sprite("lab_objects", new Rectangle(-4, -8, 8, 16), new Rectangle(8, 400, 8, 16)));
                    }

                    if(renderable.Sprites.Count >= 2)
                    {
                        if (counter.EndValue - counter.StartValue == 0) continue;
                        float percent = (float)(counter.Value-counter.StartValue) / (counter.EndValue-counter.StartValue);
                        int height = (int)Math.Round(percent * 12) + 2;

                        renderable.Sprites[1].Destination.Height = height;
                        renderable.Sprites[1].Source.Y = 400 + 16 - height;
                        renderable.Sprites[1].Source.Height = height;
                    }
                }
            }
        }

        private Entity GetAffected(Entity owner, long id)
        {
            if (id == 0) return owner;
            return Owner.Entities[id];
        }
    }
}
