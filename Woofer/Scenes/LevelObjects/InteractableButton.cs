using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Visual;

namespace WooferGame.Scenes.LevelObjects
{
    class InteractableButton : Entity
    {
        public InteractableButton(Vector2D pos, long idToActivate)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Renderable(new Sprite("lab_objects", new Rectangle(-4.5, -4.5, 9, 9), new Rectangle(0, 288, 9, 9))));
            this.Components.Add(new LevelRenderable());
            this.Components.Add(new Interactable() { IconOffset = new Vector2D(8, 8) });
            this.Components.Add(new LinkedActivationComponent(idToActivate));
        }
    }
}
