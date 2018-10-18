using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;

namespace EntityComponentSystem.Events
{
    public class EventManager
    {
        private event EventHandler<Event> Dummy = new EventHandler<Event>(DummyEvent);

        public readonly Dictionary<string, EventHandler<Event>> EventDictionary = new Dictionary<string, EventHandler<Event>>();

        public EventHandler<Event> this[string name] => EventDictionary[name];

        public EventManager()
        {
        }

        public void RegisterEventType(string type)
        {
            if (!EventDictionary.ContainsKey(type)) EventDictionary.Add(type, Dummy);
        }

        public void InvokeEvent(Event evt)
        {
            RegisterEventType(Event.IdentifierOf(evt.GetType()));
            EventDictionary[evt.EventName].Invoke(this, evt);
        }

        private static void DummyEvent(object sender, Event evt) { }
    }
}
