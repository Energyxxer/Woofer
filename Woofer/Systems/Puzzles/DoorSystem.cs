using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using WooferGame.Systems.Interaction;

namespace WooferGame.Systems.Puzzles
{
    [ComponentSystem("door_system", ProcessingCycles.Update),
        Watching(typeof(DoorComponent)),
        Listening(typeof(ActivationEvent))]
    class DoorSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach(DoorComponent door in WatchedComponents)
            {
                if(door.OpeningDirection != 0)
                {
                    door.CurrentOpenTime += Owner.DeltaTime;
                    double delta = door.OpeningDirection * ((door.MaxOpenDistance * Math.PI) / (2 * door.OpeningTime)) * Math.Sin(door.CurrentOpenTime * Math.PI / door.OpeningTime) * Owner.DeltaTime;
                    //double delta = door.OpeningDirection * ((2/door.MaxOpenDistance) * door.CurrentOpenTime) * (door.MaxOpenDistance / door.OpeningTime);

                    if(delta > 0 && door.CurrentOpenDistance + delta > door.MaxOpenDistance)
                    {
                        delta = door.MaxOpenDistance - door.CurrentOpenDistance;
                    } else if(delta < 0 && door.CurrentOpenDistance + delta < 0)
                    {
                        delta = -door.CurrentOpenDistance;
                    }

                    if (delta == 0 || door.CurrentOpenTime > door.OpeningTime)
                    {
                        Spatial sp = door.Owner.GetComponent<Spatial>();
                        if (door.OpeningDirection > 0)
                        {
                            door.CurrentOpenDistance = door.MaxOpenDistance;
                            if (sp != null)
                            {
                                sp.Position.Y = door.OpenY;
                            }
                        } else if(door.OpeningDirection < 0)
                        {
                            door.CurrentOpenDistance = 0;

                            if (sp != null)
                            {
                                sp.Position.Y = door.ClosedY;
                            }
                        }
                        door.OpeningDirection = 0;
                    }
                    else if(delta != 0)
                    {
                        Spatial sp = door.Owner.GetComponent<Spatial>();
                        if(sp != null)
                        {
                            sp.Position += new Vector2D(0, delta);
                        }
                        door.CurrentOpenDistance += delta;
                    }
                }
            }
        }
        public override void EventFired(object sender, Event evt)
        {
            if(evt is ActivationEvent ae && ae.Affected.Components.Get<DoorComponent>() is DoorComponent door)
            {
                int multiplier = 1;
                if(door.OpeningDirection == 0)
                {

                    multiplier = (door.CurrentOpenDistance < door.MaxOpenDistance || !door.Toggle) ? 1 : -1;
                    Spatial sp = door.Owner.GetComponent<Spatial>();
                    if (sp != null)
                    {
                        if (door.CurrentOpenDistance == 0)
                        {
                            door.ClosedY = sp.Position.Y;
                            door.OpenY = sp.Position.Y + door.MaxOpenDistance;
                        } else if(door.CurrentOpenDistance == door.MaxOpenDistance)
                        {
                            door.OpenY = sp.Position.Y;
                            door.ClosedY = sp.Position.Y - door.MaxOpenDistance;
                        }
                    }
                } else
                {
                    multiplier = Math.Sign(door.OpeningDirection);
                }
                door.OpeningDirection = multiplier;
                door.CurrentOpenTime = 0;
            }
        }
    }
}
