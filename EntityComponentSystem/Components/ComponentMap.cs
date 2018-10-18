using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Entities;

namespace EntityComponentSystem.Components
{
    public class ComponentMap : IEnumerable<Component>
    {
        private readonly Entity _owner;
        private readonly Dictionary<string, Component> _dict = new Dictionary<string, Component>();

        public event ComponentChangedEventHandler Changed = Dummy;

        public Component this[string name] => _dict.ContainsKey(name) ? _dict[name] : null;

        public ComponentMap(Entity owner)
        {
            _owner = owner;
        }

        public void Add(Component component)
        {
            if(!_dict.ContainsKey(component.ComponentName))
            {
                _dict.Add(component.ComponentName, component);
                component.Owner = _owner;
                Changed.Invoke(new ComponentChangedEventArgs(component, false));
            }
        }
        public void Remove<T>() where T : Component
        {
            Remove(Component.IdentifierOf<T>());
        }

        public void Remove(string identifier)
        {
            if (_dict.ContainsKey(identifier))
            {
                Changed.Invoke(new ComponentChangedEventArgs(this[identifier], true));
                _dict.Remove(identifier);
            }
        }
        public int Count => _dict.Count;

        public IEnumerator<Component> GetEnumerator() => _dict.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

        public T Get<T>() where T : Component
        {
            string identifier = Component.IdentifierOf<T>();
            return _dict.ContainsKey(identifier) ? _dict[identifier] as T : null;
        }

        public bool Has<T>() where T : Component
        {
            return _dict.ContainsKey(Component.IdentifierOf<T>());
        }

        private static void Dummy(ComponentChangedEventArgs e) { }
    }

    public delegate void ComponentChangedEventHandler(ComponentChangedEventArgs e);

    public class ComponentChangedEventArgs : EventArgs
    {
        public readonly Component Component;
        public bool WasRemoved { get; private set; }

        public ComponentChangedEventArgs(Component component, bool removed)
        {
            Component = component;
            WasRemoved = removed;
        }
    }
}
