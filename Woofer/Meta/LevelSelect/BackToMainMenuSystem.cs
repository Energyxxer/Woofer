using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using WooferGame.Controller.Commands;
using WooferGame.Meta.LevelEditor.Systems;
using WooferGame.Scenes;

namespace WooferGame.Meta.LevelSelect
{
    [ComponentSystem("back_to_menu", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause)]
    class BackToMainMenuSystem : ComponentSystem
    {
        public override bool ShouldSave => false;

        public override void EventFired(object sender, Event e)
        {
            if(e is ModalChangeEvent change)
            {
                Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new MainMenuScene()));
            }
        }
    }
}
