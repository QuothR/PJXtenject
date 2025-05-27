using System;
using System.Collections;
using CoroutineGroups = PJXtenject.Library.CoroutineExecutor.CoroutineExecutorConstants.CoroutineGroups;

namespace PJXtenject.Library.CoroutineExecutor
{
    public interface ICoroutineExecutor
    {
        void ExecuteCoroutine(IEnumerator routine, bool persistent = false, float delay = 0f, string group = CoroutineGroups.DefaultCoroutineGroup);
        void ExecuteActionWithDelay(Action callback, float delay, string group = CoroutineGroups.DefaultCoroutineGroup);
        void WaitForSceneToLoad(string scene, Action onLoad = null);
        void StopCoroutineGroup(string group);
        void StopAllNonPersistentCoroutines();
    }
}