using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Scenes;

namespace WooferGame.Meta.Loading
{
    class LoadingScreen : Scene
    {
        public LoadingScreen() : base(Woofer.Controller)
        {
            Systems.Add(new LoadingScreenSystem());
        }
    }
}
