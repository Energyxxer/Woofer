using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;
using Color = System.Drawing.Color;

namespace WooferGame.Meta.LevelEditor.Systems.EntityOutlines
{
    interface IOverlay
    {

    }

    interface ILine : IOverlay
    {
        Vector2D Start { get; }
        Vector2D End { get; }
        Color Color { get; }
        int Thickness { get; }
    }

    class SimpleLine : ILine
    {
        public Vector2D Start { get; set; }
        public Vector2D End { get; set; }
        public Color Color { get; set; }
        public int Thickness { get; set; }

        public SimpleLine(Vector2D start, Vector2D end, Color color, int thickness)
        {
            Start = start;
            End = end;
            Color = color;
            Thickness = thickness;
        }
    }

    interface IOutline : IOverlay
    {
        Rectangle? Bounds { get; }
        Color Color { get; }
        int Thickness { get; }
        Color Fill { get; }
    }

    class RectangleOutline : IOutline
    {
        public Rectangle? Bounds { get; set; }
        public Color Color { get; set; }
        public int Thickness { get; set; } = 2;
        public Color Fill { get; set; } = Color.Transparent;

        public RectangleOutline(Rectangle bounds, Color color) : this(bounds, color, 4)
        {

        }

        public RectangleOutline(Rectangle bounds, Color color, int thickness)
        {
            Bounds = bounds;
            Color = color;
            Thickness = thickness;
        }
    }

    class SpriteOutline : IOutline
    {
        public Vector2D Origin;
        public Sprite Sprite;
        public Rectangle? Bounds => Sprite.Destination + Origin;
        public Color Color { get; set; }
        public int Thickness { get; set; } = 2;
        public Color Fill { get; set; } = Color.Transparent;

        public SpriteOutline(Vector2D origin, Sprite sprite, Color color) : this(origin, sprite, color, 4)
        {

        }

        public SpriteOutline(Vector2D origin, Sprite sprite, Color color, int thickness)
        {
            Origin = origin;
            Sprite = sprite;
            Color = color;
            Thickness = thickness;
        }
    }

    class CollisionBoxOutline : IOutline
    {
        public CollisionBox Box { get; set; }
        public Color Color { get; set; }
        public int Thickness { get; set; } = 2;

        private Vector2D Pivot;
        public Rectangle? Bounds => Box.ToRectangle() + Pivot;

        public Color Fill { get; set; } = Color.Transparent;

        public CollisionBoxOutline(Vector2D pivot, CollisionBox bounds, Color color) : this(pivot, bounds, color, 2)
        {

        }

        public CollisionBoxOutline(Vector2D pivot, CollisionBox bounds, Color color, int thickness)
        {
            Pivot = pivot;
            Box = bounds;
            Color = color;
            Thickness = thickness;
        }
    }

    class EntityOutline : IOutline
    {
        public long Id;
        public Scene Scene;

        public Rectangle? Bounds => Id == 0 ? (Rectangle?)null : EditorUtil.GetSelectionBounds(Scene.Entities[Id]);
        public Color Color { get; set; } = Color.White;
        public int Thickness { get; set; } = 2;

        public Color Fill { get; set; } = Color.Transparent;

        public EntityOutline(Scene scene, long id)
        {
            Scene = scene;
            Id = id;
        }
    }
}
