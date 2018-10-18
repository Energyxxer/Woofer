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
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace WooferGame.Meta.LevelEditor.Systems.CursorModes
{
    [ComponentSystem("room_builder_mode", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartRoomBuilderModeEvent))]
    class RoomBuilderModeSystem : ComponentSystem
    {
        private EditorCursorSystem CursorSystem = null;
        private bool ModalActive = false;
        private bool ModalVisible = false;
        private StartRoomBuilderModeEvent.OnSelect Callback = null;

        private Vector2D Pivot;

        public override bool ShouldSave => false;
        private bool[,] Map;

        private Dictionary<Point, RectangleOutline> Outlines = new Dictionary<Point, RectangleOutline>();
        private List<IOverlay> Overlays = new List<IOverlay>();
        private RectangleOutline PreviewOutline;

        private byte Mode = Fill;

        private const byte Fill = 0;
        private const byte Clear = 1;

        public override void Update()
        {
            if (CursorSystem == null) CursorSystem = Owner.Systems["editor_cursor"] as EditorCursorSystem;
        }

        /*private void UpdateSelected()
        {
            SelectedBox = null;
            foreach (bool box in Map)
            {
                if ((box.ToRectangle() + Pivot).Contains(CursorSystem.CursorPos))
                {
                    SelectedBox = box;
                    break;
                }
            }
            foreach (IOutline outline in Outlines)
            {
                if (outline is CollisionBoxOutline boxOutline)
                {
                    boxOutline.Color = boxOutline.Box == SelectedBox ? Color.Magenta : Color.DarkMagenta;
                    boxOutline.Thickness = boxOutline.Box == SelectedBox ? 3 : 2;
                }
            }
        }*/

        public override void EventFired(object sender, Event e)
        {
            if (e is StartRoomBuilderModeEvent start)
            {
                Pivot = start.Pivot;
                Callback = start.Callback;
                Map = start.Map;
                Mode = Fill;

                int width = start.Map.GetLength(0);
                int height = start.Map.GetLength(1);

                for (int i = 0; i < width; i++)
                {
                    for(int j = 0; j < height; j++)
                    {
                        RectangleOutline outline = new RectangleOutline(new Rectangle(i * 16, j * 16, 16, 16) + Pivot, Color.FromArgb(128, 255, 255, 255)) { Thickness = 1 };
                        Outlines.Add(new Point(i, j), outline);
                        Owner.Events.InvokeEvent(new BeginOverlay(outline));
                        FormatTile(outline, Map[i, j]);
                    }
                }

                IOutline pivotOutline = new RectangleOutline(new Rectangle(Pivot - new Vector2D(1, 1), new Size(2, 2)), Color.White, 4);
                Overlays.Add(pivotOutline);
                Owner.Events.InvokeEvent(new BeginOverlay(pivotOutline));

                PreviewOutline = new RectangleOutline(new Rectangle(Pivot, new Size(0, 0)), Color.Transparent) { Fill = Color.FromArgb(50, 255, 255, 255) };
                Overlays.Add(PreviewOutline);
                Owner.Events.InvokeEvent(new BeginOverlay(PreviewOutline));
            }
            else if (e is ModalChangeEvent changed)
            {
                //changed.Valid = false;
                //Owner.Events.InvokeEvent(new ForceModalChangeEvent("editor_cursor", null));
                ModalActive = true;
                ModalVisible = true;
                CursorSystem.ModalActive = true;
                CursorSystem.DraggingEnabled = true;
                CursorSystem.SwitchToModal = changed.From;
                //CursorSystem.OutlinesEnabled = true;
            }
            else if (e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = CursorSystem.SwitchToModal;
                ModalActive = false;
                ModalVisible = false;
                CursorSystem.ModalActive = false;
                CursorSystem.DraggingEnabled = false;
                Callback(Map);
                Callback = null;

                foreach (IOutline outline in Outlines.Values)
                {
                    Owner.Events.InvokeEvent(new RemoveOverlay(outline));
                }
                foreach(IOverlay overlay in Overlays)
                {
                    Owner.Events.InvokeEvent(new RemoveOverlay(overlay));
                }
                Outlines.Clear();
                Overlays.Clear();
            }
        }

        public override void Input()
        {
            if (!ModalActive) return;

            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if (inputMap.Pulse.Consume())
            {
                Mode++;
                if (Mode > 1) Mode = 0;
            }

            if (Mode == Fill)
            {
                if(CursorSystem.StoppedDragging)
                {
                    Rectangle area = CursorSystem.SelectionRectangle - Pivot;
                    FillTiles(ConvertToGrid(area), true);
                }
                if (CursorSystem.Dragging)
                {
                    PreviewOutline.Bounds = ConvertFromGrid(ConvertToGrid(CursorSystem.SelectionRectangle - Pivot));
                } else
                {
                    PreviewOutline.Bounds = ConvertFromGrid(ConvertToGrid(new Rectangle(CursorSystem.CursorPos, new Size(0, 0)) - Pivot));
                }
            }
            else if (Mode == Clear)
            {
                if (CursorSystem.StoppedDragging)
                {
                    Rectangle area = CursorSystem.SelectionRectangle - Pivot;
                    FillTiles(ConvertToGrid(area), false);
                }
                if (CursorSystem.Dragging)
                {
                    PreviewOutline.Bounds = ConvertFromGrid(ConvertToGrid(CursorSystem.SelectionRectangle - Pivot));
                }
                else
                {
                    PreviewOutline.Bounds = ConvertFromGrid(ConvertToGrid(new Rectangle(CursorSystem.CursorPos, new Size(0, 0)) - Pivot));
                }
            }
        }

        private Rectangle ConvertToGrid(Rectangle area)
        {
            int minX = (int)Math.Floor(area.X / 16);
            int minY = (int)Math.Floor(area.Y / 16);
            int maxX = (int)Math.Ceiling((area.X + area.Width) / 16);
            int maxY = (int)Math.Ceiling((area.Y + area.Height) / 16);
            minX = Math.Max(minX, 0);
            minY = Math.Max(minY, 0);
            maxX = Math.Min(maxX, Map.GetLength(0));
            maxY = Math.Min(maxY, Map.GetLength(1));
            int width = maxX - minX;
            int height = maxY - minY;
            //if (width == 0) width = 1;
            //if (height == 0) height = 1;
            
            return new Rectangle(minX, minY, width, height);
        }

        private Rectangle ConvertFromGrid(Rectangle grid)
        {
            return new Rectangle(grid.X * 16, grid.Y * 16, grid.Width * 16, grid.Height * 16) + Pivot;
        }

        private void FillTiles(Rectangle rectangle, bool value)
        {
            for(int i = (int)rectangle.X; i < (int)rectangle.X+(int)rectangle.Width; i++)
            {
                for (int j = (int)rectangle.Y; j < (int)rectangle.Y + (int)rectangle.Height; j++)
                {
                    Map[i, j] = value;
                    RectangleOutline outline = Outlines[new Point(i, j)];
                    FormatTile(outline, value);
                }
            }
        }
        
        private void FormatTile(RectangleOutline outline, bool value)
        {
            //outline.Thickness = value ? 2 : 1;
            outline.Fill = value ? Color.FromArgb(55, 55, 60) : Color.Transparent;
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");

            new TextUnit("Room Builder Mode").Render(r, layer, new System.Drawing.Point(8, 8), 3);
            new TextUnit(new[] { "Fill", "Clear" }[Mode], Color.White).Render(r, layer, new System.Drawing.Point(8, 36), 2);
        }
    }

    [Event("start_room_builder_mode")]
    class StartRoomBuilderModeEvent : Event
    {
        public Vector2D Pivot;
        public bool[,] Map;
        public OnSelect Callback;

        public StartRoomBuilderModeEvent(Vector2D pivot, bool[,] initialValue, OnSelect callback) : base(null)
        {
            Pivot = pivot;
            Map = initialValue;
            Callback = callback;
        }

        public delegate void OnSelect(bool[,] map);
    }
}
