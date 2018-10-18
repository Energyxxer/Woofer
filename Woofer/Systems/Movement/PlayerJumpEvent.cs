using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Events;

namespace WooferGame.Systems.Movement
{
    [Event("jump")]
    class PlayerJumpEvent : Event
    {
        public PlayerJumpEvent(PlayerMovementComponent sender) : base(sender)
        {
        }
    }
}
