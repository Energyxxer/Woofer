using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Scenes;

namespace WooferGame.Scenes.Intro
{
    class IntroScreen : Scene
    {
        public IntroScreen() : base(Woofer.Controller)
        {
            Systems.Add(new IntroSystem());
        }
    }
}
