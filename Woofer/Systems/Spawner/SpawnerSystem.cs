using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using WooferGame.Meta.LevelEditor.Systems;
using WooferGame.Systems.Interaction;

namespace WooferGame.Systems.Spawner
{
    [ComponentSystem("spawner"), 
        Watching(typeof(Spawned)),
        Listening(typeof(ActivationEvent))]
    class SpawnerSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event e)
        {
            if(e is ActivationEvent ae && ae.Affected.GetComponent<EntitySpawner>() is EntitySpawner spawner)
            {
                if(spawner.MaxEntities <= 0 || WatchedComponents.OfType<Spawned>().Count(c => c.SpawnerId == spawner.Owner.Id) < spawner.MaxEntities)
                {
                    Entity toCopy = Owner.Entities[spawner.Blueprint];
                    if(toCopy != null)
                    {
                        Entity clone = TagIOUtils.CloneEntity(toCopy);
                        if(clone.GetComponent<Spatial>() is Spatial sp && spawner.Owner.GetComponent<Spatial>() is Spatial spawnerSp)
                        {
                            sp.Position = spawnerSp.Position;
                        }
                        clone.Active = true;
                        clone.Components.Add(new Spawned() { SpawnerId = spawner.Owner.Id });
                        Owner.Entities.Add(clone);
                    }
                }
            }
        }
    }
}
