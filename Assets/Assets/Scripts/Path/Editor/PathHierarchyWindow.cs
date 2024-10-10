using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace KabreetGames.PathSystem
{
    [InitializeOnLoad]
    public class PathHierarchyWindow
    {
        static PathHierarchyWindow()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }
        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!gameObject || !gameObject.TryGetComponent(out Path path)) return;
            var active = ToolManager.activeToolType == typeof(PathTool);

            var width = selectionRect.height;
            var rect = new Rect(selectionRect.x + selectionRect.width - width, selectionRect.y, width,
                selectionRect.height);
            var c = new GUIContent
            {
                image =
                    EditorGUIUtility.IconContent("d_AvatarPivot").image,

                tooltip = active ? "Hide Path Tool" : "Show Path Tool",
            };
            var style = new GUIStyle(EditorStyles.iconButton);
            if (!GUI.Button(rect, c, style)) return;
            if (active)
            {
                ToolManager.RestorePreviousTool();
            }
            else
            {
                if (Selection.activeGameObject == path.gameObject)
                {
                    SetActiveTool();
                    return;
                }

                Selection.selectionChanged += SetActiveTool;
                Selection.activeGameObject = path.gameObject;
            }
        }

        private static void SetActiveTool()
        {
            Selection.selectionChanged -= SetActiveTool;
            ToolManager.SetActiveTool<PathTool>();
        }
    }
}