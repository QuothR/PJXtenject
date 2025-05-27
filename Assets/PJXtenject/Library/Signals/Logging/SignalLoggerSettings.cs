using System.Collections.Generic;
using PJXtenject.Library.Enums;

namespace PJXtenject.Library.Signals.Logging
{
    public abstract class SignalLoggerSettings
    {
        public readonly bool LogToUnityConsole;
        public readonly FileLogging FileLoggingSettings;
        public readonly SignalActionsLogging SignalActionsLoggingSettings;
        public readonly Dictionary<Modules, string> ModuleColorCoding;

        public SignalLoggerSettings(bool logToUnityConsole, FileLogging fileLoggingSettings, SignalActionsLogging signalActionsLoggingSettings, Dictionary<Modules, string> moduleColorCoding)
        {
            LogToUnityConsole = logToUnityConsole;
            FileLoggingSettings = fileLoggingSettings;
            SignalActionsLoggingSettings = signalActionsLoggingSettings;
            ModuleColorCoding = moduleColorCoding;
        }
        public class FileLogging
        {
            public readonly string LogFilePath;
            public readonly bool LogToFile;
            public readonly bool LogToFileTypeOverwrite;

            public FileLogging(bool logToFile, bool logToFileTypeOverwrite, string logFilePath)
            {
                LogFilePath = logFilePath;
                LogToFile = logToFile;
                LogToFileTypeOverwrite = logToFileTypeOverwrite;
            }
        }
        public class SignalActionsLogging
        {
            public readonly bool LogSubscribe;
            public readonly bool LogUnsubscribe;
            public readonly bool LogFire;
            public readonly bool LogFireModeDetailed;
            public readonly bool LogDispose;
            public readonly bool LogCustom;

            public SignalActionsLogging(bool logSubscribe, bool logUnsubscribe, bool logFire, bool logFireModeDetailed, bool logDispose, bool logCustom)
            {
                LogSubscribe = logSubscribe;
                LogUnsubscribe = logUnsubscribe;
                LogFire = logFire;
                LogFireModeDetailed = logFireModeDetailed;
                LogDispose = logDispose;
                LogCustom = logCustom;
            }
        } 
    }
}