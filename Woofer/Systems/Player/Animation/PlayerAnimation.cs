using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Player.Animation
{
    [Component("player_animation")]
    class PlayerAnimation : Component
    {
        [PersistentProperty]
        public bool Initialized = false;

        [PersistentProperty]
        public string SpritesheetName;
        
        [PersistentProperty]
        public bool LastLookedRight = true;

        public double WooferBlinkingTime = 0;

        public PlayerAnimation() : this("char")
        {

        }

        public PlayerAnimation(string spritesheetName) => SpritesheetName = spritesheetName;

        public int WalkAnimationProgress { get; internal set; }
    }
}
