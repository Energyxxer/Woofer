using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [Event("EntitySelectEvent")]
    class EntitySelectEvent : Event
    {
        public Entity Entity;

        public EntitySelectEvent(Entity entity, Component sender) : base(sender)
        {
            this.Entity = entity;
        }
    }
}
