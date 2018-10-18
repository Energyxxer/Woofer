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
using WooferGame.Meta.LevelEditor.Systems.HelperComponents;
using Color = System.Drawing.Color;

namespace WooferGame.Meta.LevelEditor.Systems.CursorModes
{
    [ComponentSystem("entity_selection_cursor_mode", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartEntitySelectionModeEvent))]
    class EntitySelectionCursorModeSystem : ComponentSystem
    {
        private EditorCursorSystem CursorSystem = null;
        private bool ModalActive = false;
        private StartEntitySelectionModeEvent.OnSelect Callback = null;

        private Vector2D Source;
        private bool MultipleAllowed = true;

        public override bool ShouldSave => false;

        private List<long> Selected = new List<long>();

        private Dictionary<long, IOverlay> Lines = new Dictionary<long, IOverlay>();
        private List<IOverlay> Overlays = new List<IOverlay>();
        private EntityOutline HoverOutline;
        private SimpleLine HoverLine;

        private long ForbiddenLink = 0;

        private byte Mode = Add;

        private const byte Add = 0;
        private const byte Remove = 1;
        private const byte Single = 2;

        public override void Update()
        {
            if (CursorSystem == null) CursorSystem = Owner.Systems["editor_cursor"] as EditorCursorSystem;
        }

        public override void Input()
        {
            if (!ModalActive) return;

            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if (MultipleAllowed && inputMap.Pulse.Consume())
            {
                Mode++;
                if (Mode > 1) Mode = 0;
            }

            if(Mode == Add)
            {
                HoverOutline.Color = Color.CadetBlue;
                UpdateHighlighted();
                if (HoverOutline.Id != 0 && inputMap.Jump.Consume())
                {
                    AddLink(HoverOutline.Id);
                }
            } else if(Mode == Remove)
            {
                UpdateHighlighted(limitToSelected: true);
                if(HoverOutline.Id != 0)
                {
                    HoverLine.Color = Color.Red;
                    HoverOutline.Color = Color.Red;
                } else
                {
                    HoverLine.End = HoverLine.Start;
                }
                if (HoverOutline.Id != 0 && inputMap.Jump.Consume())
                {
                    RemoveLink(HoverOutline.Id);
                }
            } else if(Mode == Single)
            {
                UpdateHighlighted();
                if(HoverOutline.Id != 0 && inputMap.Jump.Consume())
                {
                    RemoveLink(Selected[0]);
                    AddLink(HoverOutline.Id);
                }
            }
        }

        private void RemoveLink(long id)
        {
            Selected.Remove(id);
            if (!Lines.ContainsKey(id)) return;

            IOverlay line = Lines[id];
            Owner.Events.InvokeEvent(new RemoveOverlay(line));
            Lines.Remove(id);
        }

        private void AddLink(long id)
        {
            if (Selected.Contains(id)) return;
            Selected.Add(id);

            ILine line = new SimpleLine(Source, EditorUtil.GetSelectionBounds(Owner.Entities[id]).Center, Color.Aqua, 4);
            Lines.Add(id, line);
            Owner.Events.InvokeEvent(new BeginOverlay(line));
        }

        private void UpdateHighlighted(bool limitToSelected = false)
        {
            HoverOutline.Id = 0;
            HoverLine.End = CursorSystem.CursorPos;
            HoverLine.Color = Color.CadetBlue;
            foreach(Entity entity in Owner.Entities)
            {
                if (entity.Components.Has<NoEditorHoverSelect>()) continue;
                if (entity.Id == ForbiddenLink) continue;
                if (limitToSelected && !Selected.Contains(entity.Id)) continue;
                Rectangle bounds = EditorUtil.GetSelectionBounds(entity);
                if (bounds != Rectangle.Empty && bounds.Contains(CursorSystem.CursorPos))
                {
                    HoverOutline.Id = entity.Id;
                    HoverLine.End = bounds.Center;
                    HoverLine.Color = Color.Aqua;
                }
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalActive) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");

            new TextUnit("Activation Link Edit Mode").Render(r, layer, new System.Drawing.Point(8, 8), 3);
            new TextUnit(new[] { "Create", "Remove", "Single" }[Mode], Mode == Remove ? Color.Red : Color.White).Render(r, layer, new System.Drawing.Point(8, 36), 2);
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is StartEntitySelectionModeEvent start)
            {
                Source = start.Source;
                Callback = start.Callback;
                MultipleAllowed = start.MultipleLinks;
                Selected = start.Selected;
                Mode = MultipleAllowed ? Add : Single;
                ForbiddenLink = start.ForbiddenLink;

                foreach (long id in Selected)
                {
                    ILine line = new SimpleLine(Source, EditorUtil.GetSelectionBounds(Owner.Entities[id]).Center, Color.Aqua, 4);
                    Lines.Add(id, line);
                    Owner.Events.InvokeEvent(new BeginOverlay(line));
                }
                HoverOutline = new EntityOutline(Owner, 0) { Thickness = 4 };
                Overlays.Add(HoverOutline);
                Owner.Events.InvokeEvent(new BeginOverlay(HoverOutline));
                HoverLine = new SimpleLine(Source, Source, Color.CadetBlue, 4);
                Overlays.Add(HoverLine);
                Owner.Events.InvokeEvent(new BeginOverlay(HoverLine));

            }
            else if (e is ModalChangeEvent changed)
            {
                ModalActive = true;
                CursorSystem.ModalActive = true;
                CursorSystem.DraggingEnabled = true;
                if (changed.From != "collision_face_view") CursorSystem.SwitchToModal = changed.From;
            }
            else if (e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = CursorSystem.SwitchToModal;
                ModalActive = false;
                CursorSystem.ModalActive = false;
                CursorSystem.DraggingEnabled = false;
                Callback(Selected);
                Callback = null;

                foreach (IOverlay overlay in Lines.Values)
                {
                    Owner.Events.InvokeEvent(new RemoveOverlay(overlay));
                }
                Lines.Clear();
                foreach (IOverlay overlay in Overlays)
                {
                    Owner.Events.InvokeEvent(new RemoveOverlay(overlay));
                }
                Overlays.Clear();
            }
        }
    }

    [Event("start_entity_selection_mode")]
    class StartEntitySelectionModeEvent : Event
    {
        public Vector2D Source;
        public List<long> Selected;
        public OnSelect Callback;
        public bool MultipleLinks;
        public long ForbiddenLink;

        public StartEntitySelectionModeEvent(Vector2D source, List<long> initialValue, OnSelect callback, bool multiple) : base(null)
        {
            Source = source;
            Selected = initialValue;
            Callback = callback;
            MultipleLinks = multiple;
        }

        public delegate void OnSelect(List<long> entities);
    }
}
