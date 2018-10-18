using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Visual.Animation
{
    [Component("animatable")]
    class AnimationComponent : Component
    {
        [PersistentProperty]
        public List<AnimatedSprite> Animations { get; set; }

        public AnimationComponent()
        {
            Animations = new List<AnimatedSprite>();
        }

        public AnimationComponent(params AnimatedSprite[] animations)
        {
            Animations = animations.ToList();
        }
    }
}
