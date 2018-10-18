using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Interfaces.Input;
using GameInterfaces.Input;
using WooferGame.Input;

namespace WooferGame.Systems.Menu
{
    [ComponentSystem("pausing", ProcessingCycles.Input, ProcessingFlags.Pause)]
    class PauseSystem : ComponentSystem
    {
        public override void Input()
        {
            ButtonInput state = Woofer.Controller.InputManager.ActiveInputMap.Pause;
            if (state.Consume())
            {
                Woofer.Controller.Paused = !Woofer.Controller.Paused;
            }
        }
    }
}
