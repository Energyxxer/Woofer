using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Puzzles
{
    [Component("door")]
    class DoorComponent : Component
    {
        [PersistentProperty]
        public double MaxOpenDistance = 48 - 8;
        [PersistentProperty]
        public double CurrentOpenDistance = 0;
        [PersistentProperty]
        public double OpeningTime = 0.5;

        [PersistentProperty]
        public double CurrentOpenTime = 0;

        [PersistentProperty]
        public double OpeningDirection = 0;

        [PersistentProperty]
        public bool Toggle = false;

        [HideInInspector]
        [PersistentProperty]
        public double OpenY = 0;

        [HideInInspector]
        [PersistentProperty]
        public double ClosedY = 0;
    }
}
