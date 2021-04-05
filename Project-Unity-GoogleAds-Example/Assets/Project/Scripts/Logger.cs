namespace CTJ
{
    public static class Logger
    {
        private const string SYMBOL = "LOGGER";

        [System.Diagnostics.Conditional(SYMBOL)]
        internal static void Log(object _message) { UnityEngine.Debug.Log(_message); }

        [System.Diagnostics.Conditional(SYMBOL)]
        internal static void LogFormat(string _message, params object[] _args) { UnityEngine.Debug.LogFormat(_message, _args); }

        [System.Diagnostics.Conditional(SYMBOL)]
        internal static void LogWarning(object _message) { UnityEngine.Debug.LogWarning(_message); }

        [System.Diagnostics.Conditional(SYMBOL)]
        internal static void LogWarningFormat(string _message, params object[] _args) { UnityEngine.Debug.LogWarningFormat(_message, _args); }

        [System.Diagnostics.Conditional(SYMBOL)]
        internal static void LogError(object _message) { UnityEngine.Debug.LogError(_message); }

        [System.Diagnostics.Conditional(SYMBOL)]
        internal static void LogErrorFormat(string _message, params object[] _args) { UnityEngine.Debug.LogErrorFormat(_message, _args); }
    }
}