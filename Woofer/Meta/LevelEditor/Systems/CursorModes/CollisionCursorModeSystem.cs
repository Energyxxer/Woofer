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

namespace WooferGame.Meta.LevelEditor.Systems.CursorModes
{
    [ComponentSystem("collision_cursor_mode", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartCollisionModeEvent))]
    class CollisionCursorModeSystem : ComponentSystem
    {
        private EditorCursorSystem CursorSystem = null;
        private bool ModalActive = false;
        private bool ModalVisible = false;
        private StartCollisionModeEvent.OnSelect Callback = null;

        private Vector2D Pivot;
        private bool MultipleAllowed = true;

        public override bool ShouldSave => false;
        private List<CollisionBox> Boxes = new List<CollisionBox>();

        private List<IOutline> Outlines = new List<IOutline>();
        private RectangleOutline FaceOutline = null;

        private CollisionBox SelectedBox = null;
        private bool SelectionLocked = false;

        private int DraggingSide = -1;
        private Rectangle PreResizeBounds = Rectangle.Empty;

        private bool Creating = false;

        private byte Mode = Edit;

        private const byte Edit = 0;
        private const byte TweakFaces = 1;
        private const byte Create = 2;
        private const byte Delete = 3;

        public override void Update()
        {
            if (CursorSystem == null) CursorSystem = Owner.Systems["editor_cursor"] as EditorCursorSystem;
        }

        private void UpdateSelected()
        {
            SelectedBox = null;
            foreach(CollisionBox box in Boxes)
            {
                if((box.ToRectangle()+Pivot).Contains(CursorSystem.CursorPos))
                {
                    SelectedBox = box;
                    break;
                }
            }
            foreach(IOutline outline in Outlines)
            {
                if(outline is CollisionBoxOutline boxOutline)
                {
                    boxOutline.Color = boxOutline.Box == SelectedBox ? Color.Magenta : Color.DarkMagenta;
                    boxOutline.Thickness = boxOutline.Box == SelectedBox ? 3 : 2;
                }
            }
        }

        private int UpdateSelectedFace()
        {
            Rectangle selectedBounds = SelectedBox.ToRectangle() + Pivot;

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
            if (e is StartCollisionModeEvent start)
            {
                Pivot = start.Pivot;
                Callback = start.Callback;
                MultipleAllowed = start.MultipleBoxes;
                Boxes = start.Boxes.ToList();
                Mode = Edit;

                foreach (CollisionBox box in Boxes)
                {
                    IOutline outline = new CollisionBoxOutline(Pivot, box, Color.Purple);
                    Outlines.Add(outline);
                    Owner.Events.InvokeEvent(new BeginOverlay(outline));
                }

                IOutline pivotOutline = new RectangleOutline(new Rectangle(Pivot - new Vector2D(1, 1), new Size(2, 2)), Color.White, 4);
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
                if(changed.From != "collision_face_view") CursorSystem.SwitchToModal = changed.From;
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
                Callback(Boxes);
                Callback = null;

                foreach (IOutline outline in Outlines)
                {
                    Owner.Events.InvokeEvent(new RemoveOverlay(outline));
                }
                Outlines.Clear();
                Boxes.Clear();
            }
        }

        public override void Input()
        {
            if (!ModalActive) return;

            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if(inputMap.Pulse.Consume())
            {
                Mode++;
                if ((MultipleAllowed && Mode > 3) || (!MultipleAllowed && Mode > 1)) Mode = 0;
                SelectionLocked = false;
            }

            if(Mode == Edit)
            {
                if (!SelectionLocked)
                {
                    UpdateSelected();
                    if(SelectedBox != null && inputMap.Jump.Consume())
                    {
                        SelectionLocked = true;
                    }
                }
                if(SelectionLocked)
                {
                    if(!CursorSystem.Dragging)
                    {
                        DraggingSide = -1;
                    }
                    FaceOutline.Bounds = new Rectangle(FaceOutline.Bounds?.X ?? 0, FaceOutline.Bounds?.Y ?? 0, 0, 0);

                    Rectangle selectedBounds = SelectedBox.ToRectangle() + Pivot;

                    if (CursorSystem.StartedDragging)
                    {
                        PreResizeBounds = selectedBounds;
                    }

                    int highlightedSide = UpdateSelectedFace();
                    if (highlightedSide != -1 && DraggingSide == -1 && CursorSystem.Dragging) DraggingSide = highlightedSide;

                    if(CursorSystem.Dragging && DraggingSide == -1 && selectedBounds.Contains(CursorSystem.CursorPos))
                    {
                        DraggingSide = 4;
                    }

                    if(CursorSystem.Dragging && DraggingSide != -1 && PreResizeBounds != Rectangle.Empty)
                    {
                        if (DraggingSide == 3) //Left
                        {
                            double prevRight = SelectedBox.Right;
                            SelectedBox.X = (CursorSystem.CursorPos.X - Pivot.X);
                            SelectedBox.Width = prevRight - SelectedBox.X;
                            if(SelectedBox.Width < 0)
                            {
                                double min = Math.Min(SelectedBox.Left, SelectedBox.Right);
                                double max = Math.Max(SelectedBox.Left, SelectedBox.Right);
                                SelectedBox.X = min;
                                SelectedBox.Width = max - min;
                                DraggingSide = 1;
                            }
                        }
                        else if (DraggingSide == 1) //Right
                        {
                            SelectedBox.Width = (CursorSystem.CursorPos.X - SelectedBox.X) - Pivot.X;

                            if (SelectedBox.Width < 0)
                            {
                                double min = Math.Min(SelectedBox.Left, SelectedBox.Right);
                                double max = Math.Max(SelectedBox.Left, SelectedBox.Right);
                                SelectedBox.X = min;
                                SelectedBox.Width = max - min;
                                DraggingSide = 3;
                            }
                        } else if (DraggingSide == 2) //Bottom
                        {
                            double prevTop = SelectedBox.Top;
                            SelectedBox.Y = (CursorSystem.CursorPos.Y - Pivot.Y);
                            SelectedBox.Height = prevTop - SelectedBox.Y;
                            if (SelectedBox.Height < 0)
                            {
                                double min = Math.Min(SelectedBox.Bottom, SelectedBox.Top);
                                double max = Math.Max(SelectedBox.Bottom, SelectedBox.Top);
                                SelectedBox.Y = min;
                                SelectedBox.Height = max - min;
                                DraggingSide = 0;
                            }
                        }
                        else if (DraggingSide == 0) //Top
                        {
                            SelectedBox.Height = (CursorSystem.CursorPos.Y - SelectedBox.Y) - Pivot.Y;

                            if (SelectedBox.Height < 0)
                            {
                                double min = Math.Min(SelectedBox.Bottom, SelectedBox.Top);
                                double max = Math.Max(SelectedBox.Bottom, SelectedBox.Top);
                                SelectedBox.Y = min;
                                SelectedBox.Height = max - min;
                                DraggingSide = 2;
                            }
                        } else if(DraggingSide == 4) //Middle
                        {
                            SelectedBox.X = PreResizeBounds.X + (CursorSystem.CursorPos.X - CursorSystem.SelectionStart.X) - Pivot.X;
                            SelectedBox.Y = PreResizeBounds.Y + (CursorSystem.CursorPos.Y - CursorSystem.SelectionStart.Y) - Pivot.Y;
                        }
                    }
                }
            }
            else if(Mode == Create)
            {
                if(CursorSystem.StartedDragging)
                {
                    CollisionBox newBox = new CollisionBox(CursorSystem.SelectionRectangle-Pivot);
                    Boxes.Add(newBox);
                    IOutline newOutline = new CollisionBoxOutline(Pivot, newBox, Color.Orange);
                    Outlines.Add(newOutline);
                    Owner.Events.InvokeEvent(new BeginOverlay(newOutline));
                    Creating = true;
                }
                if(CursorSystem.Dragging && Creating)
                {
                    CollisionBox newBox = Boxes.Last();
                    newBox.X = CursorSystem.SelectionRectangle.X - Pivot.X;
                    newBox.Y = CursorSystem.SelectionRectangle.Y - Pivot.Y;
                    newBox.Width = CursorSystem.SelectionRectangle.Width;
                    newBox.Height = CursorSystem.SelectionRectangle.Height;
                }
                if(CursorSystem.StoppedDragging && Creating)
                {
                    CollisionBox newBox = Boxes.Last();
                    if(newBox.Area == 0) RemoveBox(newBox);
                    Creating = false;
                }
            } else if(Mode == Delete)
            {
                UpdateSelected();
                if (SelectedBox != null)
                {
                    IOutline associatedOutline = null;
                    foreach(IOutline outline in Outlines)
                    {
                        if(outline is CollisionBoxOutline boxOutline)
                        {
                            if(boxOutline.Box == SelectedBox)
                            {
                                boxOutline.Color = Color.Red;
                                boxOutline.Thickness = 4;
                                associatedOutline = outline;
                            }
                        }
                    }
                    if(inputMap.Jump.Consume())
                    {
                        RemoveBox(SelectedBox);
                    }
                }
            } else if(Mode == TweakFaces)
            {
                if (!SelectionLocked)
                {
                    UpdateSelected();
                    if (SelectedBox != null && inputMap.Jump.Consume())
                    {
                        SelectionLocked = true;
                    }
                }
                if (SelectionLocked)
                {
                    FaceOutline.Bounds = new Rectangle(FaceOutline.Bounds?.X ?? 0, FaceOutline.Bounds?.Y ?? 0, 0, 0);

                    int highlightedFace = UpdateSelectedFace();
                    
                    if(highlightedFace != -1 && inputMap.Jump.Consume())
                    {
                        Owner.Events.InvokeEvent(new CollisionFaceSelectEvent(SelectedBox, highlightedFace));
                        Owner.Events.InvokeEvent(new ForceModalChangeEvent("collision_face_view", null));
                        CursorSystem.ModalActive = false;
                        ModalActive = false;
                    }
                }
            }
        }

        private void RemoveBox(CollisionBox box)
        {
            IOutline associatedOutline = Outlines.Find(o => o is CollisionBoxOutline boxOutline && boxOutline.Box == box);
            Boxes.Remove(box);
            if (associatedOutline != null)
            {
                Outlines.Remove(associatedOutline);
                Owner.Events.InvokeEvent(new RemoveOverlay(associatedOutline));
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");

            new TextUnit("Collision Edit Mode").Render(r, layer, new System.Drawing.Point(8, 8), 3);
            new TextUnit(new[] { "Edit", "Tweak Faces", "Create", "Delete" }[Mode], Mode == Delete ? Color.Red : Color.White).Render(r, layer, new System.Drawing.Point(8, 36), 2);
        }
    }
    
    [Event("start_collision_mode")]
    class StartCollisionModeEvent : Event
    {
        public Vector2D Pivot;
        public CollisionBox[] Boxes;
        public OnSelect Callback;
        public bool MultipleBoxes;

        public StartCollisionModeEvent(Vector2D pivot, CollisionBox[] initialValue, OnSelect callback, bool multiple) : base(null)
        {
            Pivot = pivot;
            Boxes = initialValue;
            Callback = callback;
            MultipleBoxes = multiple;
        }

        public delegate void OnSelect(List<CollisionBox> boxes);
    }
}
