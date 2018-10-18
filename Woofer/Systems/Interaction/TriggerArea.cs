using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Puzzles;

namespace WooferGame.Systems.Interaction
{
    class TriggerArea : Entity
    {
        public TriggerArea(Rectangle bounds, bool oneTime) : this(bounds, 0, oneTime)
        {

        }
        public TriggerArea(Rectangle bounds, long idToActivate, bool oneTime)
        {
            this.Components.Add(new Spatial(bounds.Position));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new SoftBody(new CollisionBox(0, 0, bounds.Width, bounds.Height), 0f));
            if(idToActivate != 0) this.Components.Add(new LinkedActivationComponent(idToActivate));
            this.Components.Add(new SwitchComponent() { PlayerOnly = true, OneTimeUse = oneTime });
        }
    }
}
