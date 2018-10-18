using System;
using System.Collections.Generic;
using System.Linq;

using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using WooferGame.Systems.Enemies.Boss;
using WooferGame.Systems.Player;
using WooferGame.Systems.Player.Actions;

namespace WooferGame.Systems.Physics
{
    [ComponentSystem("physics", ProcessingCycles.Update),
        Watching(typeof(Physical), typeof(RigidBody), typeof(SoftBody)),
        Listening(typeof(RaycastEvent), typeof(SoftCollisionEvent), typeof(RigidCollisionEvent))]
    public class PhysicsSystem : ComponentSystem
    {
        private Vector2D Gravity = new Vector2D(0, -362);

        private float accumulator = 0.0f;

        private List<Component> sweeper = new List<Component>();
        private List<SingleCollision> collisions = new List<SingleCollision>();

        public override void Update()
        {
            if (Owner.FixedDeltaTime == 0) return;
            accumulator += Owner.DeltaTime;
            
            int timesExecuted = 0;
            
            while (accumulator >= Owner.FixedDeltaTime)
            {
                timesExecuted++;
                accumulator -= Owner.FixedDeltaTime;
                foreach (Physical ph in WatchedComponents.Where(c => c is Physical))
                {
                    if (!ph.Owner.Active) continue;
                    ph.PreviousPosition = ph.Position;

                    ph.Velocity += Gravity * ph.GravityMultiplier * Owner.FixedDeltaTime;
                    ph.Position += ph.Velocity * Owner.FixedDeltaTime;

                    ph.Position = ph.Position.GetRounded();
                    ph.Velocity = ph.Velocity.GetRounded();

                    ph.PreviousVelocity = ph.Velocity;
                }
                
                WatchedComponents = WatchedComponents.OrderBy(a => GetCrossTickLeft(a)).ToList();

                //Handle collision
                foreach (Component c0 in WatchedComponents)
                {
                    if (c0 is Physical) continue;
                    if (!c0.Owner.Active) continue;
                    double x = GetCrossTickLeft(c0);

                    while (sweeper.Count > 0 && !IntersectsX(sweeper[0], x))
                    {
                        sweeper.RemoveAt(0);
                    }

                    foreach(Component c1 in sweeper)
                    {
                        if (c0 is RigidBody && c1 is RigidBody) continue;

                        SoftBody objA = c0 is SoftBody ? (SoftBody)c0 : (SoftBody)c1;
                        Component objB = objA == c0 ? c1 : c0;

                        if (objA.PassThroughLevel && objB is RigidBody) continue;

                        Physical physA = objA.Owner.Components.Get<Physical>();
                        Physical physB = objB.Owner.Components.Get<Physical>();

                        if (physA == null || physB == null) continue;

                        if (objB is RigidBody rb && !rb.UnionBounds.Offset(physB.Position).IntersectsWith(objA.Bounds.Offset(physA.Position))) continue;

                        IEnumerable<CollisionBox> solidBoxes =
                            objB is SoftBody ?
                                new CollisionBox[] { (objB as SoftBody).Bounds.Offset(physB.Position) } :
                                (objB as RigidBody).Bounds.Select(b => b.Offset(physB.Position));

                        foreach(CollisionBox otherBounds in solidBoxes)
                        {
                            CollisionBox intersection = objA.Bounds.Offset(physA.Position).Intersect(otherBounds);
                            if(intersection != null)
                            {
                                collisions.Add(new SingleCollision(objA, objB, otherBounds, intersection));
                            }
                        }
                    }

                    foreach(SingleCollision collision in collisions.OrderByDescending(c => c.intersection.Area))
                    {
                        SoftBody objA = collision.objA;
                        Component objB = collision.objB;

                        Physical physA = objA.Owner.Components.Get<Physical>();
                        Physical physB = objB.Owner.Components.Get<Physical>();

                        CollisionBox otherBounds = collision.box;
                        CollisionBox intersection = objA.Bounds.Offset(physA.Position).Intersect(otherBounds);

                        if (intersection == null) continue;

                        if (objB is RigidBody) //Hard collision
                        {
                            Vector2D totalVelocity = physA.PreviousVelocity - physB.Velocity;

                            List<FreeVector2D> sides = intersection.GetSides().Where(s => BelongsToBox(s, otherBounds)).Where(s => GeneralUtil.SubtractAngles(s.Normal.Angle, totalVelocity.Angle) > Math.PI / 2).ToList();

                            if (sides.Count == 0) //It's "stuck" inside a tile or that tile doesn't have one of the sides enabled
                            {
                                continue;
                            }

                            FreeVector2D normalSide = sides[0];
                            Vector2D normal = sides[0].Normal;
                            if (normal == Vector2D.Empty) continue;

                            if (sides.Count > 1)
                            {
                                if (IsAheadOfNormal(objA.Bounds.Offset(physA.PreviousPosition), sides[0]))
                                {
                                    if (IsAheadOfNormal(objA.Bounds.Offset(physA.PreviousPosition), sides[1]))
                                    {
                                        normalSide = sides[0].Normal.X == 0 ? sides[0] : sides[1];
                                    }
                                    else normalSide = sides[0];
                                }
                                else normalSide = sides[1];

                                normal = normalSide.Normal;
                            }
                            CollisionFaceProperties faceProperties = intersection.GetFaceProperties(normal);

                            if (objA.Movable)
                            {
                                if (normal.X == 0)
                                {
                                    double displacement = Math.Abs(normalSide.A.Y - (normal.Y > 0 ? objA.Bounds.Offset(physA.Position).Bottom : objA.Bounds.Offset(physA.Position).Top));
                                    if (faceProperties.Snap || Math.Round(displacement, 8) <= Math.Round(Math.Abs(physA.Position.Y - physA.PreviousPosition.Y) + Math.Abs(physB.Position.Y - physB.PreviousPosition.Y), 8))
                                    {
                                        physA.Position += new Vector2D(0, displacement) * normal.Y;
                                        physA.Velocity = new Vector2D(physA.Velocity.X * (1 - faceProperties.Friction), 0) + physB.Velocity * faceProperties.Friction;
                                    }
                                    else continue;
                                }
                                else
                                {
                                    double displacement = Math.Abs(normalSide.A.X - (normal.X > 0 ? objA.Bounds.Offset(physA.Position).Left : objA.Bounds.Offset(physA.Position).Right));
                                    if (faceProperties.Snap || Math.Round(displacement, 8) <= Math.Round(Math.Abs(physA.Position.X - physA.PreviousPosition.X) + Math.Abs(physB.Position.X - physB.PreviousPosition.X), 8))
                                    {
                                        physA.Position += new Vector2D(displacement, 0) * normal.X;
                                        physA.Velocity = new Vector2D(0, physA.Velocity.Y * (1 - faceProperties.Friction)) + physB.Velocity * faceProperties.Friction;
                                    }
                                    else continue;
                                }
                            }

                            Owner.Events.InvokeEvent(new RigidCollisionEvent(objA, objB.Owner, normal));
                            Owner.Events.InvokeEvent(new RigidCollisionEvent(objB, objA.Owner, normal));
                        }
                        else //Soft Collision
                        {
                            Owner.Events.InvokeEvent(new SoftCollisionEvent(objA, objB.Owner));
                            Owner.Events.InvokeEvent(new SoftCollisionEvent(objB, objA.Owner));

                            Vector2D center0 = objA.Bounds.Offset(physA.Position).Center;
                            Vector2D center1 = otherBounds.Center;

                            double distance = (center0 - center1).Magnitude;
                            if (distance <= 1e-4) continue;

                            double force = 2 * Owner.FixedDeltaTime * intersection.Area * (objA.Mass * (objB as SoftBody).Mass);

                            Vector2D forceVec = (center1 - center0).Normalize() * force;
                            forceVec.Y = 0;

                            if (objA.Movable && objA.Mass != 0) physA.Velocity -= forceVec / (objA.Mass);
                            if ((objB as SoftBody).Movable && (objB as SoftBody).Mass != 0) physB.Velocity -= -forceVec / (objB as SoftBody).Mass;
                        }
                    }

                    collisions.Clear();

                    sweeper.Add(c0);
                }
            }
            
            sweeper.Clear();
            collisions.Clear();
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is RaycastEvent re)
            {
                //Initialize list of intersections
                if (re.Intersected == null) re.Intersected = new List<RaycastIntersection>();
                
                //Filter all soft and rigid bodies whose X axis intersects the given ray.
                List<Component> intersectingX = WatchedComponents.Where(c => !(c is Physical) && c.Owner.Active && (IntersectsX(c, re.Ray.A.X, re.Ray.B.X))).ToList();

                foreach(Component c in intersectingX)
                {
                    Physical phys = c.Owner.Components.Get<Physical>();
                    Spatial sp = c.Owner.Components.Get<Spatial>();

                    IEnumerable<CollisionBox> solidBoxes =
                            c is SoftBody ?
                                new CollisionBox[] { (c as SoftBody).Bounds.Offset(phys?.Position ?? sp?.Position ?? Vector2D.Empty) } :
                                (c as RigidBody).Bounds.Select(b => b.Offset(phys?.Position ?? sp?.Position ?? Vector2D.Empty));

                    foreach(CollisionBox box in solidBoxes)
                    {
                        List<FreeVector2D> sides = box.GetSides();
                        foreach(FreeVector2D side in sides)
                        {
                            Vector2D? intersection = re.Ray.GetIntersection(side);
                            if(intersection.HasValue)
                            {
                                re.Intersected.Add(new RaycastIntersection(intersection.Value, side, box, c, box.GetFaceProperties(side.Normal)));
                            }
                        }
                    }
                }
            }
            else if((e is RigidCollisionEvent || e is SoftCollisionEvent) && e.Sender.Owner.Components.Get<RemoveOnCollision>() is RemoveOnCollision remove)
            {
                if((remove.RemoveOnRigid && e is RigidCollisionEvent) || (remove.RemoveOnSoft && e is SoftCollisionEvent))
                {
                    e.Sender.Owner.Remove();
                }
            }
        }

