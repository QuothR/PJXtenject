namespace PJXtenject.Library.CoroutineExecutor
{
    internal static class CoroutineExecutorConstants
    {
        internal static class CoroutineGroups
        {
            public const string SceneTransitionCoroutineGroup = "SceneTransitionCoroutineGroup";
            public const string DefaultCoroutineGroup = "DefaultCoroutineGroup";

            /// <summary>
            /// Use this group for long-lived, application-wide coroutines 
            /// that should only be stopped on final disposal (e.g., during application quit).
            /// </summary>
            public const string PersistentCoroutineGroup = "PersistentCouroutineGroup";
        }

        public const float NewSceneDelay = 1f;
    }
}