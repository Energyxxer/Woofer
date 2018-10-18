using System;
using System.Threading;
using EntityComponentSystem.Commands;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Scenes;
using GameInterfaces.Audio;
using GameInterfaces.Controller;
using WooferGame.Controller.Commands;
using WooferGame.Controller.Game;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor;
using WooferGame.Meta.Loading;
using WooferGame.Scenes;
using WooferGame.Scenes.Intro;

namespace WooferGame.Controller
{
    public class WooferController : IGameController
    {
        public Scene ActiveScene { get; internal set; }
        public IRenderingUnit RenderingUnit { get; internal set; }
        public IInputUnit InputUnit { get; set; }
        public IAudioUnit AudioUnit { get; set; }

        public InputMapManager InputManager { get; private set; }
        public bool Paused { get; set; }
        public SaveGame CurrentSave { get; internal set; } = null;

        public double GameSpeed = 1;

        private double Accumulator = 0;

        public WooferController()
        {
            RenderingUnit = new WooferRenderingUnit(this);
        }

        public void Initialize() {
            InputManager = new InputMapManager(this);
            InputManager.Add(new KeyboardInputMap(InputUnit.Keyboard, InputUnit.Mouse));
            InputManager.Add(new GamePadInputMap(InputUnit.GamePads[0]));

            AudioUnit.Load("pulse_low_alt");
            AudioUnit.Load("pulse_low");
            AudioUnit.Load("pulse_mid");
            AudioUnit.Load("pulse_high");
            AudioUnit.Load("select");
            AudioUnit.Load("refill");

            AudioUnit.Load("bgm");
            AudioUnit.Load("bgm1");
            AudioUnit.Load("bgmboss");

            AudioUnit.Load("boss_defeat");

            ActiveScene = new IntroScreen();
        }

        private bool InputQueued = true;

        public void Update(TimeSpan timeSpan, TimeSpan elapsedGameTime)
        {
            Accumulator += timeSpan.TotalMilliseconds * GameSpeed;
            if(Accumulator >= (1000d/60))
            {
                Accumulator -= 1000d / 60;
                ActiveScene.InvokeUpdate((float)(timeSpan.TotalMilliseconds * GameSpeed / 1000));
                InputQueued = true;
            }
        }

        public void Input()
        {
            if(InputQueued)
            {
                InputManager.Update();
                ActiveScene.InvokeInput();
                InputQueued = false;
            }
        }

        private bool ResolutionChanged = false;

        public void CommandFired(Command command)
        {
            switch(command)
            {
                case InternalSceneChangeCommand changeScene:
                    {
                        Scene oldScene = ActiveScene;
                        ActiveScene = changeScene.NewScene;
                        oldScene.Dispose();
                        LevelRenderingLayer.LevelScreenSize = LevelRenderingLayer.DefaultLevelScreenSize;
                        ResolutionChanged = true;
                        break;
                    }
                case SavedSceneChangeCommand changeScene:
                    {
                        Scene oldScene = ActiveScene;
                        Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new LoadingScreen()));
                        oldScene.Dispose();
                        LevelRenderingLayer.LevelScreenSize = LevelRenderingLayer.DefaultLevelScreenSize;
                        ResolutionChanged = true;

                        new Thread(() => {
                            ActiveScene = CurrentSave.GetScene(changeScene.SceneName);
                            if(ActiveScene == null)
                            {
                                ActiveScene = new MainMenuScene();
                                Console.WriteLine("An error occurred");
                            } else
                            {
                                Woofer.Controller.CurrentSave.Data.ActiveSceneName = changeScene.SceneName;
                                CommandFired(new SaveCommand());
                            }
                        }).Start();
                        break;
                    }
                case ResolutionChangeCommand changeResolution:
                    {
                        LevelRenderingLayer.LevelScreenSize = changeResolution.NewResolution;
                        ResolutionChanged = true;
                        break;
                    }
                case SaveCommand save:
                    {
                        Woofer.Controller.CurrentSave.Save();
                        break;
                    }
            }
        }

        public void Draw<TSurface, TSource>(ScreenRenderer<TSurface, TSource> screenRenderer)
        {
            if(ResolutionChanged)
            {
                screenRenderer.UpdateLayers(RenderingUnit);
                ResolutionChanged = false;
            }
            RenderingUnit.Draw(screenRenderer);
        }
    }
}
