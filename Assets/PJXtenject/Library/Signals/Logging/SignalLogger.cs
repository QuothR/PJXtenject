using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PJXtenject.Library.Enums;

namespace PJXtenject.Library.Signals.Logging
{
    public class SignalLogger : ISignalLogger
    {
        private readonly SignalLoggerSettings _settings;
        private readonly Action<string> _log;
            
        public SignalLogger(SignalLoggerSettings settings)
        {
            _settings = settings;
            _log = CreateLogAction();
        }
        public void LogSubscribe(Modules module, Type owner, Type signal, Type handler) =>
            LogAction(SignalActions.Subscribe, module, $"{owner.Name} subscribed to {signal.Name}");
        public void LogUnsubscribe(Modules module, Type owner) =>
            LogAction(SignalActions.Unsubscribe, module, $"{owner.Name} unsubscribed from all their signals");
        public void LogFire(Modules module, ISignalLog signal) =>
            LogAction(SignalActions.Fire, module,
                _settings.SignalActionsLoggingSettings.LogFireModeDetailed ? signal.LogDetailed() : signal.LogSimple());
        public void LogUnsubscribeAll(Modules module) =>
            LogAction(SignalActions.Dispose, module, $"Module signal bus has terminated all subscriptions.");
        public void LogCustom(Modules module, Func<string> logCallback) =>
            LogAction(SignalActions.Custom, module, "[CustomLog] " + logCallback());
            
        private void LogAction(SignalActions action, Modules module, string text)
        {
            if (!ShouldLog(action)) 
                return;
            
            var prefix = $"[{GetColoredText("LV3","magenta")}]" +
                         $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.f}]" +
                         $"[{GetColoredPrefix(module)}]" +
                         $"[{action.ToString()}]";
            var message = $"{prefix}{text}";
            _log(message);
        }
        private bool ShouldLog(SignalActions action) => action switch
        {
            SignalActions.Subscribe => _settings.SignalActionsLoggingSettings.LogSubscribe,
            SignalActions.Unsubscribe => _settings.SignalActionsLoggingSettings.LogUnsubscribe,
            SignalActions.Fire => _settings.SignalActionsLoggingSettings.LogFire,
            SignalActions.Dispose => _settings.SignalActionsLoggingSettings.LogDispose,
            SignalActions.Custom => _settings.SignalActionsLoggingSettings.LogCustom,
            _ => false
        };
        private Action<string> CreateLogAction()
        {
            var actions = new List<Action<string>>();

            if (_settings.LogToUnityConsole)
                actions.Add(UnityEngine.Debug.Log);

            if (_settings.FileLoggingSettings.LogToFile)
            {
                var fileMode = _settings.FileLoggingSettings.LogToFileTypeOverwrite ? FileMode.Create : FileMode.Append;
                var writer = new StreamWriter(_settings.FileLoggingSettings.LogFilePath, fileMode == FileMode.Append);
                actions.Add(message => 
                {
                    writer.WriteLine($"[{DateTime.Now}]{message}");
                    writer.Flush();
                });
            }

            return message => actions.ForEach(action => action(message));
        }
        
        //TODO these will not work when writing to file and will just look ugly
        //TODO maybe it works if you view it as an .md file?
        private string GetColoredPrefix(Modules module) => 
            GetColoredText( module.ToString(), _settings.ModuleColorCoding[module]);
        private string GetColoredText(string text, string color) =>
            $"<color={color}>{text}</color>";
    }
}