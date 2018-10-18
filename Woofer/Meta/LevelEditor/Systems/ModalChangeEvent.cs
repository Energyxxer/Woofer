using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [Event("editor_begin_modal_change")]
    class BeginModalChangeEvent : Event
    {
        public string SystemName;

        public BeginModalChangeEvent(Component sender) : base(sender)
        {
        }
    }

    [Event("editor_modal_change")]
    class ModalChangeEvent : Event
    {
        public string From;
        public bool Valid = true;

        public ModalChangeEvent(string old, Component sender) : base(sender)
        {
            this.From = old;
        }
    }

    [Event("force_editor_modal_change")]
    class ForceModalChangeEvent : Event
    {
        public string SystemName;

        public ForceModalChangeEvent(string systemName, Component sender) : base(sender)
        {
            this.SystemName = systemName;
        }
    }

    [Event("request_editor_modal_change")]
    class RequestModalChangeEvent : Event
    {
        public RequestModalChangeEvent(Component sender) : base(sender)
        {
        }
    }
}
