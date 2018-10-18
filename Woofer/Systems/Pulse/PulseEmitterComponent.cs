using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Pulse
{
    [Component("pulse_emitter")]
    class PulseEmitterComponent : Component
    {
        [PersistentProperty]
        public Vector2D Direction;
        [PersistentProperty]
        public double Strength;
        [PersistentProperty]
        public double Reach;

        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Offset { get; internal set; }

        public PulseEmitterComponent()
        {
        }

        public PulseEmitterComponent(Vector2D direction, double strength, double reach)
        {
            Direction = direction;
            Strength = strength;
            Reach = reach;
        }

    }
}
