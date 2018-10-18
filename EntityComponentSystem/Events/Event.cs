using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Events
{
    public class Event
    {
        public string EventName { get; private set; }
        public Component Sender { get; }

        private static readonly Dictionary<Type, string> identifierCache = new Dictionary<Type, string>();

        public Event(Component sender)
        {
            Sender = sender;

            try
            {
                this.EventName = IdentifierOf(this.GetType());
            }
            catch (Exception ex)
            {
                throw new AttributeException(
                    $"Required attribute of class 'EventAttribute' not found in derived class '{GetType()}'", ex);
            }
        }

        public static string IdentifierOf<T>() where T : Event
        {
            return IdentifierOf(typeof(T));
        }

        public static string IdentifierOf(Type type)
        {
            if (!identifierCache.ContainsKey(type)) identifierCache[type] = (type.GetCustomAttributes(typeof(EventAttribute), false).First() as EventAttribute).EventName;
            return identifierCache[type];
        }
    }

    public sealed class EventAttribute : Attribute
    {
        public readonly string EventName;

        public EventAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}
