using System;
using System.Collections.Generic;
using System.Linq;
using PJXtenject.Library.Signals.Base;
using PJXtenject.Library.Signals.Logging;
using PJXtenject.Library.Signals.SignalHandling.Contracts;

namespace PJXtenject.Library.Signals.SignalHandling
{
    public class TypeSafeSignalManager<TSignalBase, TSignalType> : ISignalManager<TSignalBase, TSignalType>
        where TSignalBase : SignalBase<TSignalType>
        where TSignalType : Enum
    {
        private readonly IModuleSignalBus<TSignalBase, TSignalType> _moduleSignalBus;
        private readonly ISignalLogger _signalLogger;
        private Dictionary<object, List<ISignalSubscription>> SubscriptionsByObjectMap { get; }

        protected TypeSafeSignalManager(IModuleSignalBus<TSignalBase, TSignalType> moduleSignalBus, 
            Dictionary<object, List<ISignalSubscription>> subscriptionsByObjectMap, ISignalLogger signalLogger)
        {
            _moduleSignalBus = moduleSignalBus;
            _signalLogger = signalLogger;
            SubscriptionsByObjectMap = subscriptionsByObjectMap;
        }

        public void Subscribe<TSignal>(object owner, Action<TSignal> handler, Func<string> loggerCallback = null) where TSignal : TSignalBase =>
                SubscribeInternal(owner, handler, (_, _) =>
                {
                    _signalLogger.LogSubscribe(_moduleSignalBus.Module, owner.GetType(), typeof(TSignal), handler.GetType());
                    if (loggerCallback != null)
                        _signalLogger.LogCustom(_moduleSignalBus.Module, loggerCallback);
                });

        public void Unsubscribe(object owner, Func<string> loggerCallback = null) =>
            UnsubscribeInternal(owner, _ =>
            {
                _signalLogger.LogUnsubscribe(_moduleSignalBus.Module, owner.GetType());
                if (loggerCallback != null)
                    _signalLogger.LogCustom(_moduleSignalBus.Module, loggerCallback);
            });

        public void Fire<TSignal>(TSignal signal, Func<string> loggerCallback = null) where TSignal : TSignalBase
            => FireInternal(signal, _ =>
            {
                _signalLogger.LogFire(_moduleSignalBus.Module, signal);
                if (loggerCallback != null)
                    _signalLogger.LogCustom(_moduleSignalBus.Module, loggerCallback);
            });
        public void UnsubscribeAll() => UnsubscribeAllInternal(() =>
        {
            _signalLogger.LogUnsubscribeAll(_moduleSignalBus.Module);
        });
        protected virtual void SubscribeInternal<TSignal>(object owner, Action<TSignal> handler,
            Action<object, Action<TSignal>> onSubscribe) where TSignal : TSignalBase
        {
            onSubscribe(owner,handler);
            var subscription =  new SignalSubscription<TSignal, TSignalBase, TSignalType>(_moduleSignalBus, handler);
            AddSubscription(owner, subscription);
        }
        protected virtual void UnsubscribeInternal(object owner, Action<object> onUnsubscribe)
        {
            onUnsubscribe(owner);
            if (SubscriptionsByObjectMap.TryGetValue(owner, out var subscriptions))
            {
                foreach (var subscription in subscriptions)
                    subscription.Unsubscribe();
                SubscriptionsByObjectMap.Remove(owner);
            }
        }

        protected virtual void FireInternal<TSignal>(TSignal signal, Action<TSignal> onFire) where TSignal : TSignalBase
        {
            onFire(signal);
            _moduleSignalBus.Fire(signal);
        }

        protected virtual void UnsubscribeAllInternal(Action onDispose)
        {
            onDispose();
            foreach (var subscription in SubscriptionsByObjectMap.Values
                         .SelectMany(subscriptions => subscriptions))
                subscription.Unsubscribe();

            SubscriptionsByObjectMap.Clear();
        }
        protected void AddSubscription(object owner, ISignalSubscription subscription)
        {
            if (!SubscriptionsByObjectMap.TryGetValue(owner, out var subscriptions))
            {
                subscriptions = new();
                SubscriptionsByObjectMap[owner] = subscriptions;
            }
            subscriptions.Add(subscription);
        }

        public void Dispose() { }

    }
}