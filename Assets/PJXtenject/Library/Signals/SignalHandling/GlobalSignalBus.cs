using PJXtenject.Library.Enums;
using PJXtenject.Library.Signals.Base;
using Zenject;

namespace PJXtenject.Library.Signals.SignalHandling
{
    public sealed class GlobalSignalBus : ZenjectModuleSignalBus<GlobalSignal, GlobalSignalTypes>
    {
        public GlobalSignalBus(SignalBus signalBus) : base(signalBus, Modules.GLOBAL)
        {
        }
    }
}