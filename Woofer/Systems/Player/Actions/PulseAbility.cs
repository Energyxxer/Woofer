using System;

using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Player.Actions
{
    [Component("pulse_ability")]
    class PulseAbility : Component
    {
        [PersistentProperty]
        public double MaxEnergy = 100;

        [PersistentProperty]
        public double EnergyMeter = 100;


        [PersistentProperty]
        public double PulseCost = 20;

        [PersistentProperty]
        public double PulseStrength = 256;


        [PersistentProperty]
        public double MaxRange = 48;


        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Offset { get; set; } = new Vector2D(0, 16);
    }
}
