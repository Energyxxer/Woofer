using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Cutscenes
{
    [Component("cutscene_node")]
    class CutsceneNode : Component
    {
        [PersistentProperty]
        public long Next = 0;

        [PersistentProperty]
        public float Delay = 0;
        [PersistentProperty]
        public float Duration = 1;

        [PersistentProperty]
        public bool FollowPlayer = false;

        [PersistentProperty]
        public bool Interpolate = true;
    }
}
