using EntityComponentSystem.Components;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Pulse
{
    [Event("pulse")]
    class PulseEvent : Event
    {
        public Vector2D Source { get; set; }
        public Vector2D Direction { get; set; } //Can be a zero vector, meaning radial 
        public double Strength { get; set; }
        public double Reach { get; set; }

        public PulseEvent(Component sender, Vector2D source, Vector2D direction, double strength, double reach) : base(sender)
        {
            this.Source = source;
            this.Direction = direction;
            this.Strength = strength;
            this.Reach = reach;
        }

        public override string ToString() => $"Pulse[Source={Source},Direction={Direction},Strength={Strength}]";
    }
}
