using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace WooferGame.Meta.LevelEditor.Systems.EntityOutlines
{
    [Event("begin_outline")]
    class BeginOverlay : Event
    {
        public IOverlay Overlay;

        public BeginOverlay(IOverlay overlay) : base(null)
        {
            this.Overlay = overlay;
        }
    }
    [Event("remove_outline")]
    class RemoveOverlay : Event
    {
        public IOverlay Overlay;

        public RemoveOverlay(IOverlay overlay) : base(null)
        {
            this.Overlay = overlay;
        }
    }
    [Event("clear_outline")]
    class ClearOverlays : Event
    {
        public ClearOverlays() : base(null)
        {
        }
    }
}
