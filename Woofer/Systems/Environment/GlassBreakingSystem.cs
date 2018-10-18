using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

using WooferGame.Systems.Camera.Shake;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Pulse;

namespace WooferGame.Systems.Environment
{
    [ComponentSystem("glass_breaking", ProcessingCycles.None),
        Listening(typeof(ActivationEvent))]
    class GlassBreakingSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event re)
        {
            if(re is ActivationEvent e && e.Affected.Components.Get<BreakableGlassComponent>() is BreakableGlassComponent glass)
            {
                glass.CurrentHits++;
                Owner.Events.InvokeEvent(new CameraShakeEvent(e.Sender, 8 * ((e.InnerEvent is PulseEvent pe && pe.Direction.Magnitude > 0) ? pe.Direction.Normalize() : -Vector2D.UnitJ)));
                if (glass.CurrentHits >= glass.MaxHits)
                {
                    glass.CurrentHits = 0;
                    e.Affected.Active = false;
                }
            }
        }
    }
}
