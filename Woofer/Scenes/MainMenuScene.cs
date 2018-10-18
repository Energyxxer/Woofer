using EntityComponentSystem.Scenes;
using GameInterfaces.Audio;
using WooferGame.Scenes.Menu;
using WooferGame.Systems.Player;
using WooferGame.Systems.Visual;

namespace WooferGame.Scenes
{
    class MainMenuScene : Scene
    {
        public MainMenuScene() : base(Woofer.Controller)
        {
            Systems.Add(new MenuSystem());

            ISoundEffect music = Controller.AudioUnit["bgm"];
            music.Looping = true;
            music.Volume = 0.4f;
            music.PlayAsMusic();
        }
    }
}