        public static bool IsAheadOfNormal(CollisionBox box, FreeVector2D side)
        {
            Vector2D normal = side.Normal;
            if(normal.Y == 0)
            {
                return normal.X > 0 ? box.Left >= side.A.X : box.Right <= side.A.X;
            } else
            {
                return normal.Y > 0 ? box.Bottom >= side.A.Y : box.Top <= side.A.Y;
            }
        }

        public static bool BelongsToBox(FreeVector2D side, CollisionBox realBounds)
        {
            if(side.A.Y == side.B.Y) //Is horizontal
            {
                return side.A.Y == realBounds.Bottom || side.A.Y == realBounds.Top;
            }
            else if(side.A.X == side.B.X) //Is vertical
            {
                return side.A.X == realBounds.Left || side.B.X == realBounds.Right;
            }
            throw new ArgumentException("Side given is not horizontal nor vertical");
        }

        private bool IntersectsX(Component c, double x)
        {
            return GetCrossTickLeft(c) <= x && x <= GetCrossTickRight(c);
        }

        private bool IntersectsX(Component c, double x0, double x1)
        {
            if(x0 > x1)
            {
                double temp = x0;
                x0 = x1;
                x1 = temp;
            }

            double left = GetCrossTickLeft(c);
            double right = GetCrossTickRight(c);

            return left < x1 && right > x0;
        }

