using System;
using UnityEngine;

namespace MagicWords.Core.Utilities
{
    /// <summary>
    /// Sistema sencillo de eventos para desacoplar la comunicación entre scripts.
    /// </summary>
    public static class EventBus
    {
        public static event Action OnGameStart;
        public static event Action OnGameOver;

        public static void RaiseGameStart()
        {
            OnGameStart?.Invoke();
        }

        public static void RaiseGameOver()
        {
            OnGameOver?.Invoke();
        }
    }
}
