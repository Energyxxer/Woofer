using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EntityComponentSystem.Scenes;

using WooferGame.Controller.Commands;
using WooferGame.Meta.LevelEditor;
using WooferGame.Meta.LevelEditor.Systems;
using WooferGame.Meta.LevelEditor.Systems.ComponentView;
using WooferGame.Meta.LevelEditor.Systems.InputModes;
using WooferGame.Meta.Loading;

namespace WooferGame.Meta.LevelSelect
{
    class LevelSelect : Scene
    {
        public LevelSelect() : base(Woofer.Controller)
        {
            Systems.Add(new EditorInputSystem());

            Systems.Add(new TextInputSystem());
            Systems.Add(new EnumerationSelectViewSystem());

            Systems.Add(new BackToMainMenuSystem());

            Systems.Add(new ModalFocusSystem() { CurrentSystem = "back_to_menu" });
        }

        private bool Initialized = false;
        protected override void Update()
        {
            if(!Initialized)
            {
                List<string> sceneList = new List<string>();
                foreach (string path in Directory.GetFiles(Woofer.ScenesPath)) {
                    FileInfo file = new FileInfo(path);
                    if (file.Extension == ".scn")
                    {
                        string plainName = file.Name;
                        plainName = plainName.Substring(0, plainName.Length - ".scn".Length);
                        sceneList.Add(plainName);
                    }
                }
                Events.InvokeEvent(new StartEnumSelectEvent("Select Scene", sceneList, l =>
                {
                    Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new LoadingScreen()));
                    Woofer.Controller.CurrentSave = new Controller.Game.SaveGame("scenes");
                    Woofer.Controller.CurrentSave.Load();
                    new Thread(() =>
                    {
                        Scene scene = new WooferLoadOperation(Woofer.Controller, Path.Combine(Woofer.ScenesPath, l + ".scn")).Load();
                        Editor.AttachEditor(scene);
                        Woofer.Controller.CommandFired(new InternalSceneChangeCommand(scene));
                    }).Start();
                }, null));
                Events.InvokeEvent(new ForceModalChangeEvent("enum_select", null));
                Initialized = true;
            }
        }
    }
}
