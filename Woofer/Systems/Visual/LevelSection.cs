using System;
using System.Collections.Generic;

using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor.Systems.HelperComponents;
using WooferGame.Scenes;
using WooferGame.Systems.Physics;

namespace WooferGame.Systems.Visual
{
    class LevelSection : Entity
    {
        private List<CollisionBox> collision = new List<CollisionBox>();
        private List<Entity> queuedEntities = new List<Entity>();

        public LevelSection(string texture, Vector2D pos, Rectangle bounds)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Renderable(texture, bounds));
            Components.Add(new LevelRenderable());
            Components.Add(new NoEditorHoverSelect());
        }

        protected void AddCollision(CollisionBox box)
        {
            AddCollision(CoordinateMode.Real, box);
        }

        protected void AddCollision(CoordinateMode mode, CollisionBox box)
        {
            if(mode == CoordinateMode.Grid)
            {
                box.X *= 16;
                box.Y *= 16;
                box.Width *= 16;
                box.Height *= 16;
            }
            collision.Add(box);
        }

        protected void QueueEntity(Entity entity)
        {
            queuedEntities.Add(entity);
        }

        protected void FinalizeCollision()
        {
            Components.Add(new Physical() { GravityMultiplier = 0 });
            Components.Add(new RigidBody(collision.ToArray()));
            collision.Clear();
            collision = null;
        }

        public override void Initialize()
        {
            foreach(Entity entity in queuedEntities)
            {
                if(entity.Components.Get<Spatial>() is Spatial sp)
                {
                    sp.Position += this.Components.Get<Spatial>().Position;
                }
                Owner.Entities.Add(entity);
            }
            queuedEntities.Clear();
            queuedEntities = null;
        }

        protected void AddSegment(LabRoomBuilder rb, Rectangle rect)
        {
            rb.Fill(rect, true);
            this.AddCollision(CoordinateMode.Grid, new CollisionBox(rect.X, rect.Y, rect.Width, rect.Height));
        }
    }

    enum CoordinateMode
    {
        Real,
        Grid
    }
}
