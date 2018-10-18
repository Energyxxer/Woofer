using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using WooferGame.Common;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.HUD
{
    [Event("show_text")]
    class ShowTextEvent : Event
    {
        public TextUnit Text;
        public int TextSize = 1;

        public double Duration = 5;

        public ShowTextEvent(string text, Component sender) : this(null, text, sender) { }
        
        public ShowTextEvent(Sprite icon, string text, Component sender) : this(new TextUnit(icon, text), sender)
        {
        }

        public ShowTextEvent(TextUnit text, Component sender) : base(sender)
        {
            this.Text = text;
        }
    }
}
