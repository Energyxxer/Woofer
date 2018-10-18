using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Puzzles
{
    [Component("avoid_corners")]
    class AvoidCorners : Component
    {
        [PersistentProperty]
        public float RecordedMass;

        [PersistentProperty]
        public bool Bouncing;
    }
}
