using System;
using System.Collections.Generic;

using GameInterfaces.GraphicsInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.MonoGameGraphicsImplementation
{
    public class MonoGameSpriteManager : ISpriteManager<Texture2D>
    {
        private readonly Dictionary<string, Texture2D> sprites = new Dictionary<string, Texture2D>();

        private ContentManager content;
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        public Texture2D this[string name] {
            get
            {
                if(name == null) return sprites["null"];
                if(!sprites.ContainsKey(name))
                {
                    sprites[name] = sprites["null"];
                    Console.WriteLine("[WARN]: Attempted to retrieve unknown texture '" + name + "'");
                }
                return sprites[name];
            }
            set => sprites[name] = value;
        }

        public MonoGameSpriteManager(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager content)
        {
            this.graphics = graphics;
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
            this.content = content;

            RenderTarget2D nullTexture = new RenderTarget2D(graphicsDevice, 2, 2);
            nullTexture.SetData(new Color[] { Color.Magenta, Color.Black, Color.Black, Color.Magenta });
            sprites["null"] = nullTexture;
        }

        public void LoadSprite(string name)
        {
            sprites[name] = content.Load<Texture2D>(name);
        }
    }
}
