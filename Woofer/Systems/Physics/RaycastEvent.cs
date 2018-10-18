using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Physics
{
    [Event("raycast")]
    class RaycastEvent : Event
    {
        public FreeVector2D Ray;
        public List<RaycastIntersection> Intersected;

        public RaycastEvent(Component sender, FreeVector2D ray) : base(sender) => Ray = ray;
    }
}
