using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using GameInterfaces.GraphicsInterface;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [ComponentSystem("EditorRendering", ProcessingCycles.Render)]
    class EditorRendering : ComponentSystem
    {
        public const int SidebarX = 1280 * 4 / 5;
        public const int SidebarWidth = 1280 / 5;

        public static int SidebarMargin = 8;

        public override bool ShouldSave => false;

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var layer = r.GetLayerGraphics("hi_res_overlay");
            System.Drawing.Rectangle sidebar = new System.Drawing.Rectangle(SidebarX, 0, SidebarWidth, 720);
            layer.FillRect(sidebar, Color.FromArgb(45, 45, 48));
            layer.FillRect(new System.Drawing.Rectangle(sidebar.X + SidebarMargin, sidebar.Y + SidebarMargin, sidebar.Width - 2*SidebarMargin, sidebar.Height - 2*SidebarMargin), Color.FromArgb(37, 37, 38));
            //layer.Draw(r.SpriteManager["pixel"], new System.Drawing.Rectangle(1280 * 4 / 5, 0, 1280 / 10, 720), info: new DrawInfo() { Mode = DrawMode.Normal, Color = Color.Black });
        }
    }
}
