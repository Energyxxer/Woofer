using System;
using System.Collections.Generic;
using System.Linq;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;

using GameInterfaces.Controller;
using GameInterfaces.GraphicsInterface;
using WooferGame.Controller;

namespace WooferGame.Systems.Visual
{
    [Component("renderable")]
    class Renderable : Component
    {
        [PersistentProperty]
        public List<Sprite> Sprites { get; set; }

        public Renderable()
        {
            Sprites = new List<Sprite>();
        }

        public Renderable(string texture, Rectangle bounds)
        {
            Sprites = new List<Sprite>() { new Sprite(texture, bounds) };
        }

        public Renderable(params Sprite[] sprites)
        {
            Sprites = sprites.ToList();
        }

        public void AddSprite(Sprite sprite)
        {
            Sprites.Add(sprite);
        }

        public static System.Drawing.Point ToScreenCoordinates(Vector2D pos, System.Drawing.Size layerSize)
        {
            float x = (float)pos.X;
            float y = (float)pos.Y;

            CameraView view = Woofer.Controller.ActiveScene.CurrentViewport;

            x -= (int)Math.Floor(view.X);
            y -= (int)Math.Floor(view.Y);

            y *= -1;

            x += LevelRenderingLayer.LevelScreenSize.Width / 2;
            y += LevelRenderingLayer.LevelScreenSize.Height / 2;

            float scaleX = layerSize.Width / LevelRenderingLayer.LevelScreenSize.Width;
            float scaleY = layerSize.Height / LevelRenderingLayer.LevelScreenSize.Height;

            return new System.Drawing.Point((int)Math.Floor(x * scaleX), (int)Math.Floor(y * scaleY));
        }

        public static System.Drawing.Rectangle ToScreenCoordinates(Rectangle rectangle, System.Drawing.Size layerSize)
        {
            float x = (float)rectangle.X;
            float y = (float)rectangle.Y;
            float width = (float)rectangle.Width;
            float height = (float)rectangle.Height;

            CameraView view = Woofer.Controller.ActiveScene.CurrentViewport;

            if (!new Rectangle(x, y, width, height)
                .IntersectsWith(
                new Rectangle(view.X - LevelRenderingLayer.LevelScreenSize.Width / 2, view.Y - LevelRenderingLayer.LevelScreenSize.Height / 2, LevelRenderingLayer.LevelScreenSize.Width, LevelRenderingLayer.LevelScreenSize.Height))) return new System.Drawing.Rectangle(0, 0, 0, 0);

            x -= (int)Math.Floor(view.X);
            y -= (int)Math.Floor(view.Y);

            y *= -1;
            y -= height;

            x += LevelRenderingLayer.LevelScreenSize.Width / 2;
            y += LevelRenderingLayer.LevelScreenSize.Height / 2;

            float scaleX = layerSize.Width / LevelRenderingLayer.LevelScreenSize.Width;
            float scaleY = layerSize.Height / LevelRenderingLayer.LevelScreenSize.Height;

            return new System.Drawing.Rectangle((int)Math.Floor(x * scaleX), (int)Math.Floor(y * scaleY), (int)(width * scaleX), (int)(height * scaleY));
        }

        public static void OrderSprites(List<Sprite> sprites)
        {
            IEnumerable<Sprite> sorted = sprites.OrderBy(s => s.ViewOrder).ToArray();
            sprites.Clear();
            sprites.AddRange(sorted);
        }

        public static void Render<TSurface, TSource>(DirectGraphicsContext<TSurface, TSource> layer, CameraView view, ScreenRenderer<TSurface, TSource> r, List<Sprite> sprites, Spatial spatial = null)
        {
            IGameController controller = Woofer.Controller;
            
            foreach (Sprite sprite in sprites)
            {
                if (sprite.Source.X < 0 || sprite.Source.Y < 0) continue;
                float x = (float)sprite.Destination.X;
                float y = (float)sprite.Destination.Y;
                if (spatial != null)
                {
                    x += ((float)(spatial.Position.X));
                    y += ((float)(spatial.Position.Y));
                }
                float width = (float)sprite.Destination.Width;
                float height = (float)sprite.Destination.Height;
                
                if (!new System.Drawing.RectangleF(x, y, width, height)
                    .IntersectsWith(
                    new System.Drawing.RectangleF((float)(view.X - layer.GetSize().Width / 2), (float)(view.Y - layer.GetSize().Height / 2), layer.GetSize().Width, layer.GetSize().Height))) continue;

                x -= (int)Math.Floor(view.X);
                y -= (int)Math.Floor(view.Y);

                y *= -1;
                y -= height;

                x += layer.GetSize().Width / 2;
                y += layer.GetSize().Height / 2;


                System.Drawing.Rectangle drawingRect = new System.Drawing.Rectangle((int)Math.Floor(x), (int)Math.Floor(y), (int)width, (int)height);

                layer.Draw(r.SpriteManager[sprite.Texture], drawingRect, sprite.Source != Rectangle.Empty ? sprite.Source.ToDrawing() : (System.Drawing.Rectangle?)null, new DrawInfo() { Mode = sprite.DrawMode, Color = System.Drawing.Color.FromArgb((int)(sprite.Opacity * 255), 255, 255, 255) });
            }
        }
    }
}
