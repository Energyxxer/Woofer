using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Scenes;
using WooferGame.Scenes.Intro;

namespace WooferGame.Scenes
{
    class EndScene : Scene
    {
        public EndScene() : base(Woofer.Controller)
        {
            Systems.Add(new IntroSystem() { Delay = 60, Locked = true });
        }
    }
}
