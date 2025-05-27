using System.Collections.Generic;

namespace PJXtenject.Library.Scenario
{
    public abstract class ScenarioReaderBase
    {
        protected readonly string ScenariosFilePath;
        protected ScenarioReaderBase(string path) => ScenariosFilePath = path;
        public abstract IEnumerable<ScenarioBase> ReadScenariosFromJsonFile();
    }
}