using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Events;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;

namespace EntityComponentSystem.ComponentSystems
{
    public abstract class ComponentSystem
    {
        private static readonly Dictionary<Type, ComponentSystemAttribute> attributeCache = new Dictionary<Type, ComponentSystemAttribute>();
        private static Dictionary<string, Type> identifierToTypeMap = null;

        public string SystemName { get; private set; }

        private const byte _input = 0b001;
        private const byte _update = 0b010;
        private const byte _render = 0b100;

        public Scene Owner { get; set; }
        public string[] Watching { get; private set; } = new string[0];
        public string[] Listening { get; private set; } = new string[0];
        private readonly ProcessingCycles ProcessingCycles = ProcessingCycles.None;
        private readonly ProcessingFlags ProcessingFlags = ProcessingFlags.None;

        public bool InputProcessing => (ProcessingCycles & ProcessingCycles.Input) != ProcessingCycles.None;
        public bool UpdateProcessing => (ProcessingCycles & ProcessingCycles.Update) != ProcessingCycles.None;
        public bool RenderProcessing => (ProcessingCycles & ProcessingCycles.Render) != ProcessingCycles.None;

        public bool PauseProcessing => (ProcessingFlags & ProcessingFlags.Pause) != ProcessingFlags.None;

        public virtual bool ShouldSave => true;

        protected List<Component> WatchedComponents = new List<Component>();
        
        public ComponentSystem()
        {
            try
            {
                ComponentSystemAttribute attr = AttributeOf(this.GetType());
                this.SystemName = attr.SystemName;
                this.ProcessingCycles = attr.ProcessingCycles;
                this.ProcessingFlags = attr.ProcessingFlags;
                this.Watching = attr.Watching;
                this.Listening = attr.Listening;
            }
            catch (Exception ex)
            {
                throw new AttributeException(
                    $"Required attribute of class 'ComponentSystemAttribute' not found in derived class '{GetType()}'", ex);
            }
        }

        public virtual void RemoveWatchedComponent(Component component)
        {
            WatchedComponents.Remove(component);
        }

        public virtual void AddWatchedComponent(Component component)
        {
            WatchedComponents.Add(component);
        }

        public virtual void EventFired(object sender, Event e)
        {
        }

        public virtual void Input()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
        }

        public static string IdentifierOf<T>() where T : ComponentSystem
        {
            return IdentifierOf(typeof(T));
        }

        public static string IdentifierOf(Type type)
        {
            return AttributeOf(type).SystemName;
        }

        private static ComponentSystemAttribute AttributeOf(Type type)
        {
            if (!attributeCache.ContainsKey(type))
            {
                attributeCache[type] = (type.GetCustomAttributes(typeof(ComponentSystemAttribute), false).First() as ComponentSystemAttribute);
                attributeCache[type].Watching = (type.GetCustomAttributes(typeof(WatchingAttribute), false).FirstOrDefault() as WatchingAttribute)?.Watching ?? new string[0];
                attributeCache[type].Listening = (type.GetCustomAttributes(typeof(ListeningAttribute), false).FirstOrDefault() as ListeningAttribute)?.Listening ?? new string[0];
            }
            return attributeCache[type];
        }

        public static void PopulateDictionaryWithAssembly(Assembly assembly)
        {
            if (identifierToTypeMap == null) identifierToTypeMap = new Dictionary<string, Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ComponentSystem).IsAssignableFrom(type)) {
                    object[] attributes;
                    if ((attributes = type.GetCustomAttributes(typeof(ComponentSystemAttribute), false)).Length > 0)
                    {
                        string thisIdentifier = (attributes[0] as ComponentSystemAttribute).SystemName;
                        identifierToTypeMap[thisIdentifier] = type;
                    }
                }
            }
        }

        public static void PopulateIdentifiers()
        {
            if (identifierToTypeMap == null)
            {
                PopulateDictionaryWithAssembly(Assembly.GetExecutingAssembly());
                PopulateDictionaryWithAssembly(Assembly.GetEntryAssembly());
            }
        }

        public static Type TypeForIdentifier(string identifier)
        {
            PopulateIdentifiers();
            return identifierToTypeMap[identifier];
        }

        public static List<string> GetAllIdentifiers()
        {
            PopulateIdentifiers();
            return identifierToTypeMap.Keys.ToList();
        }
    }

    public sealed class ComponentSystemAttribute : Attribute
    {
        public readonly string SystemName;
        public readonly ProcessingCycles ProcessingCycles;
        public readonly ProcessingFlags ProcessingFlags;
        public string[] Watching;
        public string[] Listening;

        public ComponentSystemAttribute(string systemName) : this(systemName, ProcessingCycles.None)
        {

        }

        public ComponentSystemAttribute(string systemName, ProcessingCycles cycles) : this(systemName, cycles, ProcessingFlags.None)
        {
        }

        public ComponentSystemAttribute(string systemName, ProcessingCycles cycles, ProcessingFlags flags)
        {
            SystemName = systemName;
            ProcessingCycles = cycles;
            ProcessingFlags = flags;
        }
    }

    public sealed class WatchingAttribute : Attribute
    {
        public readonly string[] Watching;
        public WatchingAttribute(params Type[] watching)
        {
            if (!watching.All(t => typeof(Component).IsAssignableFrom(t))) throw new ArgumentException("watching");
            Watching = watching.Select(t => Component.IdentifierOf(t)).ToArray();
        }
    }

    public sealed class ListeningAttribute : Attribute
    {
        public readonly string[] Listening;
        public ListeningAttribute(params Type[] listening)
        {
            if (!listening.All(t => typeof(Event).IsAssignableFrom(t))) throw new ArgumentException("listening");
            Listening = listening.Select(t => Event.IdentifierOf(t)).ToArray();
        }
    }

    [Flags]
    public enum ProcessingCycles
    {
        None = 0,
        Input = 1,
        Update = 2,
        Render = 4,
        All = Input | Update | Render,
    }

    [Flags]
    public enum ProcessingFlags
    {
        None = 0,
        Pause = 1
    }
}
