using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using WooferGame.Controller.Commands;
using WooferGame.Input;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.Linking;

namespace WooferGame.Systems.Player
{
    [ComponentSystem("player_orientation_system", ProcessingCycles.Input),
        Watching(typeof(PlayerOrientation))]
    class PlayerOrientationSystem : ComponentSystem
    {
        private readonly double deadzone = 0.1;

        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            Vector2D thumbstick = inputMap.Orientation;

            if(thumbstick.Magnitude >= deadzone)
            {
                foreach(PlayerOrientation po in WatchedComponents)
                {
                    if ((po.Owner.GetComponent<Health>()?.CurrentHealth ?? 1) <= 0) continue;
                    po.Unit = thumbstick.Normalize();
                    po.Owner.Components.Get<FollowedComponent>().Offset = po.OriginOffset + po.Unit * 24;
                }
            }
        }
    }
}
