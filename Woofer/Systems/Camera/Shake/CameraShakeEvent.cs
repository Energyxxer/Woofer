using EntityComponentSystem.Components;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Camera.Shake
{
    [Event("camera_shake")]
    class CameraShakeEvent : Event
    {
        public Vector2D Motion { get; set; }

        public CameraShakeEvent(Component sender, Vector2D motion) : base(sender)
        {
            this.Motion = motion;
        }

        public CameraShakeEvent(Component sender, double strength) : this(sender, new Vector2D(0, -strength))
        {
        }
    }
}
