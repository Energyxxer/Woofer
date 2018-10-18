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

namespace WooferGame.Systems.Interaction
{
    [Component("linked_activation")]
    class LinkedActivationComponent : Component
    {
        [PersistentProperty]
        public List<long> EntitiesToActivate { get; set; } = new List<long>();
        [PersistentProperty]
        public bool Enabled = true;
        [PersistentProperty]
        public bool OneTime = false;

        public LinkedActivationComponent()
        {
        }

        public LinkedActivationComponent(params long[] entitiesToActivate) => EntitiesToActivate = entitiesToActivate.ToList();
    }
}
