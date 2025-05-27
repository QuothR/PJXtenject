namespace PJXtenject.Library.Signals.Logging
{
    public interface ISignalLog
    {
        public string LogSimple();
        public string LogDetailed();
        public string TimeStamp { get; }
    }
}