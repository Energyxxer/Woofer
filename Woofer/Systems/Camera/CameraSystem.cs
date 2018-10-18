using System.Linq;

using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Camera
{
    [ComponentSystem("camera_system", ProcessingCycles.Input | ProcessingCycles.Update),
        Watching(typeof(CameraTracked)),
        Listening(typeof(CameraLocationQueryEvent))]
    class CameraSystem : ComponentSystem
    {
        public override void Input()
        {
            Owner.CurrentViewport.Location += Woofer.Controller.InputManager.ActiveInputMap.Movement;
        }

        public override void Update()
        {
            CameraLocationQueryEvent query = new CameraLocationQueryEvent(null, Owner.CurrentViewport.Location);
            Owner.Events.InvokeEvent(query);

            Owner.CurrentViewport.Location = query.SuggestedLocation;
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is CameraLocationQueryEvent qe && WatchedComponents.Count > 0)
            {
                qe.SuggestedLocation = WatchedComponents.First().Owner.Components.Get<Spatial>().Position +
                    (WatchedComponents.First() as CameraTracked).Offset;
            }
        }
    }
}
