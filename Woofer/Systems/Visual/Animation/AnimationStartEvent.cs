using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Events;

namespace WooferGame.Systems.Visual.Animation
{
    [Event("animation_start")]
    class AnimationStartEvent : Event
    {
        public AnimationComponent Component { get; set; }
        public AnimatedSprite Animation { get; set; }

        public AnimationStartEvent(AnimationComponent sender, AnimatedSprite animation) : base(sender)
        {
            this.Component = sender;
            this.Animation = animation;
        }
    }
}
