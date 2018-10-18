using System.Collections.Generic;
using System.Drawing;

using GameInterfaces.Controller;
using GameInterfaces.GraphicsInterface;

namespace WooferGame.Controller
{
    class WooferRenderingUnit : IRenderingUnit
    {
        private readonly WooferController controller;
        public Size ScreenSize { get; private set; } = new Size(1280, 720);
        public Dictionary<string, IRenderingLayer> Layers { get; private set; } = new Dictionary<string, IRenderingLayer>();

        public WooferRenderingUnit(WooferController controller)
        {
            this.controller = controller;
            Layers["parallax_back"] = new BackParallaxRenderingLayer(controller);
            Layers["background"] = new BackgroundRenderingLayer(controller);
            Layers["level"] = new LevelRenderingLayer(controller);
            Layers["hud"] = new HudRenderingLayer(controller);
            Layers["parallax_front"] = new FrontParallaxRenderingLayer(controller);
            Layers["hi_res_overlay"] = new RenderingLayer() { Name = "hi_res_overlay", Destination = new Rectangle(0, 0, ScreenSize.Width, ScreenSize.Height), LayerSize = ScreenSize};
        }

        public void LoadContent<TSurface, TSource>(ScreenRenderer<TSurface, TSource> screenRenderer)
        {
            ISpriteManager<TSource> SpriteManager = screenRenderer.SpriteManager;
            SpriteManager.LoadSprite("logo");
            SpriteManager.LoadSprite("logo_board");

            SpriteManager.LoadSprite("sprites0");
            SpriteManager.LoadSprite("intro_bg");
            SpriteManager.LoadSprite("tutorial_room5_bg");
            SpriteManager.LoadSprite("char");
            SpriteManager.LoadSprite("woofer");
            SpriteManager.LoadSprite("enemies");
            SpriteManager.LoadSprite("boss");
            SpriteManager.LoadSprite("boss_particles");
            SpriteManager.LoadSprite("particles");
            SpriteManager.LoadSprite("collectibles");
            SpriteManager.LoadSprite("overlays");
            SpriteManager.LoadSprite("lab_tileset");
            SpriteManager.LoadSprite("lab_objects");
            SpriteManager.LoadSprite("props");
            SpriteManager.LoadSprite("wall_seams");
            SpriteManager.LoadSprite("wall_seamsd");
            SpriteManager.LoadSprite("room_background");
            SpriteManager.LoadSprite("parallax_bg");
            SpriteManager.LoadSprite("switch");
            SpriteManager.LoadSprite("MainSceneBG");
            SpriteManager.LoadSprite("menu_bg");

            SpriteManager.LoadSprite("font");

            SpriteManager.LoadSprite("gui");
            
            SpriteManager.LoadSprite("gamepad_icons");
            SpriteManager.LoadSprite("keyboard_icons");

            SpriteManager.LoadSprite("pixel");

            SpriteManager.LoadSprite("editor");

            ClipSprite("brick", "sprites0", new Rectangle(16, 0, 16, 16), screenRenderer.GraphicsContext, screenRenderer.SpriteManager);
            ClipSprite("grass", "sprites0", new Rectangle(0, 0, 16, 16), screenRenderer.GraphicsContext, screenRenderer.SpriteManager);
         
            ClipSprite("room0", "intro_bg", new Rectangle(0, 0, 432, 400), screenRenderer.GraphicsContext, screenRenderer.SpriteManager);
        }

        private void ClipSprite<TSurface, TSource>(string spriteName, string spritesheet, Rectangle spriteBounds, IGraphicsContext<TSurface, TSource> graphicsContext, ISpriteManager<TSource> spriteManager)
        {
            TSurface target = graphicsContext.CreateTarget(spriteBounds.Width, spriteBounds.Height);
            graphicsContext.Draw(spriteManager[spritesheet], target, new Rectangle(0, 0, spriteBounds.Width, spriteBounds.Height), spriteBounds);
            spriteManager[spriteName] = graphicsContext.TargetToSource(target);
        }

        public void Draw<TSurface, TSource>(ScreenRenderer<TSurface, TSource> screenRenderer) {
            controller.ActiveScene.Draw(screenRenderer);
        }
    }
}
