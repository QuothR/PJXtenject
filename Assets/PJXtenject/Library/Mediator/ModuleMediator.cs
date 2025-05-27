#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PJXtenject.Library.CoroutineExecutor;
using PJXtenject.Library.Enums;
using PJXtenject.Library.Scenario;
using PJXtenject.Library.Signals.Base;
using PJXtenject.Library.Signals.ConcreteSignals;
using PJXtenject.Library.Signals.SignalHandling.Contracts;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;
using CoroutineGroups = PJXtenject.Library.CoroutineExecutor.CoroutineExecutorConstants.CoroutineGroups;

namespace PJXtenject.Library.Mediator
{
    public class ModuleMediator
    {
        private ISignalManager<GlobalSignal, GlobalSignalTypes> SignalBus;
        
        private ICoroutineExecutor Executor { get; }
        private HashSet<EntryPointBase> EntryPoints { get; }
        private HashSet<ScenarioBase> Scenarios { get; }
        private Dictionary<string, Modules> BaseSceneModuleMapping { get; }
        private Dictionary<string, string> ExitCodeScenarioMapping { get; }
        
        private Modules ActiveModule;

        private ModuleMediator(ICoroutineExecutor executor, ISignalManager<GlobalSignal, GlobalSignalTypes> signalBus, HashSet<EntryPointBase> entryPoints, 
            Dictionary<string, Modules> baseSceneModuleMapping, 
            // Dictionary<string, string> subSceneToBaseSceneMapping, 
            Dictionary<string, string> exitCodeScenarioMapping)
        {
            Executor = executor;
            SignalBus = signalBus;
            EntryPoints = entryPoints;
            BaseSceneModuleMapping = baseSceneModuleMapping;
            
            ExitCodeScenarioMapping = exitCodeScenarioMapping;
            
            SignalBus.Subscribe<ChangeModuleSignal>(this, Handle);
            
            //TODO thanks to the wonders of our moduleMediator/entrypoint system this flow is actually entirely redundant! the entryPoint knows whether you need to transition to a new module or not.
            SignalBus.Subscribe<ChangeSceneInModuleSignal>(this,
                signal => Executor.ExecuteCoroutine(Transition(signal.Scene), false, 0,
                    CoroutineGroups.SceneTransitionCoroutineGroup)
            );
        }

        private void Handle(ChangeModuleSignal minigameSignal)
        {
            var scenarioName = ExitCodeScenarioMapping[minigameSignal.ExitCode] ??
                               throw new
                                   InvalidOperationException(
                                       $"No scenario mapped for exitcode {minigameSignal.ExitCode}");
            
            var entryPoint = EntryPoints.Single(entryPoint => entryPoint.IsScenarioWithinModule(scenarioName));
            
            //stopping all non-persistent coroutines before switching modules
            Executor.StopAllNonPersistentCoroutines();
            
            entryPoint.TransitionToScenario(scenarioName, Executor);
        }
        
        private IEnumerator Transition(string targetSceneName)
        {
            Assert.IsTrue(SceneManager.sceneCount == 2);

            var currentScene = SceneManager.GetSceneAt(1);
            var asyncLoad =  SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            
            if (asyncLoad == null)
                throw new InvalidAsynchronousStateException($"Scene '{targetSceneName}' was not loaded");
            
            while (!asyncLoad.isDone)
            {
                Mathf.Clamp01(asyncLoad.progress);
                asyncLoad.allowSceneActivation = Mathf.Approximately(asyncLoad.progress, 0.9f);
                yield return null;
            }
            SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}