using System.Collections.Generic;

namespace PJXtenject.Library.Signals.Base
{
    public class GlobalSignal : SignalBase<GlobalSignalTypes>
    {
        protected override string LogData()
        {
            throw new System.NotImplementedException();
        }

        public override HashSet<GlobalSignalTypes> SignalTypes { get; }
    }
}