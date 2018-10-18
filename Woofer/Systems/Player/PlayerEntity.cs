using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;

using WooferGame.Systems.Camera;
using WooferGame.Systems.Checkpoints;
using WooferGame.Systems.DeathBarrier;
using WooferGame.Systems.Debug;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Linking;
using WooferGame.Systems.Movement;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player.Actions;
using WooferGame.Systems.Player.Animation;
using WooferGame.Systems.Pulse;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Player
{
    class PlayerEntity : Entity
    {
        public PlayerEntity(double x, double y)
        {
            //Physics
            Components.Add(new Spatial(x, y));
            Components.Add(new Physical());
            Components.Add(new SoftBody(new CollisionBox(-6, 0, 12, 16), 8f));
            Components.Add(new PulsePushable());

            //Rendering
            Components.Add(new Renderable());
            Components.Add(new LevelRenderable());

            Components.Add(new PlayerAnimation("char"));

            Components.Add(new PulsePushable());

            //Movement
            Components.Add(new PlayerMovementComponent());
            Components.Add(new PlayerOrientation());
            //Components.Add(new PulseAbility() { Offset = new Vector2D(0, 16) });

            Components.Add(new FollowedComponent());
            
            //Camera
            Components.Add(new CameraTracked() { Offset = new Vector2D(0, 24) });

            //Checkpoints
            Components.Add(new CheckpointTrigger());
            Components.Add(new CheckpointOnBarrierComponent());

            //Interacting
            Components.Add(new InteractingAgent(48));

            Components.Add(new PlayerComponent());

            //DEBUG
            Components.Add(new DebugClippable());
        }
    }
}
