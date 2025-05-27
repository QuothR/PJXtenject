using System;
using PJXtenject.Library.Signals.Base;
using PJXtenject.Library.Signals.ConcreteSignals;
using PJXtenject.Library.Signals.SignalHandling.Contracts;

namespace PJXtenject.Library.Mediator
{
    public abstract class ExitPointBase<TSignalBase, TSignalType> where TSignalBase : SignalBase<TSignalType>
                                                        where TSignalType : Enum
    {
        protected readonly ISignalManager<TSignalBase, TSignalType> ModuleSignalManager;
        protected readonly ISignalManager<GlobalSignal,GlobalSignalTypes> GlobalSignalManager;

        protected ExitPointBase(ISignalManager<TSignalBase, TSignalType> moduleSignalManager, ISignalManager<GlobalSignal, GlobalSignalTypes> globalSignalManager)
        {
            ModuleSignalManager = moduleSignalManager;
            GlobalSignalManager = globalSignalManager;
        }

        protected abstract void OnModuleExit();
        
        //TODO something instead of string here please
        protected void ExitModule(string exitCode)
        {
            OnModuleExit();
            ModuleSignalManager.UnsubscribeAll();
            GlobalSignalManager.Fire<ChangeModuleSignal>(new(exitCode));
        }
    }
}