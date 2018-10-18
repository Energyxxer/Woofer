using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

using WooferGame.Systems.Physics;

namespace WooferGame.Systems.Checkpoints
{
    [ComponentSystem("checkpoint_system", ProcessingCycles.None),
        Watching(typeof(CheckpointComponent)),
        Listening(typeof(CheckpointRequestEvent), typeof(SoftCollisionEvent))]
    class CheckpointSystem : ComponentSystem
    {

        public override void EventFired(object sender, Event re)
        {
            if(re is CheckpointRequestEvent e)
            {
                Spatial sp = e.EntityToMove.Components.Get<Spatial>();
                Physical ph = e.EntityToMove.Components.Get<Physical>();

                foreach(CheckpointComponent checkpoint in WatchedComponents)
                {
                    if(checkpoint.Selected)
                    {
                        if(sp != null) sp.Position = checkpoint.Owner.Components.Get<Spatial>().Position;
                        if(ph != null) ph.Velocity = Vector2D.Empty;
                        break;
                    }
                }
            } else if(re is SoftCollisionEvent ce &&
                ce.Victim.Components.Has<CheckpointComponent>() &&
                ce.Sender.Owner.Components.Has<CheckpointTrigger>())
            {
                foreach(CheckpointComponent checkpoint in WatchedComponents)
                {
                    checkpoint.Selected = checkpoint.Owner == ce.Victim;
                }
            }
        }
    }
}
