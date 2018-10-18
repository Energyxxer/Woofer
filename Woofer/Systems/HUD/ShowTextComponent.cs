using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooferGame.Common;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.HUD
{
    [Component("show_text")]

    class ShowTextComponent : Component 
    {
        [PersistentProperty]
        public TextUnit Text;
        [PersistentProperty]
        public double Duration;

        public ShowTextComponent() : this("", 5)
        {
        }

        public ShowTextComponent(Sprite icon, string text, double duration) : this(new TextUnit(icon, text), duration)
        {
        }

        public ShowTextComponent(string text, double duration) : this(new TextUnit(text), duration)
        {
        }

        public ShowTextComponent(TextUnit text, double duration)
        {
            Text = text;
            Duration = duration;
        }
    }
}
