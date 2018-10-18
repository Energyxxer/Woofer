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
using WooferGame.Meta.LevelEditor.Systems.EntityView;

namespace WooferGame.Meta.LevelEditor.Systems.CursorModes
{
    [ComponentSystem("move_cursor_mode", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartMoveModeEvent))]
    class MoveCursorModeSystem : ComponentSystem
    {
        private EditorCursorSystem CursorSystem = null;
        private bool ModalActive = false;
        private StartMoveModeEvent.OnSelect Callback = null;

        public override bool ShouldSave => false;

        public override void Input()
        {
            if (!ModalActive) return;
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if(inputMap.Jump.Consume())
            {
                Callback(CursorSystem.CursorPos, true);
                ModalActive = false;
                Owner.Events.InvokeEvent(new RequestModalChangeEvent(null));
            }
        }

        public override void Update()
        {
            if(CursorSystem == null) CursorSystem = Owner.Systems["editor_cursor"] as EditorCursorSystem;

            if(ModalActive)
            {
                Callback(CursorSystem.CursorPos, false);
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalActive) return;
            var layer = r.GetLayerGraphics("hi_res_overlay");

            new TextUnit("Coordinate Edit Mode").Render(r, layer, new System.Drawing.Point(8, 8), 3);
            new TextUnit($"X: {CursorSystem.CursorPos.X}, Y: {CursorSystem.CursorPos.Y}").Render(r, layer, new System.Drawing.Point(8, 36), 2);

        }

        public override void EventFired(object sender, Event e)
        {
            if(e is StartMoveModeEvent start)
            {
                ModalActive = true;
                Callback = start.Callback;
            }
            else if(e is ModalChangeEvent changed)
            {
                //changed.Valid = false;
                //Owner.Events.InvokeEvent(new ForceModalChangeEvent("editor_cursor", null));
                ModalActive = true;
                CursorSystem.ModalActive = true;
                CursorSystem.SwitchToModal = changed.From;
            }
            else if(e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = CursorSystem.SwitchToModal;
                ModalActive = false;
                CursorSystem.ModalActive = false;
                Callback = null;
            }
        }
    }

    [Event("start_move_mode")]
    class StartMoveModeEvent : Event
    {
        public OnSelect Callback;

        public StartMoveModeEvent(OnSelect callback) : base(null) => Callback = callback;

        public delegate void OnSelect(Vector2D pos, bool definitive);
    }
}
