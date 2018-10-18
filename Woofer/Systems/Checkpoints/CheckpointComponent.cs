using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Checkpoints
{
    [Component("checkpoint")]
    class CheckpointComponent : Component
    {
        [PersistentProperty]
        public bool Selected { get; set; } = false;
    }
}
