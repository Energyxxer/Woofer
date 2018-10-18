using System;
using EntityComponentSystem.Commands;
using EntityComponentSystem.Scenes;

namespace GameInterfaces.Controller
{
    /// <summary>
    /// Declares a master interface consisting of the data needed for a game to function
    /// </summary>
    public interface IGameController
    {
        /// <summary>
        /// The scene that is currently being played
        /// </summary>
        Scene ActiveScene { get; }
        /// <summary>
        /// This Controller's rendering unit
        /// </summary>
        IRenderingUnit RenderingUnit { get; }
        /// <summary>
        /// This Controller's input unit
        /// </summary>
        IInputUnit InputUnit { get; set; }
        /// <summary>
        /// This Controller's audio unit
        /// </summary>
        IAudioUnit AudioUnit { get; set; }
        /// <summary>
        /// Whether this game is currently paused
        /// </summary>
        bool Paused { get; set; }

        /// <summary>
        /// Runs once when the game starts
        /// </summary>
        void Initialize();
        /// <summary>
        /// Runs a fixed number of times per second
        /// </summary>
        /// <param name="timeSpan">The time since the last call to this method</param>
        /// <param name="elapsedGameTime">The time since the game started running</param>
        void Update(TimeSpan timeSpan, TimeSpan elapsedGameTime);
        /// <summary>
        /// Runs a fixed number of times when input information should be processed
        /// </summary>
        void Input();

        /// <summary>
        /// Executes an action based on a given command
        /// </summary>
        /// <param name="command">The command this game controller should execute</param>
        void CommandFired(Command command);
        /// <summary>
        /// Called when the game should draw itself
        /// </summary>
        /// <typeparam name="TSurface">The screen renderer's Surface type</typeparam>
        /// <typeparam name="TSource">The screen renderer's Source type</typeparam>
        /// <param name="screenRenderer">The screen renderer onto which to draw the game</param>
        void Draw<TSurface, TSource>(ScreenRenderer<TSurface, TSource> screenRenderer);
    }
}
