using System;

using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Interfaces.Input;
using EntityComponentSystem.Util;
using GameInterfaces.Input;

using WooferGame.Input;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual.Particles;

namespace WooferGame.Systems.Movement
{
    [ComponentSystem("player_controller", ProcessingCycles.Input | ProcessingCycles.Update),
        Watching(typeof(PlayerMovementComponent)),
        Listening(typeof(RigidCollisionEvent))]
    class PlayerMovement : ComponentSystem
    {
        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            foreach(PlayerMovementComponent pmc in WatchedComponents)
            {
                if (!pmc.Owner.Active) continue;
                if ((pmc.Owner.Components.Get<Health>()?.InvincibilityTimer ?? 0) > 40)
                {
                    continue;
                }
                if ((pmc.Owner.Components.Get<Health>()?.CurrentHealth ?? 1) <= 0) continue;

                Physical phys = pmc.Owner.Components.Get<Physical>();
                if (phys == null) continue;

                ButtonInput jumpButton = inputMap.Jump;

                //Jump logic
                if (pmc.OnGround)
                {
                    if(jumpButton.Consume())
                    {
                        phys.Velocity.Y = pmc.JumpSpeed;
                        Owner.Events.InvokeEvent(new PlayerJumpEvent(pmc));
                    }
                }

                //Walking logic
                double xMovement = inputMap.Movement.X * pmc.CurrentSpeed;
                double xMovementCap = pmc.CurrentMaxSpeed;
                if(xMovement > 0)
                {
                    if(phys.Velocity.X <= xMovementCap)
                    {
                        phys.Velocity.X = Math.Min(phys.Velocity.X + xMovement, xMovementCap);
                    }
                } else if(xMovement < 0)
                {
                    if (phys.Velocity.X >= -xMovementCap)
                    {
                        phys.Velocity.X = Math.Max(phys.Velocity.X + xMovement, -xMovementCap);
                    }
                }

                //For smoothly walking down slopes
                if(phys.Velocity.Y <= 0)
                {
                    //Raycast down under the player's feet up to a distance of 4 pixels
                    RaycastEvent raycast = new RaycastEvent(pmc, new FreeVector2D(phys.Position, phys.Position - 8 * Vector2D.UnitJ));
                    Owner.Events.InvokeEvent(raycast);
                    if(raycast.Intersected != null)
                    {
                        foreach(RaycastIntersection intersection in raycast.Intersected)
                        {
                            if(intersection.Component.Owner != pmc.Owner)
                            {
                                if(intersection.FaceProperties.Snap) //If the intersected face has the 'snap' property enabled (for slopes)
                                {
                                    phys.Position = intersection.Point; //Move the player down to the point of intersection, assuming the player's origin is at their feet
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void EventFired(object sender, Event re)
        {
            if(re is RigidCollisionEvent)
            {
                RigidCollisionEvent e = re as RigidCollisionEvent;

                if(e.Victim.Components.Has<PlayerMovementComponent>() && e.Normal.Y > 0)
                {
                    PlayerMovementComponent pmc = e.Victim.Components.Get<PlayerMovementComponent>();
                    pmc.OnGround = true;
                }
            }
        }

        public override void Update()
        {
            foreach(PlayerMovementComponent pmc in WatchedComponents)
            {
                if (!pmc.Owner.Active) continue;
                pmc.OnGround = false;
            }
        }
    }
}
