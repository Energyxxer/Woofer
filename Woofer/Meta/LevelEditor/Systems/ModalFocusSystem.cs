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

namespace WooferGame.Meta.LevelEditor.Systems
{
    [ComponentSystem("ModalFocusSystem", ProcessingCycles.Input, ProcessingFlags.Pause),
        Listening(typeof(ForceModalChangeEvent), typeof(RequestModalChangeEvent))]
    class ModalFocusSystem : ComponentSystem
    {
        public string CurrentSystem = "editor_cursor";

        public override bool ShouldSave => false;

        public override void Input()
        {
            ButtonInput changeButton = Woofer.Controller.InputManager.ActiveInputMap.Back;
            if(changeButton.Consume())
            {
                BeginModalChange();
            }
        }

        private void BeginModalChange()
        {
            BeginModalChangeEvent begin = new BeginModalChangeEvent(null);
            Owner.Systems[CurrentSystem].EventFired(this, begin);
            Owner.Events.InvokeEvent(begin);
            if (begin.SystemName != null)
            {
                ChangeSystem(begin.SystemName);
            }
        }

        private void ChangeSystem(string name)
        {
            ModalChangeEvent change = new ModalChangeEvent(CurrentSystem, null);
            Owner.Systems[name].EventFired(this, change);
            if(change.Valid) CurrentSystem = name;
        }

        public override void EventFired(object sender, Event e)
        {
            if(e is ForceModalChangeEvent ce)
            {
                ChangeSystem(ce.SystemName);
            } else if(e is RequestModalChangeEvent)
            {
                BeginModalChange();
            }
        }
    }
}
