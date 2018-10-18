using System;
using System.IO;
using EntityComponentSystem.Interfaces.Visuals;
using GameInterfaces.GraphicsInterface;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.MonoGameGraphics
{
    public class MonoGameGraphicsContext : IGraphicsContext<RenderTarget2D, Texture2D>
    {
        private readonly GraphicsDeviceManager manager;
        private readonly GraphicsDevice device;
        private readonly SpriteBatch spriteBatch;

        private RenderTarget2D lastRenderTarget = null;

        private Texture2D pixel = null;
        private Color currentPixelColor = Color.Transparent;

        //Constructors
        public MonoGameGraphicsContext(GraphicsDeviceManager manager, GraphicsDevice device, SpriteBatch spriteBatch)
        {
            this.manager = manager;
            this.device = device;
            this.spriteBatch = spriteBatch;
        }

        private void ChangeRenderTarget(RenderTarget2D surface)
        {
            if (lastRenderTarget != surface)
            {
                device.SetRenderTarget(surface);
                if (surface != null) device.Clear(new Color(255, 255, 255, 0));
                lastRenderTarget = surface;
            }
        }

        private void ChangePixelColor(System.Drawing.Color color)
        {
            Color toXna = DrawingToXna(color);
            if(currentPixelColor != toXna)
            {
                pixel.SetData(new[] { toXna });
                currentPixelColor = toXna;
            }
        }

        //Clear surface
        public void Clear(RenderTarget2D surface, System.Drawing.Color color)
        {
            ChangeRenderTarget(surface);
            device.Clear(new Color(color.R, color.G, color.B, color.A));
        }
        public void Clear(System.Drawing.Color color) => Clear(null, color);

        //Create a target
        public RenderTarget2D CreateTarget(int width, int height) {
            RenderTarget2D target = new RenderTarget2D(device, width, height);
            ChangeRenderTarget(target);
            return target;
        }

        //Create a source from target
        public Texture2D TargetToSource(RenderTarget2D target) => target;

        //Draw texture
        public void Draw(Texture2D subject, RenderTarget2D target, System.Drawing.Rectangle destination, System.Drawing.Rectangle? source = null, DrawInfo info = default(DrawInfo))
        {
            ChangeRenderTarget(target);
            spriteBatch.Begin(SpriteSortMode.Immediate, info.Mode == DrawMode.Additive ? BlendState.Additive : info.Mode == DrawMode.Overlay ? BlendState.AlphaBlend : BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(subject, DrawingToXna(destination), source.HasValue ? DrawingToXna(source.Value) : (Rectangle?)null, info.Color.HasValue ? DrawingToXna(info.Color.Value) : Color.White);
            spriteBatch.End();
        }

        public void FillRect(RenderTarget2D target, System.Drawing.Rectangle rectangle, System.Drawing.Color color) => FillRect(target, rectangle, new DrawInfo() { Color = color, Mode = DrawMode.Normal });

        public void FillRect(RenderTarget2D target, System.Drawing.Rectangle rectangle, DrawInfo info)
        {
            ChangeRenderTarget(target);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendState: ModeToBlend(info.Mode));
            if (pixel == null)
            {
                pixel = new Texture2D(device, 1, 1);
                pixel.SetData(new[] { Color.White });
            }
            //ChangePixelColor(color);
            spriteBatch.Draw(pixel, DrawingToXna(rectangle), null, DrawingToXna(info.Color ?? System.Drawing.Color.White));
            spriteBatch.End();
        }

        public void DrawLine(RenderTarget2D target, System.Drawing.Point point1, System.Drawing.Point point2, System.Drawing.Color color, int thickness)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            int length = (int)Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));

            ChangeRenderTarget(target);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendState: null);
            if (pixel == null)
            {
                pixel = new Texture2D(device, 1, 1);
                pixel.SetData(new[] { Color.White });
            }
            //ChangePixelColor(color);
            spriteBatch.Draw(pixel, DrawingToXna(new System.Drawing.Rectangle(point1.X, point1.Y-thickness/2, length, thickness)), null, DrawingToXna(color), angle, new Vector2(0, 0), SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private BlendState ModeToBlend(DrawMode mode) => mode == DrawMode.Additive ? BlendState.Additive : mode == DrawMode.Overlay ? BlendState.AlphaBlend : BlendState.NonPremultiplied;

        //Retrieve size of texture or screen
        public System.Drawing.Size GetSize(Texture2D surface) => surface != null ? new System.Drawing.Size(surface.Width, surface.Height) : GetScreenSize();// new System.Drawing.Size(surface.Width, surface.Height);
        public System.Drawing.Size GetScreenSize() => new System.Drawing.Size(manager.PreferredBackBufferWidth, manager.PreferredBackBufferHeight);

        //Scale texture
        public RenderTarget2D Scale(RenderTarget2D surface, double scaleX, double scaleY, bool antialias)
        {
            System.Drawing.Size newSize = new System.Drawing.Size((int)(surface.Width * scaleX), (int)(surface.Height * scaleY));
            RenderTarget2D newTarget = CreateTarget(newSize.Width, newSize.Height);
            Draw(surface, newTarget, new System.Drawing.Rectangle(new System.Drawing.Point(0,0), newSize));
            return newTarget;
        }
        public RenderTarget2D Scale(RenderTarget2D surface, double scale, bool antialias) => Scale(surface, scale, scale, antialias);

        //Update screen
        public void Update(RenderTarget2D surface) => Draw(surface, null, new System.Drawing.Rectangle(0, 0, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight));
        public void Update(RenderTarget2D surface, System.Drawing.Rectangle destination)
        {
            //System.Console.WriteLine(destination);
            Draw(surface, null, destination);
        }

        //Dispose surfaces
        public void DisposeSurface(RenderTarget2D surface) => surface.Dispose();
        public void DisposeSource(Texture2D source) => source.Dispose();

        //Utility conversion from System.Drawing.Rectangle to Microsoft.Xna.Framework
        private static Rectangle DrawingToXna(System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        private static Color DrawingToXna(System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public void Begin()
        {
            ChangeRenderTarget(null);
            device.Clear(Color.Black);
        }
        public void Reset() => ChangeRenderTarget(null);

        //Redirection of subclasses of Texture2D (as type parameter 1 extends type parameter 2)
        System.Drawing.Size IGraphicsContext<RenderTarget2D, Texture2D>.GetSize(RenderTarget2D surface) => GetSize(surface);
        void IGraphicsContext<RenderTarget2D, Texture2D>.Draw(RenderTarget2D subject, RenderTarget2D target, System.Drawing.Rectangle destination, System.Drawing.Rectangle? source, DrawInfo info) => Draw(subject, target, destination, source, info);
        void IGraphicsContext<RenderTarget2D, Texture2D>.Draw(Texture2D subject, RenderTarget2D target, System.Drawing.Rectangle destination, System.Drawing.Rectangle? source, DrawInfo info) => Draw(subject, target, destination, source, info);
    }
}
