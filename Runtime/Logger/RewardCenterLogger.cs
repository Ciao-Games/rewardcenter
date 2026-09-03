using UnityEngine;

namespace CiaoGames.RewardCenter
{
    public static class RewardCenterLogger
    {
        private const string Prefix = "[RewardCenter] ";
        private static LogLevel _currentLogLevel = LogLevel.Warn;

        internal static void Initialize(LogLevel logLevel)
        {
            _currentLogLevel = logLevel;
        }

        public static void Log(string source, string message)
        {
            if (_currentLogLevel >= LogLevel.Info)
                Debug.Log($"{Prefix}[{source}] {message}");
        }

        public static void LogWarning(string source, string message)
        {
            if (_currentLogLevel >= LogLevel.Warn)
                Debug.LogWarning($"{Prefix}[{source}] {message}");
        }

        public static void LogError(string source, string message)
        {
            if (_currentLogLevel >= LogLevel.Error)
                Debug.LogError($"{Prefix}[{source}] {message}");
        }

        public static LogLevel CurrentLogLevel => _currentLogLevel;
    }
}