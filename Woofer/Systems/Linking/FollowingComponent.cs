using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Linking
{
    [Component("following")]
    class FollowingComponent : Component
    {
        [PersistentProperty]
        public long FollowedID { get; set; }

        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D Offset { get; set; }

        public FollowingComponent() : this(0)
        {
        }

        public FollowingComponent(long followedID) => FollowedID = followedID;

        public FollowingComponent(Entity followed) : this(followed.Id)
        {

        }
    }
}
