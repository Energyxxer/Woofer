using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Setters
{
    [Component("position_setter")]
    class PositionSetter : Component
    {
        [PersistentProperty]
        public long ChangedId = 0;
        [PersistentProperty]
        public Vector2D Amount = new Vector2D();
        [PersistentProperty]
        public bool Add = false;

        public PositionSetter()
        {

        }

        public PositionSetter(long changedId, Vector2D amount, bool add)
        {
            ChangedId = changedId;
            Amount = amount;
            Add = add;
        }
    }
}
