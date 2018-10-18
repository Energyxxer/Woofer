using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using GameInterfaces.Input;
using WooferGame.Controller;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor.Systems.EntityOutlines;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [ComponentSystem("editor_cursor", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(ForceMoveCursorEvent))]
    class EditorCursorSystem : ComponentSystem
    {
        private Vector2D _cursorPos;
        internal Vector2D CursorPos
        {
            get => BoundToGrid ? new Vector2D(8 * (int)Math.Round(_cursorPos.X / 8), 8 * (int)Math.Round(_cursorPos.Y / 8)) : _cursorPos;
            set => _cursorPos = value;
        }

        internal Vector2D SelectionStart;
        internal Rectangle SelectionRectangle = new Rectangle();
        internal RectangleOutline Outline = null;

        internal bool BoundToGrid = true;
        internal bool ModalActive = true;
        internal bool DraggingEnabled { get; set; } = true;

        public bool StartedDragging { get; private set; } = false;
        public bool StoppedDragging { get; private set; } = false;
        public bool MayDrag { get; set; } = false;
        public bool Dragging { get; set; } = false;

        public string SwitchToModal = "editor_menu";

        public override bool ShouldSave => false;

        public override void Input()
        {
            if (!ModalActive) return;
            if (!Woofer.Controller.Paused) return;

            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            _cursorPos += (inputMap.Run.Pressed ? 4 : 2)*Woofer.Controller.InputManager.ActiveInputMap.Movement;
            if(BoundToGrid && !inputMap.Interact.Pressed)
            {
                _cursorPos = CursorPos;
            }
            BoundToGrid = inputMap.Interact.Pressed;

            if(DraggingEnabled)
            {
                if (!inputMap.Jump.Pressed)
                {
                    SelectionStart = CursorPos;
                    MayDrag = true;
                    StoppedDragging = Dragging;
                    Dragging = false;
                    StartedDragging = false;
                }
                else
                {
                    if (MayDrag)
                    {
                        StoppedDragging = false;
                        StartedDragging = !Dragging;
                        Dragging = true;
                        SelectionRectangle.X = Math.Min(SelectionStart.X, CursorPos.X);
                        SelectionRectangle.Y = Math.Min(SelectionStart.Y, CursorPos.Y);
                        SelectionRectangle.Width = Math.Max(SelectionStart.X, CursorPos.X) - SelectionRectangle.X;
                        SelectionRectangle.Height = Math.Max(SelectionStart.Y, CursorPos.Y) - SelectionRectangle.Y;
                    }
                }
            }
        }

        public override void Update()
        {
            if (!Woofer.Controller.Paused) return;
            Rectangle viewRect = new Rectangle(Owner.CurrentViewport.Location - new Vector2D(LevelRenderingLayer.LevelScreenSize.Width / 2 - 16, LevelRenderingLayer.LevelScreenSize.Height / 2 - 16), new Size(LevelRenderingLayer.LevelScreenSize.Width - 32, LevelRenderingLayer.LevelScreenSize.Height - 32));
            viewRect.Width -= EditorRendering.SidebarWidth * LevelRenderingLayer.LevelScreenSize.Width / 1280;

            if (!viewRect.Contains(CursorPos))
            {
                if (_cursorPos.X < viewRect.Left) Owner.CurrentViewport.X += (_cursorPos.X - viewRect.Left) / 2;
                if (_cursorPos.X > viewRect.Right) Owner.CurrentViewport.X += (_cursorPos.X - viewRect.Right) / 2;
                if (_cursorPos.Y < viewRect.Bottom) Owner.CurrentViewport.Y += (_cursorPos.Y - viewRect.Bottom) / 2;
                if (_cursorPos.Y > viewRect.Top) Owner.CurrentViewport.Y += (_cursorPos.Y - viewRect.Top) / 2;
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!Woofer.Controller.Paused) return;
            var layer = r.GetLayerGraphics("hi_res_overlay");

            CameraView view = Owner.CurrentViewport;

            System.Drawing.Size screenSize = Woofer.Controller.RenderingUnit.ScreenSize;

            Vector2D pos = CursorPos;
            pos -= view.Location;

            float scale = screenSize.Width / (float)LevelRenderingLayer.LevelScreenSize.Width;
            pos *= scale;

            pos.Y *= -1;

            pos.X += screenSize.Width / 2;
            pos.Y += screenSize.Height / 2;

            float size = 11 * 4;

            layer.Draw(r.SpriteManager["editor"], new Rectangle(pos.X - size / 2, pos.Y - size / 2, size, size).ToDrawing(), new System.Drawing.Rectangle(0, 0, 11, 11));
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = SwitchToModal;
                ModalActive = false;
            } else if (e is ModalChangeEvent ce)
            {
                SwitchToModal = ce.From;
                ModalActive = true;
            } else if(e is ForceMoveCursorEvent move)
            {
                CursorPos = move.Position;
                SelectionStart = move.Position;
                MayDrag = false;
                Owner.CurrentViewport.Location = CursorPos;
            }
        }
    }

    [Event("force_move_cursor")]
    public class ForceMoveCursorEvent : Event
    {
        public Vector2D Position;

        public ForceMoveCursorEvent(Vector2D position) : base(null) => Position = position;
    }
}
