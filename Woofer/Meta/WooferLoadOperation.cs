using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Saves.Json.Converter.DefaultConverters;
using GameInterfaces.Controller;
using WooferGame.Common;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Generators;
using WooferGame.Systems.Sounds;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;
using WooferGame.Systems.HealthSystems;
using System.Threading;

namespace WooferGame.Meta
{
    class WooferLoadOperation : LoadOperation
    {
        public WooferLoadOperation(IGameController controller, string path) : base(controller, path)
        {
            this.AddConverter(new CollisionBoxConverter());
            this.AddConverter(new ColorConverter());
            this.AddConverter(new ListConverter<CollisionBox>());
            this.AddConverter(new ListConverter<Sound>());
            this.AddConverter(new ListConverter<Sprite>());
            this.AddConverter(new ListConverter<AnimatedSprite>());
            this.AddConverter(new EnumConverter<DrawMode>());
            this.AddConverter(new EnumConverter<DamageFilter>());
            this.AddConverter(new BoolMapConverter());
        }
    }
}
