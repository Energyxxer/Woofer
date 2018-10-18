using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Linking
{
    [ComponentSystem("following_system", ProcessingCycles.Update),
        Watching(typeof(FollowingComponent))]
    class FollowingSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach(FollowingComponent following in WatchedComponents)
            {
                if(following.FollowedID != 0 && Owner.Entities[following.FollowedID] is Entity followed)
                {
                    Transform followedSp = followed.Components.Get<Transform>();
                    if (followedSp == null) continue;
                    Transform followingSp = following.Owner.Components.Get<Transform>();
                    if (followingSp == null) continue;
                    FollowedComponent fc = followed.Components.Get<FollowedComponent>();
                    
                    following.Owner.Components.Get<Transform>().Position = followedSp.Position + (fc != null ? fc.Offset : Vector2D.Empty) + following.Offset;
                }
            }
        }
    }
}
