using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Enemies
{
    [Component("sentry_ai")]
    class SentryAI : Component
    {
        [PersistentProperty]
        public int FollowDistance = 192;
        
        public bool OnGround = true;

        public int ActionTime = 0;

        public SentryAction Action = SentryAction.Idle;
    }

    enum SentryAction
    {
        Idle,
        Throw,
        Charge,
        Dodge
    }
}
