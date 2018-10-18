using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Interaction
{
    [Component("interactable")]
    class Interactable : Component
    {
        [PersistentProperty]
        public long EntityToActivate { get; set; }
        [HideInInspector]
        public bool InRange = false;

        [PersistentProperty]
        [Inspector(InspectorEditType.Offset)]
        public Vector2D IconOffset { get; set; }

        public Interactable() : this(0) { }

        public Interactable(long entityToActivate) => EntityToActivate = entityToActivate;

    }
}
