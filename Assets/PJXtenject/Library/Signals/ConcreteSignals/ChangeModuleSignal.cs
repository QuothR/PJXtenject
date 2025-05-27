using System.Collections.Generic;
using PJXtenject.Library.Signals.Base;

namespace PJXtenject.Library.Signals.ConcreteSignals
{
    public sealed class ChangeModuleSignal : GlobalSignal
    {
        public readonly string ExitCode;
        public ChangeModuleSignal(string exitCode) =>
            ExitCode = exitCode;
        protected override string LogData() => $"Changing module from exitpoint {ExitCode}";
        public override HashSet<GlobalSignalTypes> SignalTypes { get; }
    }
}