using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using GameInterfaces.GraphicsInterface;
using GameInterfaces.Input;
using WooferGame.Common;
using WooferGame.Controller.Commands;
using WooferGame.Controller.Game;
using WooferGame.Input;
using WooferGame.Meta;
using WooferGame.Meta.FileSelect;
using WooferGame.Meta.LevelEditor;
using WooferGame.Meta.LevelSelect;
using WooferGame.Meta.Loading;
using WooferGame.Systems.Visual;

namespace WooferGame.Scenes.Menu
{
    [ComponentSystem("menu", ProcessingCycles.Input | ProcessingCycles.Render)]
    class MenuSystem : ComponentSystem
    {
        private static InputTimeframe SelectInput = new InputTimeframe(1);

        private readonly string[] OptionLabels = new[] { "Start", "Options", "Level Editor", "Quit" };
        private int SelectedIndex = 0;

        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            SelectInput.RegisterState(inputMap.Movement.Magnitude > 0 ? ButtonState.Pressed : ButtonState.Released);

            if(inputMap.Movement.Magnitude > 0)
            {
                if(inputMap.Movement.Y > 0 && SelectInput.Execute())
                {
                    SelectedIndex--;
                    Woofer.Controller.AudioUnit["select"].Play();
                } else if(inputMap.Movement.Y < 0 && SelectInput.Execute())
                {
                    SelectedIndex++;
                    Woofer.Controller.AudioUnit["select"].Play();
                }

                if (SelectedIndex > 3 ) SelectedIndex = 0;
                if (SelectedIndex < 0 ) SelectedIndex = 3;
            }


            if (inputMap.Jump.Consume())
            {
                switch (SelectedIndex)
                {
                    case 0:
                        //Woofer.Controller.CurrentSave = new SaveGame("main");
                        //Woofer.Controller.CommandFired(new SavedSceneChangeCommand("Tutorial"));
                        Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new FileSelect()));
                        break;
                    case 1:
                        break;
                    case 2:
                        Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new LevelSelect()));
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var layer = r.GetLayerGraphics("hud");

            layer.Draw(r.SpriteManager["menu_bg"], new System.Drawing.Rectangle(new Point(0, 0), layer.GetSize()));
            
            r.GetLayerGraphics("hi_res_overlay").Draw(r.SpriteManager["logo"], new System.Drawing.Rectangle(648, 128, 480, 152));

            int destY = 100;
            int index = 0;
            foreach(string label in OptionLabels)
            {
                TextUnit text;
                if (index == SelectedIndex) text = new TextUnit(new Sprite("gui", new EntityComponentSystem.Util.Rectangle(0, 0, 8, 8), new EntityComponentSystem.Util.Rectangle(0, 0, 8, 8)), label ,Color.Coral);
                else text = new TextUnit(label , Color.White);
                text.Render(r, layer, new Point(8, destY));
                destY += 12;
                index++;
            }

        }
    }
}
