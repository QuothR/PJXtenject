using System;
using System.Collections.Generic;
using PJXtenject.Library.Enums;
using PJXtenject.Library.Signals.Base;
using PJXtenject.Library.Signals.SignalHandling.Contracts;
using Zenject;

namespace PJXtenject.Library.Installers
{
    public class PJXScriptableObjectInstaller : ScriptableObjectInstaller
    {
        protected void InstallModuleSignalManager<TSignalBase, TSignalType, TModuleSignalBus, TSignalManager>(
            Modules module)
            where TSignalBase : SignalBase<TSignalType>
            where TSignalType : Enum
            where TModuleSignalBus : IModuleSignalBus<TSignalBase, TSignalType>
            where TSignalManager : ISignalManager<TSignalBase, TSignalType>

        {
            Container.BindInstance(module).WhenInjectedInto<IModuleSignalBus<TSignalBase, TSignalType>>();

            Container.Bind<IModuleSignalBus<TSignalBase, TSignalType>>()
                .To<TModuleSignalBus>().AsSingle()
                .WhenInjectedInto<ISignalManager<TSignalBase, TSignalType>>().NonLazy();

            Container.BindInstance(new Dictionary<object, List<ISignalSubscription>>()).AsSingle()
                .WhenInjectedInto<ISignalManager<TSignalBase, TSignalType>>().NonLazy();

            Container.Bind<ISignalManager<TSignalBase, TSignalType>>()
                .To<TSignalManager>().AsSingle().NonLazy();
        }
    }
}