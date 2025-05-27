using System;

namespace PJXtenject.Library.Signals.SignalHandling.Contracts
{
    public interface ISignalSubscription : IDisposable
    {
        void Unsubscribe();
    }
}