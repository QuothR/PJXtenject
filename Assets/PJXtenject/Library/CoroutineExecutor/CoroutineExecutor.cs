using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PJXtenject.Library.Enums;
using PJXtenject.Library.Signals.ConcreteSignals;
using PJXtenject.Library.Signals.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

using CoroutineGroups = PJXtenject.Library.CoroutineExecutor.CoroutineExecutorConstants.CoroutineGroups;

namespace PJXtenject.Library.CoroutineExecutor
{
    public class CoroutineExecutor : MonoBehaviour, ICoroutineExecutor
    {
        [Inject] private Dictionary<string, List<Coroutine>> RunningCoroutines { get; init; }
        [Inject] private ISignalLogger SignalLogger { get; init; }

        private void ExecuteCoroutineInternal(IEnumerator routine, string group)
        {
            // if coroutineExecutor is somehow disabled or not active, the StartCoroutine will silently fail and
            // trackCoroutine will track a null
            if (!isActiveAndEnabled)
                throw new InvalidOperationException("Cannot start coroutine: CoroutineExecutor is disabled.");
            
            SignalLogger.LogCustom(Modules.GLOBAL, () => $"Starting coroutine {routine} from {group}");
            
            var coroutine = StartCoroutine(routine);
            TrackCoroutine(coroutine, group);
        }
        public void ExecuteCoroutine(IEnumerator routine, bool persistent = false, float delay = 0f, string group = CoroutineGroups.DefaultCoroutineGroup)
        {
            ExecuteCoroutineInternal(delay == 0f
                    ? routine 
                    : ApplyDelayToCoroutine(routine, delay, group),
                persistent
                    ? CoroutineGroups.PersistentCoroutineGroup
                    : group);
            return;
            
            IEnumerator ApplyDelayToCoroutine(IEnumerator _routine, float _delay, string _group)
            {
                yield return new WaitForSeconds(_delay);
                ExecuteCoroutineInternal(_routine, _group);
            }
        }
        public void ExecuteActionWithDelay(Action callback, float delay, string group)
        {
            ExecuteCoroutineInternal(ApplyDelayToAction(callback, delay), group);
            return;

            IEnumerator ApplyDelayToAction(Action _callback, float _delay)
            {
                yield return new WaitForSeconds(_delay);
                _callback();
            }
        }
        public void WaitForSceneToLoad(string scene, Action onLoad = null)
        {
            ExecuteCoroutineInternal(WaitForSceneLoadCoroutine(onLoad), CoroutineGroups.DefaultCoroutineGroup);
            return;
            
            IEnumerator WaitForSceneLoadCoroutine(Action _onLoad)
            {
                while (SceneManager.GetActiveScene().name != scene)
                    yield return null;
                ExecuteActionWithDelay(_onLoad ??= () => { }, CoroutineExecutorConstants.NewSceneDelay,
                    CoroutineGroups.SceneTransitionCoroutineGroup);
            }
        }
        private void TrackCoroutine(Coroutine coroutine, string group)
        {
            if (!RunningCoroutines.ContainsKey(group))
                RunningCoroutines[group] = new ();
            
            RunningCoroutines[group].Add(coroutine);
        }
        
        public void StopCoroutineGroup(string group)
        {
            if (RunningCoroutines.TryGetValue(group, out var coroutines))
            {
                foreach (var coroutine in coroutines.Where(coroutine => coroutine != null))
                    StopCoroutine(coroutine);

                coroutines.Clear();
                RunningCoroutines.Remove(group);
            }
            else
            {
                Debug.LogError($"Tried stopping coroutine group {group} which was not found");
                throw new InvalidOperationException($"Coroutine group '{group}' not found.");
            }
        }
        public void StopAllNonPersistentCoroutines()
        {
            List<Coroutine> persistentGroup = null;

            foreach (var kv in RunningCoroutines)
            {
                if (kv.Key == CoroutineGroups.PersistentCoroutineGroup)
                {
                    persistentGroup = kv.Value;
                    continue;
                }

                foreach (var coroutine in kv.Value)
                    if (coroutine != null)
                        StopCoroutine(coroutine);
            }

            RunningCoroutines.Clear();

            if (persistentGroup != null)
                RunningCoroutines[CoroutineGroups.PersistentCoroutineGroup] = persistentGroup;
        }
        private new void StopAllCoroutines()
        {
            Debug.LogWarning("Stopping all coroutines");
            foreach (var coroutine in RunningCoroutines
                         .SelectMany(kv => kv.Value.Where(c => c != null)))
                StopCoroutine(coroutine);

            RunningCoroutines.Clear();
        }

        private void OnApplicationQuit() =>
            StopAllCoroutines();
    }
}