using System;
using System.Collections.Generic;
using PJXtenject.Library.Signals.Logging;

namespace PJXtenject.Library.Signals.Base
{
    public abstract class SignalBase<T> : ISignalLog where T: Enum
    {
        protected abstract string LogData();
        public abstract HashSet<T> SignalTypes { get; }
        public string TimeStamp => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.f}";
        public string LogDetailed() => $"{LogSimple()} {LogData()}";
        public string LogSimple() => $"[{GetType().Name}]";
    }
}
