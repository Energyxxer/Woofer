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

namespace WooferGame.Systems.Camera
{
    [ComponentSystem("camera_region_system", ProcessingCycles.None),
        Watching(typeof(CameraRegionComponent)),
        Listening(typeof(CameraLocationQueryEvent))]
    class CameraRegionSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event evt)
        {
            if(evt is CameraLocationQueryEvent qe)
            {
                foreach(CameraRegionComponent region in WatchedComponents)
                {
                    Spatial sp = region.Owner.Components.Get<Spatial>();
                    Rectangle area = region.Area;
                    if (sp != null) area += sp.Position;

                    bool contained = false;

                    if((contained = area.Contains(qe.SuggestedLocation)) || region.Easing > 0)
                    {
                        region.Easing += (contained ? 1 : -1) * region.EasingStep;
                        if (region.Easing < 0) region.Easing = 0;
                        if (region.Easing > region.MaxEasing) region.Easing = region.MaxEasing;

                        Vector2D oldPos = qe.SuggestedLocation;
                        Vector2D newPos = region.Focus;

                        if (sp != null) newPos += sp.Position;

                        Vector2D mixedPos = region.Easing * newPos + (1 - region.Easing) * oldPos;

                        qe.SuggestedLocation = mixedPos;
                    }
                }
            }
        }
    }
}
