using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityComponentSystem.Saves.Json.Objects;
using EntityComponentSystem.Scenes;
using WooferGame.Meta;
using WooferGame.Meta.LevelEditor.Systems;

namespace WooferGame.Controller.Game
{
    public class SaveGame
    {
        public readonly string DirectoryName;
        public string DirectoryPath => Path.Combine(Woofer.DirectoryPath, DirectoryName);
        private Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>();
        private Dictionary<string, Task<Scene>> LoadingTasks = new Dictionary<string, Task<Scene>>();

        public GameData Data { get; private set; } = new GameData();

        public SaveGame(string directoryName) => DirectoryName = directoryName;

        public Scene GetScene(string sceneName)
        {
            return PrepareScene(sceneName).Result;
        }

        public async Task<Scene> PrepareScene(string sceneName)
        {
            if (Scenes.ContainsKey(sceneName) && !Scenes[sceneName].Disposed) return Scenes[sceneName];
            if(LoadingTasks.ContainsKey(sceneName))
            {
                return await LoadingTasks[sceneName];
            }
            WooferLoadOperation loader = new WooferLoadOperation(Woofer.Controller, Path.Combine(DirectoryPath, sceneName + ".scn"));
            Task<Scene> task = Task.Run<Scene>(() => {
                Scene scene = loader.Load();
                Scenes[sceneName] = scene;
                LoadingTasks.Remove(sceneName);
                if(scene != null) Console.WriteLine($"Successfully loaded scene '{scene.Name}'");
                else Console.WriteLine("Could not load scene of name '" + sceneName + "'");
                return scene;
            });

            LoadingTasks[sceneName] = task;
            return await task;
        }

        public void Save()
        {
            string rootPath = DirectoryPath;
            string dataPath = Path.Combine(rootPath, "data.wgf");

            Directory.CreateDirectory(rootPath);

            BinaryWriter writer = new BinaryWriter(new FileStream(dataPath, FileMode.Create));
            TagCompound rootTag = new TagCompound();
            rootTag.AddProperty("data", Data);
            TagIOUtils.Master.Write(rootTag, writer);
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }

        public void Load()
        {
            string rootPath = Path.Combine(Woofer.DirectoryPath, DirectoryName);
            string dataPath = Path.Combine(rootPath, "data.wgf");

            if (File.Exists(dataPath))
            {
                BinaryReader reader = new BinaryReader(new FileStream(dataPath, FileMode.Open));
                TagCompound rootTag = TagIOUtils.Master.Read(reader);
                Data = rootTag.Get<GameData>(TagIOUtils.Master, "data");
                reader.Close();
                reader.Dispose();
            }

        }

        public void CreateNew()
        {
            Directory.CreateDirectory(DirectoryPath);
            foreach(string file in Directory.EnumerateFiles(Woofer.ScenesPath, "*.scn"))
            {
                File.Copy(file, file.Replace(Woofer.ScenesPath, DirectoryPath), true);
            }
            Save();
        }
    }
}
