using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Commands;
using EntityComponentSystem.Scenes;

namespace WooferGame.Controller.Commands
{
    class InternalSceneChangeCommand : Command
    {
        public Scene NewScene;

        public InternalSceneChangeCommand(Scene scene) => NewScene = scene;
    }

    class SavedSceneChangeCommand : Command
    {
        public string SceneName;

        public SavedSceneChangeCommand(string sceneName) => SceneName = sceneName;
    }
}
