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

namespace WooferGame.Systems.Variables
{
    [Component("counter_variable")]
    class CounterVariableComponent : Component
    {
        [PersistentProperty]
        public int StartValue = 0;
        [PersistentProperty]
        public int Value = 0;
        [PersistentProperty]
        public int EndValue = 10;
        [PersistentProperty]
        public int Increment = 1;

        [PersistentProperty]
        public long TriggerId = 0;

        [PersistentProperty]
        public bool DoRender = false;
    }
}
