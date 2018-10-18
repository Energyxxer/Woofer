using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Saves.Json.Converter.DefaultConverters;
using EntityComponentSystem.Scenes;
using GameInterfaces.Controller;
using WooferGame.Common;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Generators;
using WooferGame.Systems.Sounds;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;
using WooferGame.Systems.HealthSystems;

namespace WooferGame.Meta
{
    class WooferSaveOperation : SaveOperation
    {
        public WooferSaveOperation(Scene scene, string name, string save) : base(scene, Path.Combine(Woofer.DirectoryPath, save, name + ".scn"))
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
