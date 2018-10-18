using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Visual
{
    [Component("level_renderable")]
    class LevelRenderable : Component
    {
        [PersistentProperty]
        public float ZOrder { get; set; }

        public LevelRenderable() : this(0)
        {
        }

        public LevelRenderable(float zOrder) => ZOrder = zOrder;
    }
}