        private double GetCrossTickLeft(Component c)
        {
            if (c is Physical ph) return Math.Min(ph.Position.X, ph.PreviousPosition.X);
            ph = c.Owner.Components.Get<Physical>();
            SoftBody sb = c as SoftBody;
            Spatial sp = c.Owner.GetComponent<Spatial>();

            CollisionBox box = (c is RigidBody rb) ? rb.UnionBounds : sb?.Bounds;

            return Math.Min(
                box.Offset(ph?.Position ?? sp?.Position ?? Vector2D.Empty).Left,
                box.Offset(ph?.PreviousPosition ?? sp?.Position ?? Vector2D.Empty).Left
            );
        }

        private double GetCrossTickRight(Component c)
        {
            if (c is Physical ph) return Math.Min(ph.Position.X, ph.PreviousPosition.X);
            ph = c.Owner.Components.Get<Physical>();
            SoftBody sb = c as SoftBody;
            Spatial sp = c.Owner.GetComponent<Spatial>();

            CollisionBox box = (c is RigidBody rb) ? rb.UnionBounds : sb?.Bounds;

            return Math.Max(
                box.Offset(ph?.Position ?? sp?.Position ?? Vector2D.Empty).Right,
                box.Offset(ph?.PreviousPosition ?? sp?.Position ?? Vector2D.Empty).Right
            );
        }
    }

    internal struct SingleCollision
    {
        internal SoftBody objA;
        internal Component objB;
        internal CollisionBox box;
        internal CollisionBox intersection;

        public SingleCollision(SoftBody objA, Component objB, CollisionBox box, CollisionBox intersection)
        {
            this.objA = objA;
            this.objB = objB;
            this.box = box;
            this.intersection = intersection;
        }

        public override string ToString() => $"SingleCollision[objA={objA.Owner},objB={objB.Owner},box={box},intersection={intersection},intersection.Area={intersection.Area}]";
    }
}
