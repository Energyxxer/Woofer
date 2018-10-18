using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using WooferGame.Controller.Commands;
using WooferGame.Systems.Interaction;

namespace WooferGame.Systems.Commands
{
    [ComponentSystem("CommandSystem", ProcessingCycles.Update),
        Listening(typeof(ActivationEvent))]
    class CommandSystem : ComponentSystem
    {
        private string ScheduledSceneChange = null;

        public override void Update()
        {
            if(ScheduledSceneChange != null)
            {
                Owner.Controller.CommandFired(new SavedSceneChangeCommand(ScheduledSceneChange));
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if(e is ActivationEvent ae)
            {
                if(ae.Affected.Components.Get<ResolutionChangeComponent>() is ResolutionChangeComponent resolutionChange)
                {
                    Owner.Controller.CommandFired(new ResolutionChangeCommand(resolutionChange.Resolution.ToDrawing()));
                }
                else if (ae.Affected.Components.Get<SceneChangeComponent>() is SceneChangeComponent sceneChange)
                {
                    ScheduledSceneChange = sceneChange.Name;
                }
                else if (ae.Affected.Components.Get<ScenePrepareComponent>() is ScenePrepareComponent scenePrepare)
                {
                    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Woofer.Controller.CurrentSave.PrepareScene(scenePrepare.Name);
                    #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }
    }
}
