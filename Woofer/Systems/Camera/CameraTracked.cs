using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Camera
{
    [Component("camera_tracked")]
    class CameraTracked : Component
    {
        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Offset { get; set; }

        public CameraTracked()
        {
        }
    }
}
