using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PJXtenject.Library.CoroutineExecutor;
using PJXtenject.Library.Scenario;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using CoroutineGroups = PJXtenject.Library.CoroutineExecutor.CoroutineExecutorConstants.CoroutineGroups;
#nullable enable

namespace PJXtenject.Library.Mediator
{
    public abstract class EntryPointBase : ScriptableObject
    {
        private float LoadingProgress { get; set; }
        protected abstract HashSet<ScenarioType> AcceptedScenarioTypes { get; }
        private List<ScenarioBase> Scenarios { get; set; }
        protected abstract string BaseSceneName { get; }
        protected abstract ScenarioReaderBase ScenarioReader { get; }

        public void InitializeScenariosFromDataFile() =>
            Scenarios = ScenarioReader.ReadScenariosFromJsonFile().ToList();
        public string? GetDestinationSceneForScenario(string scenarioName) =>
            Scenarios.FirstOrDefault(s => s.ScenarioName == scenarioName)?.ScenarioName;                

        //TODO Should preferably also check that there's a single matching scenario
        public bool IsScenarioWithinModule(string scenarioName) => 
            Scenarios.Any(sc => sc.ScenarioName == scenarioName);

        public void TransitionToScenario(string scenarioName, ICoroutineExecutor coroutineExecutor)
        {
            var destinationScenario = Scenarios.Find(s => s.ScenarioName == scenarioName);
            
            if (destinationScenario == null) throw new KeyNotFoundException($"Scenario {scenarioName} not found");
            
            var internalTransition = false;
            
            for (var i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).name == BaseSceneName)
                    internalTransition = true;

            
            var destinationScene = destinationScenario.DestinationScene;

            //Please read this method call very carefully, you might be surprised xdd
            coroutineExecutor.ExecuteCoroutine(internalTransition
                    ? TransitionFromWithinModule(destinationScene)
                    : TransitionFromOutsideModule(new[] { BaseSceneName, destinationScene }),
                false, 0, CoroutineGroups.SceneTransitionCoroutineGroup);

            AssertThenInitialize(destinationScenario, coroutineExecutor);
        }

        private void AssertThenInitialize(ScenarioBase scenario, ICoroutineExecutor coroutineExecutor)
        {
            Assert.IsNotNull(scenario);
            Assert.IsNotNull(coroutineExecutor);
            InitializeModule(scenario);
        }

        protected abstract void InitializeModule(ScenarioBase scenario);

        private IEnumerator TransitionFromWithinModule(string scene)
        {
            Assert.IsTrue(SceneManager.sceneCount == 2);
            
            // scenario scene should be [1], base scene ought to be [0]
            var currentScene = SceneManager.GetSceneAt(1);
            
            var asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                LoadingProgress = Mathf.Clamp01(asyncLoad.progress);
                asyncLoad.allowSceneActivation = asyncLoad.progress == 0.9f;
                yield return null;
            }
            SceneManager.UnloadSceneAsync(currentScene);
        }
        private IEnumerator TransitionFromOutsideModule(IReadOnlyCollection<string> scenes)
        {
            Assert.IsTrue(SceneManager.sceneCount == 2);
            Assert.IsTrue(scenes.Count == 2);
            
            var currentBaseScene = SceneManager.GetSceneAt(0);
            SceneManager.UnloadSceneAsync(currentBaseScene);
            var currentScene = SceneManager.GetSceneAt(1);

            foreach (var asyncLoad in scenes.Select(scene => SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive)))
                while (!asyncLoad.isDone)
                {
                    LoadingProgress = Mathf.Clamp01(asyncLoad.progress);
                    asyncLoad.allowSceneActivation = asyncLoad.progress == 0.9f;
                    yield return null;
                }
            
            SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}