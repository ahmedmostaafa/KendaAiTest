using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace KabreetGames.PathSystem
{
    [EditorTool("Path Tool", typeof(Path))]
    public class PathTool : EditorTool
    {
        private bool[] showPositionHandles;
        public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("d_AvatarPivot");

        [Shortcut("Activate Path Tool", typeof(SceneView), KeyCode.P)]
        private static void PathToolShortcut()
        {
            if (Selection.GetFiltered<Path>(SelectionMode.TopLevel).Length > 0)
                ToolManager.SetActiveTool<PathTool>();
        }


        [MenuItem("GameObject/Path")]
        public static void AddPathToScene()
        {
            var path = new GameObject("Path").AddComponent<Path>();
            path.AddPoint(new Vector3(0, 0, 0));
            path.AddPoint(new Vector3(1, 0, 0));
            Selection.activeGameObject = path.gameObject;
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (window is not SceneView sceneView)
                return;

            var sceneCamera = sceneView.camera;

            foreach (var obj in targets)
            {
                if (obj is not Path path) continue;
                path.transform.position = Handles.PositionHandle(path.transform.position, Quaternion.identity);
                var distanceToCamera = Vector3.Distance(sceneCamera.transform.position, path.transform.position);
                var buttonSize = Mathf.Max(0.05f, distanceToCamera * 0.01f);
                if (path.Count == 0)
                {
                    if (Handles.Button(path.transform.position, Quaternion.identity, buttonSize, buttonSize,
                            Handles.SphereHandleCap))
                    {
                        path.AddPoint();
                    }

                    continue;
                }

                if (showPositionHandles == null || showPositionHandles.Length != path.Count)
                {
                    showPositionHandles = new bool[path.Count];
                }

                for (var i = 0; i < path.Count; i++)
                {
                    var pathPoint = path[i];
                    var position = pathPoint.Position;

                    distanceToCamera = Vector3.Distance(sceneCamera.transform.position, position);
                    buttonSize = Mathf.Max(0.05f, distanceToCamera * 0.01f);
                    Handles.color = Color.yellow;
                    if (Handles.Button(position, Quaternion.identity, buttonSize, buttonSize, Handles.SphereHandleCap))
                    {
                        showPositionHandles[i] = !showPositionHandles[i];
                    }

                    if (showPositionHandles.Length > i && showPositionHandles[i])
                    {
                        EditorGUI.BeginChangeCheck();
                        position = Handles.PositionHandle(position, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(obj, "Move Point");

                            pathPoint.Position = position;

                            path[i] = pathPoint;

                            EditorUtility.SetDirty(obj);
                        }

                        Handles.color = Color.white;
                        if (Handles.Button(position + new Vector3(buttonSize, buttonSize, 0), Quaternion.identity,
                                buttonSize / 2f, buttonSize / 2f, Handles.SphereHandleCap))
                        {
                            Undo.RecordObject(obj, "Add Point");
                            path.AddPoint(i + 1);
                            EditorUtility.SetDirty(obj);
                        }

                        Handles.color = Color.red;
                        if (Handles.Button(position + new Vector3(-buttonSize, buttonSize, 0), Quaternion.identity,
                                buttonSize / 2f, buttonSize / 2f, Handles.SphereHandleCap))
                        {
                            Undo.RecordObject(obj, "Delete Point");
                            path.RemovePoint(pathPoint);
                            EditorUtility.SetDirty(obj);
                        }
                    }

                    if (i >= path.Count - 1) continue;
                    var nextPosition = path[i + 1].Position;
                    Handles.color = Color.green;
                    Handles.DrawLine(position, nextPosition);
                }
            }
        }
    }
}