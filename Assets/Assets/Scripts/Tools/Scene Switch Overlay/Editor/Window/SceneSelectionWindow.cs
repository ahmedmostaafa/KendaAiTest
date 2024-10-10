using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KabreetGames.Editor
{
    public class SceneSelectionWindow
    {
        public const string SceneSelectionFolderKey = "SceneSelectionFolders";
        public const string PrefabsKey = "SelectedPrefabs";
        private static bool IsPackage => UnityEditor.PackageManager.PackageInfo.FindForAssembly(System.Reflection.Assembly.GetExecutingAssembly()) != null;
        private static GameObjectListWrapper selectedPrefabs;

        private static GameObjectListWrapper SelectedPrefabs => selectedPrefabs ??= IsPackage
            ? AssetDatabase.LoadAssetAtPath<GameObjectListWrapper>(
                "Packages/com.kabreetgames.sceneswitchoverlay/Editor/Data/SceneOverlay_Data.asset")
            : AssetDatabase.LoadAssetAtPath<GameObjectListWrapper>(
                "Assets/Packages/Scene_Switch_Overlay/Editor/Data/SceneOverlay_Data.asset");

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Kabreet Games/Scene Selection", SettingsScope.Project)
            {
                label = "Scene Selection",
                guiHandler = (_) =>
                {
                    var labelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 20,
                    };

                    HandleScenePath(labelStyle);

                    HandlePrefab(labelStyle);
                },
                keywords = new HashSet<string>(new[] { "Folder", "Paths" })
            };

            return provider;
        }
        private static void HandleScenePath(GUIStyle labelStyle)
        {
            GUILayout.Label("List of Folder Paths:", labelStyle);

            GUILayout.Space(30);

            var folderPaths = GetFolderPaths();
            foreach (var folderPath in folderPaths)
            {
                if (string.IsNullOrEmpty(folderPath)) continue;

                GUILayout.BeginHorizontal();
                GUILayout.Label(folderPath, labelStyle, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    RemoveFolder(folderPath);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Folder Path"))
            {
                SaveFolderPaths();
            }

            if (GUILayout.Button("Remove All Paths"))
            {
                EditorPrefs.DeleteKey(SceneSelectionFolderKey);
            }

            GUILayout.EndHorizontal();
        }
        
        
        private static void RemoveFolder(string folderPath)
        {
            var oldFolderPaths = EditorPrefs.GetString(SceneSelectionFolderKey);
            var newFolderPaths = oldFolderPaths.Replace(folderPath, "");
            newFolderPaths = newFolderPaths.TrimEnd(',');
            EditorPrefs.SetString(SceneSelectionFolderKey, newFolderPaths);
        }

        private static IEnumerable<string> GetFolderPaths()
        {
            var folders = EditorPrefs.GetString(SceneSelectionFolderKey);
            return folders.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void SaveFolderPaths()
        {
            var folderPath = GetOpenedFolderPath();
            if (string.IsNullOrEmpty(folderPath)) return;
            var relativePath = Path.GetRelativePath(Application.dataPath, folderPath);
            var addAssetsPath = @"Assets\" + (relativePath.Equals(".") ? string.Empty : relativePath);
            var oldFolderPaths = EditorPrefs.GetString(SceneSelectionFolderKey);
            string newFolderPaths;
            if (string.IsNullOrEmpty(oldFolderPaths))
            {
                newFolderPaths = addAssetsPath;
            }
            else
            {
                newFolderPaths = oldFolderPaths + "," + addAssetsPath;
            }

            EditorPrefs.SetString(SceneSelectionFolderKey, newFolderPaths);
        }

        private static string GetOpenedFolderPath()
        {
            return EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
        }
        private static void HandlePrefab(GUIStyle labelStyle)
        {
            GUILayout.Space(30);

            GUILayout.Label("List of Folder Paths:", labelStyle);

            // Display existing prefabs
            for (var i = 0; i < SelectedPrefabs.selectedPrefabs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SelectedPrefabs.selectedPrefabs[i] = EditorGUILayout.ObjectField(SelectedPrefabs.selectedPrefabs[i], typeof(GameObject), false) as GameObject;

                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    RemovePrefab(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            
            GUILayout.Space(10);

            // Add and remove buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Prefab"))
            {
                AddPrefab();
            }

            if (GUILayout.Button("Remove All Prefabs"))
            {
                RemoveAllPrefabs();
            }
            EditorGUILayout.EndHorizontal();
            EditorUtility.SetDirty(SelectedPrefabs);
        }
        
        private static void AddPrefab()
        {
            SelectedPrefabs.selectedPrefabs.Add(null);
            EditorUtility.SetDirty(SelectedPrefabs);
        }

        private static void RemovePrefab(int index)
        {
            SelectedPrefabs.selectedPrefabs.RemoveAt(index);
            EditorUtility.SetDirty(SelectedPrefabs);
        }

        private static void RemoveAllPrefabs()
        {
            SelectedPrefabs.selectedPrefabs.Clear();
            EditorUtility.SetDirty(SelectedPrefabs);
        }
    }
}