using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Scenes;

namespace EntityComponentSystem.Entities
{
    /// <summary>
    /// A bundle of components marked with a unique identifier
    /// </summary>
    public class Entity : IEquatable<Entity>
    {
        /// <summary>
        /// The name for this entity, to list in the editor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The scene this entity belongs to. May be null until properly added to the scene
        /// </summary>
        public Scene Owner { get; set; }

        /// <summary>
        /// The internal ID field
        /// </summary>
        internal long _id;
        /// <summary>
        /// Whether this entity's ID has been set by the scene's entity manager after being added
        /// </summary>
        internal bool _is_id_set = false;
        /// <summary>
        /// This entity's unique identifier. If attempted to retrieve before the ID is set, an InvalidOperationException is thrown
        /// </summary>
        public long Id => (_is_id_set ? _id : throw new InvalidOperationException("Id is still not assigned"));
        /// <summary>
        /// This entity's component map
        /// </summary>
        public ComponentMap Components { get; }
        /// <summary>
        /// Whether this entity is marked as currently active. The component systems will determine what 'active' means.
        /// If an entity is inactive, it usually means it exists but doesn't interact with the world
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Creates an entity
        /// </summary>
        public Entity()
        {
            this.Components = new ComponentMap(this);
            this.Name = this.GetType().Name;
        }

        /// <summary>
        /// Marks this entity for removal, making it inactive and scheduling it for removal from the scene
        /// </summary>
        public void Remove()
        {
            Active = false;
            Owner.Entities.Remove(Id);
        }

        /// <summary>
        /// Runs once this entity's Owner and Id fields are set
        /// </summary>
        public virtual void Initialize()
        {

        }

        public T GetComponent<T>() where T : Component => Components.Get<T>();
        public bool HasComponent<T>() where T : Component => Components.Has<T>();

        public override bool Equals(object obj) => Equals(obj as Entity);
        public bool Equals(Entity other) => other != null && _id == other._id;
        public override int GetHashCode() => 1969571243 + _id.GetHashCode();
    }
}
