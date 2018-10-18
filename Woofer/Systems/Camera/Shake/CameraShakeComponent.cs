using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Camera.Shake
{
    [Component("camera_shake")]
    class CameraShakeComponent : Component
    {
        [PersistentProperty]
        public Vector2D Motion = new Vector2D(8, 0);
    }
}
