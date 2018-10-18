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
using GameInterfaces.GraphicsInterface;
using WooferGame.Systems.Interaction;

namespace WooferGame.Systems.HUD
{
    [ComponentSystem("HintSystem", ProcessingCycles.Update | ProcessingCycles.Render),
        Listening(typeof(ShowTextEvent), typeof(ActivationEvent))]
    class HintSystem : ComponentSystem
    {
        private List<ShowTextEvent> Active = new List<ShowTextEvent>();

        public override void EventFired(object sender, Event e)
        {
            if(e is ShowTextEvent te)
            {
                Active.Insert(0, te);
            }
            else if(e is ActivationEvent ae)
            {
                ShowTextComponent comp = ae.Affected.Components.Get<ShowTextComponent>();

                if (comp == null) return;

                Owner.Events.InvokeEvent(new ShowTextEvent(comp.Text, comp) { Duration = comp.Duration });
            }
        }
        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if(Active.Count > 0)
            {
                var layer = r.GetLayerGraphics("hud");

                int destY = 3*layer.GetSize().Height/4;

                foreach(ShowTextEvent current in Active)
                {
                    System.Drawing.Size size = current.Text.GetSize(current.TextSize);
                    
                    int destX = layer.GetSize().Width / 2 - size.Width / 2;

                    var rect = new System.Drawing.Rectangle(destX, destY - size.Height, size.Width, size.Height);

                    current.Text.Render(r, layer, rect.Location, current.TextSize);

                    destY -= size.Height + 2;
                }
            }
        }
        public override void Update()
        {
            for(int i = 0; i < Active.Count; i++)
            {
                ShowTextEvent te = Active[i];
                if (te.Duration > 0) te.Duration -= Owner.DeltaTime;
                if (te.Duration <= 0)
                {
                    te.Duration = 0;
                    Active.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
