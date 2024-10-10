using KabreetGames.TimeSystem;
using UnityEditor;
using UnityEngine;

namespace Packages.TimeSystem.Editor
{
    public class TimerMonitorWindow : EditorWindow
    {
        [MenuItem("Kabreet Games/Timer Monitor")]
        private static void ShowWindow()
        {
            var window = GetWindow<TimerMonitorWindow>();
            window.titleContent = new GUIContent("Timer Monitor");
            window.Show();
        }

        private bool isShowMoreFoldoutOpen;
        private bool isActiveTimersFoldoutOpen = true;
        private bool isTimersPoolFoldoutOpen;
        private bool isSequencePoolFoldoutOpen;
        private bool isAddTimersFoldoutOpen;
        private bool isRemoveTimersFoldoutOpen;


        private void OnGUI()
        {
            ShowActiveTimers();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(isShowMoreFoldoutOpen ? "Hide more..." : "Show more...", GUILayout.Width(100)))
            {
                isShowMoreFoldoutOpen = !isShowMoreFoldoutOpen;
            }

            GUILayout.EndHorizontal();
            if (isShowMoreFoldoutOpen)
            {
                GUILayout.Space(10);
                ShowTimersPool();
                GUILayout.Space(10);
                ShowSequencePool();
                GUILayout.Space(10);
                AddTimersToAdd();
                GUILayout.Space(10);
                AddTimersToRemove();
            }


            Repaint();
        }

        private void AddTimersToRemove()
        {
            isRemoveTimersFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isRemoveTimersFoldoutOpen,
                "Timers to remove", new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (isRemoveTimersFoldoutOpen)
            {
                var timesToRemove = TimeManager.GetTimesToRemove();
                if (timesToRemove == null || timesToRemove.Count == 0)
                {
                    GUILayout.Label("\tNo timers to remove.");
                }
                else
                {
                    foreach (var timer in timesToRemove)
                    {
                        GUILayout.Label("\t  " + timer);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void AddTimersToAdd()
        {
            isAddTimersFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isAddTimersFoldoutOpen,
                "Timers to add", new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if (isAddTimersFoldoutOpen)
            {
                var timesToAdd = TimeManager.GetTimesToAdd();
                if (timesToAdd == null || timesToAdd.Count == 0)
                {
                    GUILayout.Label("\tNo timers to add.");
                }
                else
                {
                    foreach (var timer in timesToAdd)
                    {
                        var time = timer.GetTime().ToString("0.00");
                        GUILayout.Label("\t  " + timer + " (" + time + " s)");
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void ShowTimersPool()
        {
            isTimersPoolFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isTimersPoolFoldoutOpen,
                "Timers Pool", new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (isTimersPoolFoldoutOpen)
            {
                var timers = TimeManager.GetTimersPool();
                if (timers == null || timers.Count == 0)
                {
                    GUILayout.Label("\tNo timers in pool.");
                }
                else
                {
                    foreach (var timer in timers)
                    {
                        GUILayout.Label("\t  " + timer);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void ShowSequencePool()
        {
            isSequencePoolFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isSequencePoolFoldoutOpen,
                "Sequence Pool", new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (isSequencePoolFoldoutOpen)
            {
                var sequences = TimeManager.GetSequencePool();
                if (sequences == null || sequences.Count == 0)
                {
                    GUILayout.Label("\tNo sequences in pool.");
                }
                else
                {
                    foreach (var sequence in sequences)
                    {
                        GUILayout.Label("\t  " + sequence);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void ShowActiveTimers()
        {
            isActiveTimersFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isActiveTimersFoldoutOpen,
                "Active Timers", new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if (isActiveTimersFoldoutOpen)
            {
                var activeTimers = TimeManager.GetActiveTimers();

                if (activeTimers == null || activeTimers.Count == 0)
                {
                    GUILayout.Label("\t No active timers.");
                }
                else
                {
                    foreach (var timer in activeTimers)
                    {
                        var time = timer.GetTime().ToString("0.00");
                        GUILayout.Label("\t" + timer + "\t(" + time + " s)");
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}