using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

namespace EntityComponentSystem.Components
{
    [Component("transform")]
    public class Transform : Component
    {
        public Vector2D Position
        {
            get => (Owner.ParentEntity?.GetComponent<Transform>().Position ?? Vector2D.Empty) + RelativePosition;
            set => RelativePosition = value - (Owner.ParentEntity?.GetComponent<Transform>().Position ?? Vector2D.Empty);
        }
        public Vector2D RelativePosition = Vector2D.Empty;
    }
}
