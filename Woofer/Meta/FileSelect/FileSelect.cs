using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Scenes;

namespace WooferGame.Meta.FileSelect
{
    class FileSelect : Scene
    {
        public FileSelect() : base(Woofer.Controller)
        {
            Systems.Add(new FileSelectSystem());
        }
    }
}
