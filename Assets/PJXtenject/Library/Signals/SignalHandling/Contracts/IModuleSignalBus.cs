using System;
using PJXtenject.Library.Enums;
using PJXtenject.Library.Signals.Base;

namespace PJXtenject.Library.Signals.SignalHandling.Contracts
{
    public interface IModuleSignalBus<in TSignalBase, TSignalType> where TSignalBase : SignalBase<TSignalType>
                                                                   where TSignalType : Enum
    {
        public Modules Module { get; }
        public void Subscribe<TSignal>(Action<TSignal> handler) where TSignal : TSignalBase;

        public void Fire<TSignal>(TSignal signal) where TSignal : TSignalBase;

        public void Unsubscribe<TSignal>(Action<TSignal> handler) where TSignal : TSignalBase;
    }
}