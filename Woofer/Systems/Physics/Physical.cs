using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Physics
{
    [Component("physical")]
    class Physical : Component
    {
        [HideInInspector]
        public Vector2D Position
        {
            get => Owner.Components.Get<Spatial>().Position;
            set => Owner.Components.Get<Spatial>().Position = value;
        }
        [PersistentProperty]
        public Vector2D Velocity = new Vector2D();

        [PersistentProperty]
        [HideInInspector]
        public Vector2D PreviousPosition { get; set; }
        [PersistentProperty]
        [HideInInspector]
        public Vector2D PreviousVelocity { get; set; }

        [PersistentProperty]
        public double GravityMultiplier { get; set; } = 1;
    }
}
