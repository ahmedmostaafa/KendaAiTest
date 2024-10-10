using UnityEngine;
using UnityEngine.LowLevel;
using UnityEditor;
using UnityEngine.PlayerLoop;
using KabreetGames.TimeSystem.Utilities;

namespace KabreetGames.TimeSystem
{
    internal static class TimerBootstrapper
    {
        private static PlayerLoopSystem timeSystem;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            if (!InsertTime<Update>(ref currentPlayerLoop, 0))
            {
                Debug.LogWarning("Time Ticker, IS NOT ADDED. Failed to insert time ticker into player loop");
                return;
            }
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            // PlayerLoopUtilities.PrintPlayerLoop(currentPlayerLoop);
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            static void OnPlayModeStateChanged(PlayModeStateChange state)
            {
                if (state != PlayModeStateChange.ExitingPlayMode) return;
                var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                RemoveTime<Update>(ref currentPlayerLoop);
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                // PlayerLoopUtilities.PrintPlayerLoop(currentPlayerLoop);
                TimeManager.ClearTimers();
            }
#endif
        }


        private static bool InsertTime<T>(ref PlayerLoopSystem loop, int index)
        {
            timeSystem = new PlayerLoopSystem()
            {
                type = typeof(TimeManager),
                updateDelegate = TimeManager.UpdateTimeTic,
                subSystemList = null
            };
            
            return PlayerLoopUtilities.InsertSystem<T>(ref loop, in timeSystem, index); 
        }

        private static void RemoveTime<T>(ref PlayerLoopSystem loop)
        {
            PlayerLoopUtilities.RemoveSystem<T>(ref loop, in timeSystem);
        }
    }
}