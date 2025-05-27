using System.Collections.Generic;
using PJXtenject.Library.Signals.Base;

namespace PJXtenject.Library.Signals.ConcreteSignals
{
    public sealed class ChangeSceneInModuleSignal : GlobalSignal
    {
        public string Scene { get;}
        
        public ChangeSceneInModuleSignal(string scene)
        {
            Scene = scene;
        }

        protected override string LogData() =>
            $"Changed to scene {Scene}";

        public override HashSet<GlobalSignalTypes> SignalTypes { get; }
    }
}