using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using GameInterfaces.Input;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor.Systems;
using WooferGame.Meta.LevelEditor.Systems.AnimationView;
using WooferGame.Meta.LevelEditor.Systems.ComponentView;
using WooferGame.Meta.LevelEditor.Systems.CursorModes;
using WooferGame.Meta.LevelEditor.Systems.InputModes;
using WooferGame.Meta.LevelEditor.Systems.ObjectEditor;
using WooferGame.Meta.LevelEditor.Systems.SoundView;
using WooferGame.Scenes;
using WooferGame.Systems.Debug;

namespace WooferGame.Meta.LevelEditor
{
    static class Editor
    {
        internal static InputRepeatingTimeframe MoveTimeframe = new InputRepeatingTimeframe(15, 3);
        //internal static InputHybridTimeframe SelectTimeframe = new InputHybridTimeframe(15, 3);
        //internal static InputHybridTimeframe SelectSecondaryTimeframe = new InputHybridTimeframe(15, 3);

        public static void AttachEditor(Scene scene)
        {
            scene.Controller.Paused = true;

            scene.Systems.Add(new EditorInputSystem());

            scene.Systems.Add(new OutlineSystem());
            scene.Systems.Add(new EditorCursorSystem() { CursorPos = scene.CurrentViewport.Location });
            scene.Systems.Add(new EditorRendering());
            scene.Systems.Add(new EntityListSystem());
            scene.Systems.Add(new SystemListSystem());
            scene.Systems.Add(new EntityViewSystem());

            scene.Systems.Add(new EditorMenuSystem());
            
            scene.Systems.Add(new ObjectViewSystem());
            scene.Systems.Add(new CollisionFaceViewSystem());
            scene.Systems.Add(new SpriteSourceViewSystem());
            scene.Systems.Add(new AnimationViewSystem());
            scene.Systems.Add(new SoundsViewSystem());

            scene.Systems.Add(new MoveCursorModeSystem());
            scene.Systems.Add(new CollisionCursorModeSystem());
            scene.Systems.Add(new EntitySelectionCursorModeSystem());
            scene.Systems.Add(new SpriteCursorModeSystem());
            scene.Systems.Add(new RoomBuilderModeSystem());
            
            scene.Systems.Add(new NumberInputSystem());
            scene.Systems.Add(new TextInputSystem());
            scene.Systems.Add(new EnumerationSelectViewSystem());

            scene.Systems.Add(new ModalFocusSystem());

            scene.Systems.Add(new DebugClipping());
        }
    }

    [ComponentSystem("editor_input", ProcessingCycles.Input, ProcessingFlags.Pause)]
    class EditorInputSystem : ComponentSystem
    {
        public override bool ShouldSave => false;

        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            Editor.MoveTimeframe.RegisterState(inputMap.Movement.Magnitude > 1e-5 ? ButtonState.Pressed : ButtonState.Released);
            /*Editor.SelectTimeframe.RegisterState(inputMap.Jump);
            Editor.SelectSecondaryTimeframe.RegisterState(inputMap.Pulse);*/
        }
    }
}
