using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Movement
{
    [Component("player_controlled")]
    class PlayerMovementComponent : Component
    {
        [PersistentProperty]
        public double WalkSpeed { get; set; } = 100;
        [PersistentProperty]
        public double MaxWalkSpeed { get; set; } = 100;
        [PersistentProperty]
        public double AirborneSpeed { get; set; } = 6;
        [PersistentProperty]
        public double MaxAirborneSpeed { get; set; } = 30;
        [PersistentProperty]
        public double JumpSpeed { get; set; } = 192;

        public double CurrentSpeed => OnGround ? WalkSpeed : AirborneSpeed;
        public double CurrentMaxSpeed => OnGround ? MaxWalkSpeed : MaxAirborneSpeed;

        public bool OnGround { get; internal set; } = false;
    }
}