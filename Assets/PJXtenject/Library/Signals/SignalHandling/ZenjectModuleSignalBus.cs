using System;
using PJXtenject.Library.Enums;
using PJXtenject.Library.Signals.Base;
using PJXtenject.Library.Signals.SignalHandling.Contracts;
using UnityEngine;
using Zenject;

namespace PJXtenject.Library.Signals.SignalHandling
{
    public abstract class ZenjectModuleSignalBus<TSignalBase, TSignalType> : IModuleSignalBus<TSignalBase, TSignalType>, ILateDisposable
                                                                             where TSignalBase : SignalBase<TSignalType>
                                                                             where TSignalType : Enum
    {
        private readonly SignalBus _signalBus;
        public Modules Module { get; }

        protected ZenjectModuleSignalBus(SignalBus signalBus, Modules module)
        {
            _signalBus = signalBus;
            Module = module;
            Debug.LogWarning($"ZJModule signal bus of {Module} has instance number {_signalBus.InstanceNumber}");
        }
        public void Subscribe<TSignal> (Action<TSignal> handler) where TSignal : TSignalBase
        {
            if (!_signalBus.IsSignalDeclared<TSignal>())
                DeclareSignal<TSignal>();
            _signalBus.Subscribe(handler);
        }
        public void Fire<TSignal>(TSignal signal) where TSignal : TSignalBase =>
            _signalBus.Fire(signal);
        public void Unsubscribe<TSignal>(Action<TSignal> handler) where TSignal : TSignalBase =>
            _signalBus.Unsubscribe(handler);
        private void DeclareSignal<TSignal>() where TSignal : SignalBase<TSignalType> =>
            _signalBus.DeclareSignal<TSignal>();

        public void LateDispose()
        {
            Debug.LogWarning($"zj module sb {_signalBus.InstanceNumber} late disposed.");
            _signalBus.LateDispose();
        }
    }
}