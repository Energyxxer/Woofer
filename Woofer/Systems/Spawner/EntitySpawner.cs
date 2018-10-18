using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Spawner
{
    [Component("entity_spawner")]
    class EntitySpawner : Component
    {
        [PersistentProperty]
        public long Blueprint = 0;

        [PersistentProperty]
        public int MaxEntities = 10;
    }
}
