using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Setters
{
    [Component("active_setter")]
    class ActiveSetter : Component
    {
        [PersistentProperty]
        public long ChangedId = 0;
        [PersistentProperty]
        public bool Active = false;
    }
}
