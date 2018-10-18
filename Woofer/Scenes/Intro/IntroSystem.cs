using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using GameInterfaces.Controller;
using WooferGame.Common;
using WooferGame.Controller.Commands;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using WooferGame.Systems.Visual;
using EntityComponentSystem.Util;

namespace WooferGame.Scenes.Intro
{
    [ComponentSystem("intro_system", ProcessingCycles.All)]
    class IntroSystem : ComponentSystem
    {
        public double Delay = 2;

        public bool Locked { get; internal set; } = false;

        public override void Input()
        {
            if(!Locked && Woofer.Controller.InputManager.ActiveInputMap.Jump.Consume())
            {
                Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new MainMenuScene()));
            }
        }
        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var layer = r.GetLayerGraphics("hi_res_overlay");

            layer.Clear(Color.FromArgb(31, 24, 51));

            System.Drawing.Size layerSize = layer.GetSize();

            var logo = r.SpriteManager["logo"];
            System.Drawing.Size logoSize = r.GraphicsContext.GetSize(logo);
            logoSize.Width *= 4;
            logoSize.Height *= 4;

            layer.Draw(logo, new System.Drawing.Rectangle(layerSize.Width / 2 - logoSize.Width / 2, layerSize.Height / 2 - logoSize.Height / 2, logoSize.Width, logoSize.Height));

            if(Delay <= 0)
            {
                TextUnit text = new TextUnit(new Sprite("x_icons", new Rectangle(0, -4, 9*4, 9*4), new Rectangle(0, 0, 9, 9)) { Modifiers = Sprite.Mod_InputType }, " Start", Color.FromArgb(182, 203, 207));

                System.Drawing.Size textSize = text.GetSize(4);
                text.Render(r, layer, new Point(layerSize.Width/2 - textSize.Width/2, layerSize.Height/2 + logoSize.Height/2 + 32), 4);
            }

        }
        public override void Update()
        {
            if(!Locked && Delay >= 0) Delay -= Owner.DeltaTime;
        }
    }
}
