using EntityComponentSystem.Components;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Camera
{
    [Event("camera_query")]
    class CameraLocationQueryEvent : Event
    {
        public Vector2D SuggestedLocation;

        public CameraLocationQueryEvent(Component sender, Vector2D loc) : base(sender)
        {
            this.SuggestedLocation = loc;
        }
    }
}
