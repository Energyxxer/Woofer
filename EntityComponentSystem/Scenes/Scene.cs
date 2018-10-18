using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;

namespace EntityComponentSystem.Scenes
{
    /// <summary>
    /// Declares a collection of entities and systems that should act together in a level
    /// </summary>
    public class Scene : IDisposable
    {
        /// <summary>
        /// The IGameController this scene belongs to
        /// </summary>
        public readonly IGameController Controller;

        /// <summary>
        /// The name of this scene. Used for cross-scene references and the level editor
        /// </summary>
        public string Name { get; set; } = "Scene";

        /// <summary>
        /// This scene's entity manager
        /// </summary>
        public EntityMap Entities;
        /// <summary>
        /// This scene's system manager
        /// </summary>
        public SystemMap Systems;
        /// <summary>
        /// This scene's event manager
        /// </summary>
        public EventManager Events;

        /// <summary>
        /// The scene's current camera settings
        /// </summary>
        public CameraView CurrentViewport { get; set; }

        /// <summary>
        /// The amount of seconds elapsed since the execution of the previous tick
        /// </summary>
        public float DeltaTime { get; private set; } = 0;
        /// <summary>
        /// The random number generator for this scene
        /// </summary>
        public Random Random { get; set; } = new Random();

        /// <summary>
        /// The fixed amount of seconds each timing-sensitive system should use per update
        /// </summary>
        public float FixedDeltaTime = 1 / 60f;

        /// <summary>
        /// Creates and initializes a scene for the given controller
        /// </summary>
        /// <param name="controller"></param>
        public Scene(IGameController controller)
        {
            Controller = controller;

            Entities = new EntityMap(this);
            Systems = new SystemMap(this);
            Events = new EventManager();

            Entities.Changed += NotifyEntityChange;
            CurrentViewport = new CameraView();
        }

        /// <summary>
        /// Called when the scene should draw itself
        /// </summary>
        /// <typeparam name="TSurface">The screen renderer's Surface type</typeparam>
        /// <typeparam name="TSource">The screen renderer's Source type</typeparam>
        /// <param name="screenRenderer">The screen renderer onto which to draw the scene</param>
        public void Draw<TSurface, TSource>(ScreenRenderer<TSurface, TSource> screenRenderer)
        {
            Systems.Flush();
            Systems.InvokeRender(screenRenderer);
        }

        /// <summary>
        /// Called once every frame. Can be optionally overriden by a subclass of scene.
        /// </summary>
        protected virtual void Update()
        {

        }

        /// <summary>
        /// Called once every frame
        /// </summary>
        /// <param name="timeSpan">The time since the last call to this method</param>
        public void InvokeUpdate(float deltaTime)
        {
            DeltaTime = deltaTime;
            Entities.Flush();
            Systems.Flush();

            Systems.InvokeUpdate();

            Update();

            while(AfterTick.Count > 0)
            {
                AfterTick[0]();
                AfterTick.RemoveAt(0);
            }
        }

        /// <summary>
        /// Updates all systems' watched component list to reflect an addition or a removal of an entity
        /// </summary>
        /// <param name="e">The event detailing what entity changed</param>
        private void NotifyEntityChange(EntityChangedEventArgs e)
        {
            foreach(Component component in e.Entity.Components)
            {
                NotifyComponentChange(new ComponentChangedEventArgs(component, e.WasRemoved));
            }
            if(e.WasRemoved)
            {
                e.Entity.Components.Changed -= NotifyComponentChange;
            } else
            {
                e.Entity.Components.Changed += NotifyComponentChange;
            }
        }

        protected List<Action> AfterTick = new List<Action>();

        public void QueueAction(Action action)
        {
            AfterTick.Add(action);
        }

        /// <summary>
        /// Updates all systems' watched component list to reflect an addition or a removal of a component
        /// </summary>
        /// <param name="e">The event detailing what component changed</param>
        private void NotifyComponentChange(ComponentChangedEventArgs e)
        {
            foreach (ComponentSystem system in Systems)
            {
                if (system.Watching.Contains(e.Component.ComponentName))
                {
                    if (e.WasRemoved) system.RemoveWatchedComponent(e.Component);
                    else system.AddWatchedComponent(e.Component);
                }
            }
        }

        /// <summary>
        /// Called when input information should be processed.
        /// Delegates the call to all inner systems that are marked for input processing
        /// </summary>
        public virtual void InvokeInput()
        {
            Systems.Flush();
            Systems.InvokeInput();
        }

        /// <summary>
        /// Clears all event listeners, entities and systems in this scene to prepare it for disposal
        /// </summary>
        public void Destroy()
        {
            List<string> systemNames = Systems.Select(s => s.SystemName).ToList();
            foreach(string systemName in systemNames)
            {
                Systems.Remove(systemName);
            }
            foreach(Entity entity in Entities)
            {
                entity.Remove();
            }
            Entities.Flush();
            Entities.Changed -= NotifyEntityChange;
        }

        #region IDisposable Support
        public bool Disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Destroy();
                }
                
                Entities = null;
                Systems = null;
                Events = null;

                Disposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
