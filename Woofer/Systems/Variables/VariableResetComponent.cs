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
    [Component("variable_reset")]
    class VariableResetComponent : Component
    {
        [PersistentProperty]
        public long EntityToReset = 0;
    }
}
