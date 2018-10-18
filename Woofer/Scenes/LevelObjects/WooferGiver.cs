using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Util;
using WooferGame.Systems;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player;
using WooferGame.Systems.Puzzles;
using WooferGame.Systems.Visual;

namespace WooferGame.Scenes.LevelObjects
{
    class WooferGiver : Entity
    {
        public WooferGiver(Vector2D pos)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new SoftBody(new CollisionBox(-4, -4, 8, 8), 0f));
            this.Components.Add(new Renderable(
                new Sprite("woofer", new Rectangle(-10, -9, 20, 18), new Rectangle(268, 9, 20, 18)),
                new Sprite("woofer", new Rectangle(-12, -9, 22, 18), new Rectangle(265, 201, 22, 18)) { DrawMode = DrawMode.Additive }
                ));
            this.Components.Add(new LevelRenderable(1));
            this.Components.Add(new SwitchComponent() { PlayerOnly = true, OneTimeUse = true });

            this.Components.Add(new WooferGiverComponent());
        }
    }
}
