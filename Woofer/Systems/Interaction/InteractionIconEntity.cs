using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Interaction
{
    class InteractionIconEntity : Entity
    {
        public InteractionIconEntity()
        {
            this.Components.Add(new Spatial());
            this.Components.Add(new Renderable(new Sprite("gui", new Rectangle(-4.5, -4.5, 9, 9), new Rectangle(0, 18, 9, 9)) { Modifiers = Sprite.Mod_InputType}));
            this.Components.Add(new LevelRenderable(99));
            this.Components.Add(new InteractionIcon());

            this.Active = false;
        }
    }
}
