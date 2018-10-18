using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using WooferGame.Common;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor.Systems.EntityOutlines;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;
using Color = System.Drawing.Color;

namespace WooferGame.Meta.LevelEditor.Systems.CursorModes
{
    [ComponentSystem("sprite_cursor_mode", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartSpriteModeEvent))]
    class SpriteCursorModeSystem : ComponentSystem
    {
        private EditorCursorSystem CursorSystem = null;
        private bool ModalActive = false;
        private bool ModalVisible = false;
        private StartSpriteModeEvent.OnSelect Callback = null;

        private Vector2D Origin;
        private bool MultipleAllowed = true;

        public override bool ShouldSave => false;
        private List<Sprite> Sprites = new List<Sprite>();

        private List<IOutline> Outlines = new List<IOutline>();
        private RectangleOutline FaceOutline = null;

        private Sprite SelectedSprite = null;
        private bool SelectionLocked = false;

        private int DraggingSide = -1;
        private Rectangle PreResizeBounds = Rectangle.Empty;

        private bool Creating = false;

        private byte Mode = Edit;

        private const byte Edit = 0;
        private const byte TweakUV = 1;
        private const byte Create = 2;
        private const byte Delete = 3;
        private const byte Clone = 4;

        public override void Update()
        {
            if (CursorSystem == null) CursorSystem = Owner.Systems["editor_cursor"] as EditorCursorSystem;
        }

        private void UpdateSelected()
        {
            SelectedSprite = Sprites.LastOrDefault(s => (s.Destination + Origin).Contains(CursorSystem.CursorPos));
            foreach (IOutline outline in Outlines)
            {
                if (outline is SpriteOutline spriteOutline)
                {
                    spriteOutline.Color = spriteOutline.Sprite == SelectedSprite ? Color.Yellow : Color.Orange;
                    spriteOutline.Thickness = spriteOutline.Sprite == SelectedSprite ? 3 : 2;
                }
            }
        }

        private int UpdateSelectedFace()
        {
            Rectangle selectedBounds = SelectedSprite.Destination + Origin;

            if (selectedBounds.Bottom <= CursorSystem.CursorPos.Y && CursorSystem.CursorPos.Y <= selectedBounds.Top)
            {
                if (Math.Abs(selectedBounds.Left - CursorSystem.CursorPos.X) <= 4)
                {
                    FaceOutline.Bounds = new Rectangle(selectedBounds.Left, selectedBounds.Bottom, 1, selectedBounds.Height);

                    return 3;
                }
                else if (Math.Abs(selectedBounds.Right - CursorSystem.CursorPos.X) <= 4)
                {
                    FaceOutline.Bounds = new Rectangle(selectedBounds.Right, selectedBounds.Bottom, 1, selectedBounds.Height);

                    return 1;
                }
            }
            if (selectedBounds.Left <= CursorSystem.CursorPos.X && CursorSystem.CursorPos.X <= selectedBounds.Right)
            {
                if (Math.Abs(selectedBounds.Bottom - CursorSystem.CursorPos.Y) <= 4)
                {
                    FaceOutline.Bounds = new Rectangle(selectedBounds.Left, selectedBounds.Bottom, selectedBounds.Width, 1);

                    return 2;
                }
                else if (Math.Abs(selectedBounds.Top - CursorSystem.CursorPos.Y) <= 4)
                {
                    FaceOutline.Bounds = new Rectangle(selectedBounds.Left, selectedBounds.Top, selectedBounds.Width, 1);

                    return 0;
                }
            }
            return -1;
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is StartSpriteModeEvent start)
            {
                Origin = start.Origin;
                Callback = start.Callback;
                MultipleAllowed = start.MultipleSprites;
                Sprites = start.Sprites;
                Mode = Edit;

                foreach (Sprite sprite in Sprites)
                {
                    IOutline outline = new SpriteOutline(Origin, sprite, Color.Purple);
                    Outlines.Add(outline);
                    Owner.Events.InvokeEvent(new BeginOverlay(outline));
                }

                IOutline pivotOutline = new RectangleOutline(new Rectangle(Origin - new Vector2D(1, 1), new Size(2, 2)), Color.White, 4);
                Outlines.Add(pivotOutline);
                Owner.Events.InvokeEvent(new BeginOverlay(pivotOutline));

                FaceOutline = new RectangleOutline(new Rectangle(0, 0, 0, 0), Color.White, 4);
                Outlines.Add(FaceOutline);
                Owner.Events.InvokeEvent(new BeginOverlay(FaceOutline));
            }
            else if (e is ModalChangeEvent changed)
            {
                //changed.Valid = false;
                //Owner.Events.InvokeEvent(new ForceModalChangeEvent("editor_cursor", null));
                ModalActive = true;
                ModalVisible = true;
                CursorSystem.ModalActive = true;
                CursorSystem.DraggingEnabled = true;
                if (changed.From != "sprite_source_view") CursorSystem.SwitchToModal = changed.From;
                if(Sprites != null) Renderable.OrderSprites(Sprites);
                //CursorSystem.OutlinesEnabled = true;
            }
            else if (e is BeginModalChangeEvent bmce)
            {
                if (SelectionLocked)
                {
                    SelectionLocked = false;
                    return;
                }

                bmce.SystemName = CursorSystem.SwitchToModal;
                ModalActive = false;
                ModalVisible = false;
                CursorSystem.ModalActive = false;
                CursorSystem.DraggingEnabled = false;
                Callback(Sprites);
                Callback = null;

                foreach (IOutline outline in Outlines)
                {
                    Owner.Events.InvokeEvent(new RemoveOverlay(outline));
                }
                Outlines.Clear();
            }
        }

        public override void Input()
        {
            if (!ModalActive) return;

            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if (inputMap.Pulse.Consume())
            {
                Mode++;
                if ((MultipleAllowed && Mode > 4) || (!MultipleAllowed && Mode > 1)) Mode = 0;
                SelectionLocked = false;
            }

            if (Mode == Edit)
            {
                if (!SelectionLocked)
                {
                    UpdateSelected();
                    if (SelectedSprite != null && inputMap.Jump.Consume())
                    {
                        SelectionLocked = true;
                    }
                }
                if (SelectionLocked)
                {
                    if (!CursorSystem.Dragging)
                    {
                        DraggingSide = -1;
                    }
                    FaceOutline.Bounds = new Rectangle(FaceOutline.Bounds?.X ?? 0, FaceOutline.Bounds?.Y ?? 0, 0, 0);

                    Rectangle selectedBounds = SelectedSprite.Destination + Origin;

                    if (CursorSystem.StartedDragging)
                    {
                        PreResizeBounds = selectedBounds;
                    }

                    int highlightedSide = UpdateSelectedFace();
                    if (highlightedSide != -1 && DraggingSide == -1 && CursorSystem.Dragging) DraggingSide = highlightedSide;

                    if (CursorSystem.Dragging && DraggingSide == -1 && selectedBounds.Contains(CursorSystem.CursorPos))
                    {
                        DraggingSide = 4;
                    }

                    if (CursorSystem.Dragging && DraggingSide != -1 && PreResizeBounds != null)
                    {
                        if (DraggingSide == 3) //Left
                        {
                            double prevRight = SelectedSprite.Destination.Right;
                            SelectedSprite.Destination.X = (CursorSystem.CursorPos.X - Origin.X);
                            SelectedSprite.Destination.Width = prevRight - SelectedSprite.Destination.X;
                            if (SelectedSprite.Destination.Width < 0)
                            {
                                double min = Math.Min(SelectedSprite.Destination.Left, SelectedSprite.Destination.Right);
                                double max = Math.Max(SelectedSprite.Destination.Left, SelectedSprite.Destination.Right);
                                SelectedSprite.Destination.X = min;
                                SelectedSprite.Destination.Width = max - min;
                                DraggingSide = 1;
                            }
                        }
                        else if (DraggingSide == 1) //Right
                        {
                            SelectedSprite.Destination.Width = (CursorSystem.CursorPos.X - SelectedSprite.Destination.X) - Origin.X;

                            if (SelectedSprite.Destination.Width < 0)
                            {
                                double min = Math.Min(SelectedSprite.Destination.Left, SelectedSprite.Destination.Right);
                                double max = Math.Max(SelectedSprite.Destination.Left, SelectedSprite.Destination.Right);
                                SelectedSprite.Destination.X = min;
                                SelectedSprite.Destination.Width = max - min;
                                DraggingSide = 3;
                            }
                        }
                        else if (DraggingSide == 2) //Bottom
                        {
                            double prevTop = SelectedSprite.Destination.Top;
                            SelectedSprite.Destination.Y = (CursorSystem.CursorPos.Y - Origin.Y);
                            SelectedSprite.Destination.Height = prevTop - SelectedSprite.Destination.Y;
                            if (SelectedSprite.Destination.Height < 0)
                            {
                                double min = Math.Min(SelectedSprite.Destination.Bottom, SelectedSprite.Destination.Top);
                                double max = Math.Max(SelectedSprite.Destination.Bottom, SelectedSprite.Destination.Top);
                                SelectedSprite.Destination.Y = min;
                                SelectedSprite.Destination.Height = max - min;
                                DraggingSide = 0;
                            }
                        }
                        else if (DraggingSide == 0) //Top
                        {
                            SelectedSprite.Destination.Height = (CursorSystem.CursorPos.Y - SelectedSprite.Destination.Y) - Origin.Y;

                            if (SelectedSprite.Destination.Height < 0)
                            {
                                double min = Math.Min(SelectedSprite.Destination.Bottom, SelectedSprite.Destination.Top);
                                double max = Math.Max(SelectedSprite.Destination.Bottom, SelectedSprite.Destination.Top);
                                SelectedSprite.Destination.Y = min;
                                SelectedSprite.Destination.Height = max - min;
                                DraggingSide = 2;
                            }
                        }
                        else if (DraggingSide == 4) //Middle
                        {
                            SelectedSprite.Destination.X = PreResizeBounds.X + (CursorSystem.CursorPos.X - CursorSystem.SelectionStart.X) - Origin.X;
                            SelectedSprite.Destination.Y = PreResizeBounds.Y + (CursorSystem.CursorPos.Y - CursorSystem.SelectionStart.Y) - Origin.Y;
                        }
                    }
                }
            }
            else if (Mode == Create)
            {
                if (CursorSystem.StartedDragging)
                {
                    Sprite newSprite = new Sprite("null", CursorSystem.SelectionRectangle - Origin, Rectangle.Empty);
                    AddSprite(newSprite);
                    Creating = true;
                }
                if (CursorSystem.Dragging && Creating)
                {
                        Sprite newSprite = Sprites.Last();
                    newSprite.Destination.X = CursorSystem.SelectionRectangle.X - Origin.X;
                    newSprite.Destination.Y = CursorSystem.SelectionRectangle.Y - Origin.Y;
                    newSprite.Destination.Width = CursorSystem.SelectionRectangle.Width;
                    newSprite.Destination.Height = CursorSystem.SelectionRectangle.Height;
                }
                if (CursorSystem.StoppedDragging && Creating)
                {
                    Sprite newSprite = Sprites.Last();
                    if (newSprite.Destination.Area == 0) RemoveSprite(newSprite);
                    Creating = false;
                }
            }
            else if (Mode == Delete)
            {
                UpdateSelected();
                if (SelectedSprite != null)
                {
                    IOutline associatedOutline = null;
                    foreach (IOutline outline in Outlines)
                    {
                        if (outline is SpriteOutline spriteOutline)
                        {
                            if (spriteOutline.Sprite == SelectedSprite)
                            {
                                spriteOutline.Color = Color.Red;
                                spriteOutline.Thickness = 4;
                                associatedOutline = outline;
                            }
                        }
                    }
                    if (inputMap.Jump.Consume())
                    {
                        RemoveSprite(SelectedSprite);
                    }
                }
            }
            else if (Mode == TweakUV)
            {
                if (!SelectionLocked)
                {
                    UpdateSelected();
                    if (SelectedSprite != null && inputMap.Jump.Consume())
                    {
                        //SelectionLocked = true;
                        Owner.Events.InvokeEvent(new StartSpriteSourceEditEvent(SelectedSprite));
                        Owner.Events.InvokeEvent(new ForceModalChangeEvent("sprite_source_view", null));
                        CursorSystem.ModalActive = false;
                        ModalActive = false;
                        Console.WriteLine("Start editing UV of sprite");
                    }
                }
            } else if(Mode == Clone)
            {
                UpdateSelected();
                if (SelectedSprite != null)
                {
                    IOutline associatedOutline = null;
                    foreach (IOutline outline in Outlines)
                    {
                        if (outline is SpriteOutline spriteOutline)
                        {
                            if (spriteOutline.Sprite == SelectedSprite)
                            {
                                spriteOutline.Color = Color.MediumAquamarine;
                                spriteOutline.Thickness = 4;
                                associatedOutline = outline;
                            }
                        }
                    }
                    if (inputMap.Jump.Consume())
                    {
                        Sprite clone = new Sprite
                        {
                            Texture = SelectedSprite.Texture,
                            Destination = new Rectangle(SelectedSprite.Destination) + new Vector2D(4, 4),
                            Source = SelectedSprite.Source,
                            DrawMode = SelectedSprite.DrawMode,
                            Modifiers = SelectedSprite.Modifiers,
                            Opacity = SelectedSprite.Opacity
                        };
                        AddSprite(clone);
                    }
                }
            }
        }

        private void RemoveSprite(Sprite sprite)
        {
            IOutline associatedOutline = Outlines.Find(o => o is SpriteOutline spriteOutline && spriteOutline.Sprite == sprite);
            Sprites.Remove(sprite);
            if (associatedOutline != null)
            {
                Outlines.Remove(associatedOutline);
                Owner.Events.InvokeEvent(new RemoveOverlay(associatedOutline));
            }
        }

        private void AddSprite(Sprite sprite)
        {
            Sprites.Add(sprite);
            IOutline newOutline = new SpriteOutline(Origin, sprite, Color.Orange);
            Outlines.Add(newOutline);
            Owner.Events.InvokeEvent(new BeginOverlay(newOutline));
            Renderable.OrderSprites(Sprites);
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");

            new TextUnit("Sprite Edit Mode").Render(r, layer, new System.Drawing.Point(8, 8), 3);
            new TextUnit(new[] { "Edit", "Tweak UV", "Create", "Delete", "Clone" }[Mode], Mode == Delete ? Color.Red : Color.White).Render(r, layer, new System.Drawing.Point(8, 36), 2);
        }
    }

    [Event("start_sprite_mode")]
    class StartSpriteModeEvent : Event
    {
        public Vector2D Origin;
        public List<Sprite> Sprites;
        public OnSelect Callback;
        public bool MultipleSprites;

        public StartSpriteModeEvent(Vector2D origin, List<Sprite> initialValue, OnSelect callback, bool multiple) : base(null)
        {
            Origin = origin;
            Sprites = initialValue;
            Callback = callback;
            MultipleSprites = multiple;
        }

        public delegate void OnSelect(List<Sprite> sprites);
    }
}
