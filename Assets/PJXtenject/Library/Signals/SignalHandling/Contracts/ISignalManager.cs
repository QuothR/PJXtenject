using System;
using PJXtenject.Library.Signals.Base;

namespace PJXtenject.Library.Signals.SignalHandling.Contracts
{
    public interface ISignalManager<in TSignalBase, TSignalType> : IDisposable 
        where TSignalBase : SignalBase<TSignalType>
        where TSignalType : Enum
    {
        void Subscribe<TSignal>(object owner, Action<TSignal> handler, Func<string> loggerCallback = null) where TSignal : TSignalBase;
        public void Unsubscribe(object owner, Func<string> loggerCallback = null);
        public void UnsubscribeAll();
        void Fire<TSignal>(TSignal signal, Func<string> loggerCallback = null) where TSignal : TSignalBase;
    }
    

}