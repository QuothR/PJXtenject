using System;
using PJXtenject.Library.Signals.Base;
using PJXtenject.Library.Signals.SignalHandling.Contracts;

namespace PJXtenject.Library.Signals.SignalHandling
{
    public class SignalSubscription<TSignal, TSignalBase, TSignalType> : ISignalSubscription
                                                                                  where TSignal : TSignalBase
                                                                                  where TSignalBase : SignalBase<TSignalType>
                                                                                  where TSignalType : Enum
    {
        private readonly IModuleSignalBus<TSignalBase, TSignalType>  _moduleSignalBus;
        private readonly Action<TSignal> _handler;

        public SignalSubscription(IModuleSignalBus<TSignalBase, TSignalType>  moduleSignalBus, Action<TSignal> handler)
        {
            _moduleSignalBus = moduleSignalBus;
            _handler = handler;
            _moduleSignalBus.Subscribe(_handler);
        }
        public void Unsubscribe() =>
            _moduleSignalBus.Unsubscribe(_handler);

        public void Dispose() { }
    }
}