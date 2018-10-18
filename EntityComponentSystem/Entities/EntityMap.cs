using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util.Generics;

namespace EntityComponentSystem.Entities
{
    public class EntityMap : IEnumerable<Entity>
    {
        private readonly Scene Owner;
        private readonly OrderedDictionary<long, Entity> _dict = new OrderedDictionary<long, Entity>();
        private readonly Dictionary<long, Entity> _scheduledAdd = new Dictionary<long, Entity>();
        private readonly List<long> _scheduledRemove = new List<long>();

        public EntityMap(Scene owner)
        {
            Owner = owner;
        }

        public void Flush()
        {
            foreach (KeyValuePair<long, Entity> item in _scheduledAdd)
            {
                _dict.Add(item.Key, item.Value);
                Changed.Invoke(new EntityChangedEventArgs(item.Value, false));
            }
            _scheduledAdd.Clear();
            foreach (long id in _scheduledRemove)
            {
                if (!_dict.ContainsKey(id)) continue;
                Changed.Invoke(new EntityChangedEventArgs(_dict[id], true));
                _dict.Remove(id);
            }
            _scheduledRemove.Clear();
        }

        public long GetNewId()
        {
            long id;
            do
            {
                id = ((long)Owner.Random.Next() << 32) + Owner.Random.Next();
            } while (_dict.ContainsKey(id) || _scheduledAdd.ContainsKey(id));
            return id;
        }

        public event EntityChangedEventHandler Changed = Dummy;

        public Entity this[long key] { get => _dict.ContainsKey(key) ? _dict[key] : null; }

        public int Count => _dict.Count;

        public void EagerAssignId(Entity entity)
        {
            if(!entity._is_id_set)
            {
                entity._id = GetNewId();
                entity._is_id_set = true;
            }
        }

        public void Add(Entity entity)
        {
            EagerAssignId(entity);
            _scheduledAdd[entity.Id] = entity;
            entity.Owner = Owner;
            entity.Initialize();
        }

        public void AddRange(IEnumerable<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                Add(entity);
            }
        }

        public void SwapIndices(int index0, int index1)
        {
            Entity old0 = _dict.ElementAt(index0).Value;
            _dict.Remove(old0.Id);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool ContainsId(long id)
        {
            return _dict.ContainsKey(id);
        }

        public bool Contains(Entity entity)
        {
            return ContainsId(entity.Id);
        }

        public bool Remove(long id)
        {
            if (ContainsId(id))
            {
                _scheduledRemove.Add(id);
                return true;
            }
            return false;
        }

        public bool Remove(Entity entity)
        {
            if (Contains(entity))
            {
                _scheduledRemove.Add(entity.Id);
                return true;
            }
            return false;
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return _dict.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        private static void Dummy(EntityChangedEventArgs e) { }
    }

    public delegate void EntityChangedEventHandler(EntityChangedEventArgs e);

    public class EntityChangedEventArgs : EventArgs
    {
        public readonly Entity Entity;
        public bool WasRemoved { get; private set; }

        public EntityChangedEventArgs(Entity entity, bool removed)
        {
            this.Entity = entity;
            this.WasRemoved = removed;
        }
    }
}
