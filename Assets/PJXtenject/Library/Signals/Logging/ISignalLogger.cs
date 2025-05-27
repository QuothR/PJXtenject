using System;
using PJXtenject.Library.Enums;

namespace PJXtenject.Library.Signals.Logging
{
    public interface ISignalLogger
    {
        void LogSubscribe(Modules module, Type owner, Type signal, Type handler);
        void LogUnsubscribe(Modules module, Type owner);
        void LogFire(Modules module, ISignalLog signal);
        void LogUnsubscribeAll(Modules module);
        void LogCustom(Modules module, Func<string> logCallback);
    }
}