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

namespace WooferGame.Systems.Commands
{
    [Component("scene_prepare")]
    class ScenePrepareComponent : Component
    {
        [PersistentProperty]
        public string Name { get; set; } = "";
    }
}
