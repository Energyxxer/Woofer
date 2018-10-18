using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Timer;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;

namespace WooferGame.Systems.Enemies.Boss
{
    class BossLaser : Entity
    {
        public BossLaser(Vector2D pos, bool big)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Physical() { GravityMultiplier = 0 });
            Components.Add(new SoftBody(new CollisionBox(big ? -22 : -11, big ? -212 : -180, big ? 44 : 22, big ? 212 : 180), 1f) { Movable = false });
            Components.Add(new LevelRenderable(1));
            Rectangle source = big ? new Rectangle(176, 180, 88, 212) : new Rectangle(176, 0, 48, 180);
            Components.Add(new Renderable(new Sprite("boss", big ? new Rectangle(-44, -212, 88, 212) : new Rectangle(-24, -180, 48, 180), source)));
            Components.Add(new AnimationComponent(new AnimatedSprite(0, source, new Vector2D(512, 0), 2, 10) { Loop = true }));
            Components.Add(new TimerComponent(2));
            Components.Add(new BossLaserComponent(big));
        }
    }

    [Component("boss_laser")]
    class BossLaserComponent : Component
    {
        [PersistentProperty]
        public bool Big { get; set; } = false;

        public BossLaserComponent()
        {
        }

        public BossLaserComponent(bool big) => Big = big;
    }
}
