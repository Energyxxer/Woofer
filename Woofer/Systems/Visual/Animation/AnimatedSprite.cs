using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Visual.Animation
{
    [PersistentObject]
    class AnimatedSprite
    {
        [PersistentProperty]
        public int SpriteIndex { get; set; }
        [PersistentProperty]
        public Rectangle Frame { get; set; }
        [PersistentProperty]
        public Vector2D Step { get; set; }

        [PersistentProperty]
        public int FrameCount { get; set; }
        [PersistentProperty]
        public bool Loop { get; set; }
        [PersistentProperty]
        public int LoopFrame { get; set; }

        [PersistentProperty]
        public int CurrentFrame { get; set; }
        [PersistentProperty]
        public double FrameProgress { get; set; }

        [PersistentProperty]
        public int FrameDuration { get; set; }

        public AnimatedSprite()
        {
            Frame = new Rectangle();
            FrameDuration = 1;
        }

        public AnimatedSprite(int spriteIndex, Rectangle frame, Vector2D step, int frameCount, int frameDuration)
        {
            SpriteIndex = spriteIndex;
            Frame = frame;
            Step = step;
            FrameCount = frameCount;
            FrameDuration = frameDuration;

            CurrentFrame = 0;
            FrameProgress = -1;
            Loop = false;
            LoopFrame = 0;
        }
    }
}
