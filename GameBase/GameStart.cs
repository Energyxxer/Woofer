using System;

using GameInterfaces.Controller;

namespace GameBase
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class GameStart
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Start(IGameController controller)
        {
            using (var game = new Game1(controller))
                game.Run();
        }
    }
}
