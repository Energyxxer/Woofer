using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Commands;

namespace WooferGame.Controller.Commands
{
    class ResolutionChangeCommand : Command
    {
        public Size NewResolution;

        public ResolutionChangeCommand(Size newResolution) => NewResolution = newResolution;
    }
}
