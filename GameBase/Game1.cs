using System;
using GameBase.MonoGameAudio;
using GameBase.MonoGameGraphics;
using GameBase.MonoGameGraphicsImplementation;
using GameBase.MonoGameInput;
using GameInterfaces.Controller;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameBase
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        IGameController gameController;

        MonoGameScreenRenderer screenRenderer;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public Game1(IGameController gameController)
        {
            this.gameController = gameController;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //IsFixedTimeStep = false;

            graphics.PreferredBackBufferWidth = gameController.RenderingUnit.ScreenSize.Width;
            graphics.PreferredBackBufferHeight = gameController.RenderingUnit.ScreenSize.Height;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;

            gameController.InputUnit = new MonoGameInputUnit();
            gameController.AudioUnit = new MonoGameAudioUnit(Content);

            gameController.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.screenRenderer = new MonoGameScreenRenderer(
                new MonoGameGraphicsContext(graphics, GraphicsDevice, spriteBatch),
                new MonoGameSpriteManager(graphics, GraphicsDevice, spriteBatch, Content),
                gameController.RenderingUnit);

            gameController.RenderingUnit.LoadContent(this.screenRenderer);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private TimeSpan lastUpdate;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gameController.Input();

            /*if(!stop && GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
            {
                //Console.WriteLine(GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.2f));
                Console.WriteLine(GamePad.GetCapabilities(PlayerIndex.One).);
            }*/

            // TODO: Add your update logic here
            gameController.Update(gameTime.TotalGameTime - lastUpdate, gameTime.ElapsedGameTime);
            lastUpdate = gameTime.TotalGameTime;
            /*System.Console.WriteLine(gameTime.TotalGameTime);
            System.Console.WriteLine(gameTime.ElapsedGameTime);
            System.Console.WriteLine();*/

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null);
            GraphicsDevice.Clear(Color.White);
            spriteBatch.End();

            screenRenderer.Begin();
            gameController.Draw(screenRenderer);
            screenRenderer.End();

            base.Draw(gameTime);
        }
    }
}
