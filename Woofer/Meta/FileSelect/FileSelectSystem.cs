using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using GameInterfaces.Controller;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using WooferGame.Controller.Commands;
using WooferGame.Scenes;
using WooferGame.Common;
using WooferGame.Controller.Game;
using System.IO;
using WooferGame.Meta.LevelEditor.Systems;
using EntityComponentSystem.Saves.Json.Objects;
using EntityComponentSystem.Interfaces.Visuals;
using WooferGame.Input;
using GameInterfaces.Input;
using WooferGame.Systems.Visual;
using EntityComponentSystem.Util;

namespace WooferGame.Meta.FileSelect
{
    [ComponentSystem("file_select", ProcessingCycles.Input | ProcessingCycles.Render)]
    class FileSelectSystem : ComponentSystem
    {
        public const int MaxFileCount = 4;
        private List<GameData> Slots = new List<GameData>();
        private int SelectedIndex = 0;

        private static InputTimeframe SelectInput = new InputTimeframe(1);

        public FileSelectSystem()
        {
            for(int i = 0; i < MaxFileCount; i++)
            {
                string folderPath = Path.Combine(Woofer.DirectoryPath, "save" + (i + 1));
                string dataPath = Path.Combine(folderPath, "data.wgf");
                if(!File.Exists(dataPath))
                {
                    Slots.Add(null);
                } else
                {
                    BinaryReader reader = new BinaryReader(new FileStream(dataPath, FileMode.Open));
                    TagCompound rootTag = TagIOUtils.Master.Read(reader);
                    Slots.Add(rootTag.Get<GameData>(TagIOUtils.Master, "data"));
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if (inputMap.Back.Consume())
            {
                Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new MainMenuScene()));
            }

            SelectInput.RegisterState(inputMap.Movement.Magnitude > 0 ? ButtonState.Pressed : ButtonState.Released);

            if (inputMap.Movement.Magnitude > 0)
            {
                if (inputMap.Movement.Y > 0 && SelectInput.Execute())
                {
                    SelectedIndex--;
                    Woofer.Controller.AudioUnit["select"].Play();
                }
                else if (inputMap.Movement.Y < 0 && SelectInput.Execute())
                {
                    SelectedIndex++;
                    Woofer.Controller.AudioUnit["select"].Play();
                }

                if (SelectedIndex >= MaxFileCount) SelectedIndex = 0;
                if (SelectedIndex < 0) SelectedIndex = 3;
            }


            if (inputMap.Jump.Consume())
            {
                Woofer.Controller.CurrentSave = new SaveGame("save" + (SelectedIndex + 1));
                if(Slots[SelectedIndex] != null)
                {
                    Woofer.Controller.CurrentSave.Load();
                } else
                {
                    Woofer.Controller.CurrentSave.CreateNew();
                }
                Woofer.Controller.CommandFired(new SavedSceneChangeCommand(Woofer.Controller.CurrentSave.Data.ActiveSceneName));
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var bg = r.GetLayerGraphics("hud");

            bg.Draw(r.SpriteManager["menu_bg"], new System.Drawing.Rectangle(new Point(0, 0), bg.GetSize()));

            var layer = r.GetLayerGraphics("hi_res_overlay");

            int y = 0;

            int index = 0;
            foreach(GameData slot in Slots)
            {
                y += 128;
                DrawSlot(index, y, r, layer);
                index++;
            }

            new TextUnit("Select a file").Render(r, layer, new Point(32, 32), 4);
        }

        private void DrawSlot<TSurface, TSource>(int index, int y, ScreenRenderer<TSurface, TSource> r, LayerGraphics<TSurface, TSource> layer)
        {
            Color a = Color.FromArgb(104, 113, 122);
            Color b = Color.FromArgb(43, 46, 66);

            if(SelectedIndex == index)
            {
                Color temp = a;
                a = b;
                b = temp;
            }

            System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(128, y, layer.GetSize().Width - 256, 96);

            layer.FillRect(bounds, (SelectedIndex == index) ? Color.FromArgb(65, 72, 89) : Color.FromArgb(31, 24, 51));
            layer.FillRect(new System.Drawing.Rectangle(bounds.X, bounds.Y, bounds.Width, 4), a);
            layer.FillRect(new System.Drawing.Rectangle(bounds.X, bounds.Y, 4, bounds.Height), a);
            layer.FillRect(new System.Drawing.Rectangle(bounds.X, bounds.Y + bounds.Height - 4, bounds.Width, 4), b);
            layer.FillRect(new System.Drawing.Rectangle(bounds.X + bounds.Width - 4, bounds.Y, 4, bounds.Height), b);

            GameData slot = Slots[index];

            string text = "File " + (index + 1);
            if (slot == null && SelectedIndex == index) text += "    -    Create";

            List<Sprite> icons = new List<Sprite>();

            new TextUnit(text).Render(r, layer, new Point(bounds.X + bounds.Height, y + 24), 3);
            if(slot != null)
            {
                layer.Draw(r.SpriteManager["gui"], new System.Drawing.Rectangle(bounds.X, bounds.Y, bounds.Height, bounds.Height), new System.Drawing.Rectangle(0, 76, 32, 32));
                if(slot.HasWoofer)
                {
                    icons.Add(new Sprite("gui", new Rectangle(0, 0, 21, 21), new Rectangle(0, slot.MaxEnergy > 100 ? 34 : 13, 21, 21)));
                }
                if(slot.MaxHealth > 4)
                {
                    icons.Add(new Sprite("gui", new Rectangle(0, 0, 19, 21), new Rectangle(0, 55, 19, 21)));
                }
            }

            int iconX = bounds.X + bounds.Height;
            int iconY = y + 56;
            foreach(Sprite sprite in icons)
            {
                layer.Draw(r.SpriteManager[sprite.Texture], new System.Drawing.Rectangle(iconX+(int)sprite.Destination.X, iconY+(int)sprite.Destination.Y, (int)sprite.Destination.Width, (int)sprite.Destination.Height), sprite.Source != Rectangle.Empty ? sprite.Source.ToDrawing() : (System.Drawing.Rectangle?)null);
                iconX += 24;
            }
        }
    }
}
