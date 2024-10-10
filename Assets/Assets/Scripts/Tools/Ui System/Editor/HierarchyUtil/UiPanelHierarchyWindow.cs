using UnityEditor;
using UnityEngine;

namespace KabreetGames.UiSystem.Editor.HierarchyUtil
{
    [InitializeOnLoad]
    public class UiPanelHierarchyWindow
    {
        static UiPanelHierarchyWindow()
        {
            // hook in to the rendering of hierarchy items
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

            EditorApplication.playModeStateChanged -= CleanData;
            EditorApplication.playModeStateChanged += CleanData;
        }

        private static void CleanData(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                MenuManager.CleanUp();
            }
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!gameObject || !gameObject.TryGetComponent(out IMenuPanel uiPanel))
            {
                return;
            }

            var active = uiPanel.Active;
            var width = selectionRect.height;
            var rect = new Rect(selectionRect.x + selectionRect.width - width, selectionRect.y, width,
                selectionRect.height);
            var c = new GUIContent
            {
                image = active
                    ? EditorGUIUtility.IconContent("animationvisibilitytoggleoff").image
                    : EditorGUIUtility.IconContent("animationvisibilitytoggleon").image,
                tooltip = active ? "Hide" : "Show",
            };
            var style = new GUIStyle();
            if (!GUI.Button(rect, c, style)) return;
            if (active)
            {
                uiPanel.Hide();
            }
            else
            {
                uiPanel.Show();
            }


            if (EditorWindow.HasOpenInstances<UiWindowStackDisplayWindow>())
            {
                EditorWindow.GetWindow<UiWindowStackDisplayWindow>()?.Repaint();
            }
        }
    }
}