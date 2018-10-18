using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Linking
{
    [Component("followed")]
    class FollowedComponent : Component
    {
        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Offset { get; set; }
    }
}
