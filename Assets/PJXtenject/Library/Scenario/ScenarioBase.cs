using System;

namespace PJXtenject.Library.Scenario
{
    [Serializable]
    public abstract class ScenarioBase
    {
        public string ScenarioName { get; }
        public ScenarioType ScenarioType { get; }
        public string DestinationScene { get;}

        protected ScenarioBase(string scenarioName, ScenarioType scenarioType, string destinationScene)
        {
            ScenarioName = scenarioName;
            ScenarioType = scenarioType;
            DestinationScene = destinationScene;
        }
    }
}