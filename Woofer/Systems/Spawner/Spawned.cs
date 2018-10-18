using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Spawner
{
    [Component("spawned")]
    class Spawned : Component
    {
        [PersistentProperty]
        public long SpawnerId;
    }
}
