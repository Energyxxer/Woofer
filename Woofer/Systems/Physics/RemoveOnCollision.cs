using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace WooferGame.Systems.Physics
{
    [Component("remove_on_collision")]
    class RemoveOnCollision : Component
    {
        public bool RemoveOnRigid = true;
        public bool RemoveOnSoft = false;
    }
}
