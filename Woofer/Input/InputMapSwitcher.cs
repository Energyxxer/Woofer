using System;
using System.Collections.Generic;
using GameInterfaces.Controller;
using WooferGame.Systems.Meta;

namespace WooferGame.Input
{
    public class InputMapManager
    {
        private readonly IGameController Owner;
        private readonly List<IInputMap> inputMaps = new List<IInputMap>();
        public IInputMap ActiveInputMap { get; private set; } = null;

        public InputMapManager(IGameController owner)
        {
            this.Owner = owner;
        }

        public void Add(IInputMap inputMap)
        {
            inputMaps.Add(inputMap);
        }

        public void Remove(IInputMap inputMap)
        {
            inputMaps.Remove(inputMap);
        }

        private void SwitchInputMap(IInputMap inputMap)
        {
            ActiveInputMap = inputMap;
            Console.WriteLine("New input: " + inputMap.Name);
            Owner.ActiveScene.Events.InvokeEvent(new SwitchInputMethod());
        }

        public void Update()
        {
            if (ActiveInputMap == null && inputMaps.Count >= 1) SwitchInputMap(inputMaps[0]);

            foreach(IInputMap inputMap in inputMaps)
            {
                if(inputMap != ActiveInputMap && inputMap.IsBeingUsed)
                {
                    SwitchInputMap(inputMap);
                    break;
                }
            }
            ActiveInputMap.ProcessInput();
        }
    }
}
