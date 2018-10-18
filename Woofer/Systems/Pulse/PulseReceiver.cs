using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Pulse
{
    [Component("pulse_receiver")]
    class PulseReceiver : Component
    {
        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Offset { get; set; }

        [PersistentProperty]
        public double Sensitivity { get; set; } = 1;
    }

    [Component("pulse_receiver_physical")]
    class PulseReceiverPhysical : Component
    {
    }

    [Component("pulse_damaged")]
    class PulseDamaged : Component
    {
    }
}
