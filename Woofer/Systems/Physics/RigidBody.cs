using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Physics
{
    [Component("rigidbody")]
    class RigidBody : Component
    {
        private CollisionBox[] bounds = new CollisionBox[0];

        [PersistentProperty]
        public CollisionBox[] Bounds
        {
            get => bounds;
            set
            {
                CachedUnion = null;
                bounds = value;
            }
        }

        private CollisionBox CachedUnion = null;

        [HideInInspector]
        public CollisionBox UnionBounds
        {
            get
            {
                if (CachedUnion == null)
                {
                    if (Bounds.Length == 0) return CachedUnion = new CollisionBox(0, 0, 0, 0);
                    else if (Bounds.Length == 1) return CachedUnion = Bounds[0];
                    else
                    {
                        CollisionBox union = Bounds[0];
                        for (int i = 0; i < Bounds.Length; i++)
                        {
                            union = union.Union(Bounds[i]);
                        }
                        return CachedUnion = union;
                    }
                }
                return CachedUnion;
            }
        }

        public RigidBody() : this(new CollisionBox[0])
        {
        }

        public RigidBody(params CollisionBox[] bounds) => Bounds = bounds;
    }
}
