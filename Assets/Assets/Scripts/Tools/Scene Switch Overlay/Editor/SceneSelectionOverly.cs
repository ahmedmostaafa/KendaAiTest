using System.Collections.Generic;
using System.IO;
using System.Linq;
using KabreetGames.Editor;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Tools.Editor
{
    [Overlay(typeof(SceneView), "Scene Selection")]
    public class SceneSelectionOverly : ToolbarOverlay
    {
        private SceneSelectionOverly() : base(SceneDropDownToggle.Id)
        {
        }

        [EditorToolbarElement(Id, typeof(SceneView))]
        private class SceneDropDownToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
        {
            public const string Id = "SceneSelectionOverly/SceneDropDownToggle";

            private static bool IsPackage =>
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                    System.Reflection.Assembly.GetExecutingAssembly()) != null;

            private static string[] Folders => EditorPrefs.GetString(SceneSelectionWindow.SceneSelectionFolderKey)
                .Split(',').Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray();

            private static GameObjectListWrapper selectedPrefabs;

            private static GameObjectListWrapper SelectedPrefabs => selectedPrefabs ??= IsPackage
                ? AssetDatabase.LoadAssetAtPath<GameObjectListWrapper>(
                    "Packages/com.kabreetgames.sceneswitchoverlay/Editor/Data/SceneOverlay_Data.asset")
                : AssetDatabase.LoadAssetAtPath<GameObjectListWrapper>(
                    "Assets/Packages/Scene_Switch_Overlay/Editor/Data/SceneOverlay_Data.asset");
            public EditorWindow containerWindow { get; set; }

            private SceneDropDownToggle()
            {
                text = "";
                tooltip = "Select a scene to load";
                icon = IsPackage
                    ? AssetDatabase.LoadAssetAtPath<Texture2D>(
                        "Packages/com.kabreetgames.sceneswitchoverlay/Editor/Data/Icon/d_UnityLogo.png")
                    : AssetDatabase.LoadAssetAtPath<Texture2D>(
                        "Assets/Packages/Scene_Switch_Overlay/Editor/Data/Icon/d_UnityLogo.png");
                dropdownClicked += ShowSceneMenu;
            }

            private void ShowSceneMenu()
            {
                var menu = new GenericMenu();
                var sceneGuids = EditorBuildSettings.scenes.Select(s => s.guid.ToString()).ToArray();
                var assetScenes = Folders.Length == 0
                    ? new List<string>()
                    : AssetDatabase.FindAssets("t:scene", Folders).Select(AssetDatabase.GUIDToAssetPath)
                        .ToList();
                var currentScene = SceneManager.GetActiveScene();

                foreach (var sceneGuid in sceneGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(sceneGuid);
                    var sceneName = Path.GetFileNameWithoutExtension(path);

                    if (currentScene.name == sceneName && !PrefabMode())
                    {
                        menu.AddDisabledItem(new GUIContent(sceneName));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(sceneName + "/Single"), false,
                            () => OpenScene(currentScene, path, OpenSceneMode.Single));
                        menu.AddItem(new GUIContent(sceneName + "/Additive"), false,
                            () => OpenScene(currentScene, path, OpenSceneMode.Additive));
                    }
                }

                if (assetScenes.Count > 0) menu.AddSeparator("");
                foreach (var scene in assetScenes)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scene);

                    if (currentScene.name == sceneName && !PrefabMode())
                    {
                        menu.AddDisabledItem(new GUIContent(sceneName));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(sceneName + "/Single"), false,
                            () => OpenScene(currentScene, scene, OpenSceneMode.Single));
                        menu.AddItem(new GUIContent(sceneName + "/Additive"), false,
                            () => OpenScene(currentScene, scene, OpenSceneMode.Additive));
                    }
                }

                if (SelectedPrefabs.selectedPrefabs.Count > 0)
                    menu.AddSeparator("");


                foreach (var prefab in SelectedPrefabs.selectedPrefabs.Where(prefab => prefab != null))
                {
                    menu.AddItem(new GUIContent($"Open {prefab.name} Prefab"), PrefabMode(prefab),
                        () => AssetDatabase.OpenAsset(prefab));
                }


                menu.ShowAsContext();
            }

            private static void OpenScene(Scene currentScene, string path, OpenSceneMode mode)
            {
                if (Application.isPlaying)
                {
                    SceneManager.LoadScene(path, mode == OpenSceneMode.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                    return;
                }
                if (currentScene.isDirty)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(path, mode);
                }
                else
                    EditorSceneManager.OpenScene(path, mode);
            }

            private static bool PrefabMode(Object prefab = null)
            {
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefab == null || stage == null)
                {
                    return stage;
                }

                return string.Equals(prefab.name, stage.prefabContentsRoot.name);
            }
        }
    }
}