using System;
using System.Collections.Generic;
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
using WooferGame.Controller;
using WooferGame.Meta.LevelEditor.Systems.EntityOutlines;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;
using Point = System.Drawing.Point;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [ComponentSystem("entity_outliner", ProcessingCycles.Render),
        Listening(typeof(BeginOverlay), typeof(RemoveOverlay), typeof(ClearOverlays))]
    class OutlineSystem : ComponentSystem
    {
        private List<IOverlay> Highlighted = new List<IOverlay>();

        public override bool ShouldSave => false;

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var layer = r.GetLayerGraphics("hi_res_overlay");

            CameraView view = Owner.CurrentViewport;

            foreach (IOverlay overlay in Highlighted)
            {
                if (overlay is IOutline outline)
                {
                    Rectangle? realBounds = outline.Bounds;
                    if (realBounds.HasValue)
                    {
                        System.Drawing.Size screenSize = Woofer.Controller.RenderingUnit.ScreenSize;
                        System.Drawing.Size layerSize = layer.GetSize();

                        float x = (float)realBounds.Value.X;
                        float y = (float)realBounds.Value.Y;
                        float width = (float)realBounds.Value.Width;
                        float height = (float)realBounds.Value.Height;

                        if (!new Rectangle(x, y, width, height)
                            .IntersectsWith(
                            new Rectangle(view.X - layerSize.Width / 2, view.Y - layerSize.Height / 2, layerSize.Width, layerSize.Height))) continue;

                        x -= (int)Math.Floor(view.X);
                        y -= (int)Math.Floor(view.Y);

                        y *= -1;
                        y -= height;

                        float scale = (screenSize.Width / (float)LevelRenderingLayer.LevelScreenSize.Width);

                        x *= scale;
                        y *= scale;
                        width *= scale;
                        height *= scale;

                        x += layerSize.Width / 2;
                        y += layerSize.Height / 2;

                        x = (float)Math.Floor(x);
                        y = (float)Math.Floor(y);

                        x -= (int)(scale * GeneralUtil.EuclideanMod(view.X, 1));
                        y += (int)(scale * (GeneralUtil.EuclideanMod(view.Y, 1) - 1));
                        
                        System.Drawing.Rectangle drawingRect = new System.Drawing.Rectangle((int)Math.Floor(x), (int)Math.Floor(y), (int)width, (int)height);

                        layer.FillRect(drawingRect, new DrawInfo() { Color = outline.Fill, Mode = DrawMode.Additive });

                        layer.FillRect(new System.Drawing.Rectangle(drawingRect.X, drawingRect.Y, drawingRect.Width, outline.Thickness), outline.Color);
                        layer.FillRect(new System.Drawing.Rectangle(drawingRect.X, drawingRect.Y + drawingRect.Height - outline.Thickness, drawingRect.Width, outline.Thickness), outline.Color);
                        layer.FillRect(new System.Drawing.Rectangle(drawingRect.X, drawingRect.Y, outline.Thickness, drawingRect.Height), outline.Color);
                        layer.FillRect(new System.Drawing.Rectangle(drawingRect.X + drawingRect.Width - outline.Thickness, drawingRect.Y, outline.Thickness, drawingRect.Height), outline.Color);

                        //layer.Draw(r.SpriteManager["grass"], drawingRect);
                    }
                } else if(overlay is ILine line)
                {
                    System.Drawing.Size screenSize = Woofer.Controller.RenderingUnit.ScreenSize;

                    float x0 = (float)line.Start.X;
                    float y0 = (float)line.Start.Y;
                    float x1 = (float)line.End.X;
                    float y1 = (float)line.End.Y;

                    if (!new Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x1-x0), Math.Abs(y1-y0))
                        .IntersectsWith(
                        new Rectangle(view.X - layer.GetSize().Width / 2, view.Y - layer.GetSize().Height / 2, layer.GetSize().Width, layer.GetSize().Height))) continue;

                    x0 -= (int)Math.Floor(view.X);
                    y0 -= (int)Math.Floor(view.Y);

                    x1 -= (int)Math.Floor(view.X);
                    y1 -= (int)Math.Floor(view.Y);

                    y0 *= -1;
                    y1 *= -1;

                    float scale = (screenSize.Width / (float)LevelRenderingLayer.LevelScreenSize.Width);

                    x0 *= scale;
                    y0 *= scale;
                    x1 *= scale;
                    y1 *= scale;

                    x0 += layer.GetSize().Width / 2;
                    y0 += layer.GetSize().Height / 2;
                    x1 += layer.GetSize().Width / 2;
                    y1 += layer.GetSize().Height / 2;

                    x0 = (float)Math.Floor(x0);
                    y0 = (float)Math.Floor(y0);
                    x1 = (float)Math.Floor(x1);
                    y1 = (float)Math.Floor(y1);

                    x0 -= (int)(scale * GeneralUtil.EuclideanMod(view.X, 1));
                    y0 += (int)(scale * (GeneralUtil.EuclideanMod(view.Y, 1) - 1));
                    x1 -= (int)(scale * GeneralUtil.EuclideanMod(view.X, 1));
                    y1 += (int)(scale * (GeneralUtil.EuclideanMod(view.Y, 1) - 1));
                    
                    layer.DrawLine(new Point((int)x0, (int)y0), new Point((int)x1, (int)y1), line.Color, line.Thickness);
                }
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if(e is BeginOverlay begin)
            {
                Highlighted.Add(begin.Overlay);
            } else if(e is ClearOverlays)
            {
                Highlighted.Clear();
            } else if(e is RemoveOverlay remove)
            {
                Highlighted.Remove(remove.Overlay);
            }
        }
    }
}
