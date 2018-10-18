using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Environment
{
    [Component("breakable_glass")]
    class BreakableGlassComponent : Component
    {
        [PersistentProperty]
        public int MaxHits = 1;
        [PersistentProperty]
        public int CurrentHits = 0;
    }
}
