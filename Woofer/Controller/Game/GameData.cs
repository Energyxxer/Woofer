using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Controller.Game
{
    [PersistentObject]
    public class GameData
    {
        [PersistentProperty]
        public bool HasWoofer = false;
        [PersistentProperty]
        public int MaxEnergy = 100;
        [PersistentProperty]
        public int MaxHealth = 4;
        [PersistentProperty]
        [HideInInspector]
        public string ActiveSceneName = "Tutorial";

        public override string ToString()
        {
            return $"Has Woofer: {HasWoofer}, Max Energy: {MaxEnergy}, Max Health: {MaxHealth}, Active Scene Name: {ActiveSceneName}";
        }
    }
}
