using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Components
{
    /// <summary>
    /// A named bundle of data to be used by its respective system(s)
    /// </summary>
    [PersistentObject]
    public abstract class Component
    {
        /// <summary>
        /// The entity this component belongs to. May be null until added to an entity
        /// </summary>
        public Entity Owner { get; set; }
        /// <summary>
        /// The unique identifier for this component type
        /// </summary>
        public string ComponentName { get; private set; }

        /// <summary>
        /// A cache of component type-to-identifier mappings for quick retrieval
        /// </summary>
        private static readonly Dictionary<Type, string> identifierCache = new Dictionary<Type, string>();
        /// <summary>
        /// A cache of component identifier-to-type mappings for quick retrieval
        /// </summary>
        private static Dictionary<string, Type> identifierToTypeMap = null;

        /// <summary>
        /// Creates a component and initializes its values based on the attributes of its class
        /// </summary>
        protected Component()
        {
            try
            {
                this.ComponentName = IdentifierOf(this.GetType());
            }
            catch (Exception ex) {
                throw new AttributeException(
                    $"Required attribute of class 'ComponentAttribute' not found in derived class '{GetType()}'", ex);
            }
        }

        /// <summary>
        /// Retrieves the identifier of the given component type
        /// </summary>
        /// <typeparam name="T">The component type of which to retrieve the identifier</typeparam>
        /// <returns>The identifier for the given type, if it exists</returns>
        public static string IdentifierOf<T>() where T : Component
        {
            return IdentifierOf(typeof(T));
        }

        /// <summary>
        /// Retrieves the identifier of the given component type
        /// </summary>
        /// <param name="T">The component type of which to retrieve the identifier</param>
        /// <returns>The identifier for the given type, if it exists</returns>
        public static string IdentifierOf(Type type)
        {
            if (!identifierCache.ContainsKey(type)) identifierCache[type] = (type.GetCustomAttributes(typeof(ComponentAttribute), false).First() as ComponentAttribute).ComponentName;
            return identifierCache[type];
        }

        /// <summary>
        /// Searches the given assembly and caches all the component types and their annotated identifiers
        /// </summary>
        /// <param name="assembly">The assembly to search for components</param>
        public static void PopulateDictionaryWithAssembly(Assembly assembly)
        {
            if (identifierToTypeMap == null) identifierToTypeMap = new Dictionary<string, Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(Component).IsAssignableFrom(type))
                {
                    object[] attributes;
                    if ((attributes = type.GetCustomAttributes(typeof(ComponentAttribute), false)).Length > 0)
                    {
                        string thisIdentifier = (attributes[0] as ComponentAttribute).ComponentName;
                        identifierToTypeMap[thisIdentifier] = type;
                    }
                }
            }
        }

        /// <summary>
        /// Searches the entry assembly for component types and caches their identifiers
        /// </summary>
        public static void PopulateIdentifiers()
        {
            if (identifierToTypeMap == null)
            {
                PopulateDictionaryWithAssembly(Assembly.GetEntryAssembly());
            }
        }

        /// <summary>
        /// Retrieves the component type for the given identifier
        /// </summary>
        /// <param name="identifier">The identifier from of which to retrieve the type</param>
        /// <returns>The type object for the given identifier, if it exists</returns>
        public static Type TypeForIdentifier(string identifier)
        {
            PopulateIdentifiers();
            return identifierToTypeMap[identifier];
        }

        /// <summary>
        /// Retrieves a list of all component identifiers
        /// </summary>
        /// <returns>A list of all component identifiers</returns>
        public static List<string> GetAllIdentifiers()
        {
            PopulateIdentifiers();
            return identifierToTypeMap.Keys.ToList();
        }
    }

    /// <summary>
    /// The attribute all component types should be marked with, to specify its identifier
    /// </summary>
    public sealed class ComponentAttribute : Attribute
    {
        /// <summary>
        /// The identifier for this component
        /// </summary>
        public readonly string ComponentName;

        /// <summary>
        /// Creates a component attribute for a component with the given identifier
        /// </summary>
        /// <param name="componentName">The identifier of this component</param>
        public ComponentAttribute(string componentName)
        {
            ComponentName = componentName;
        }
    }
}
