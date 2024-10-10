using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KabreetGames.UiSystem.Editor
{
    public class UiWindowStackDisplayWindow : EditorWindow, IHasCustomMenu
    {
        [MenuItem("Kabreet Games/Ui System Manager", priority = -100000000)]
        public static void OpenWindow()
        {
            var window = GetWindow<UiWindowStackDisplayWindow>();
            window.Show();
        }

        private int activeWindow;
        private Vector2 scrollPos;
        private const int PreviewSize = 80;
        private const int Width = 255;
        private const int Height = 255;
        private const string ThumbnailDir = "Assets/Editor/Ui Window Thumbnails";
        private float lastClickTime;
        private const float DoubleClickThreshold = 0.3f;

        private UiMenuData uiData;

        private void OnEnable()
        {
            Load();
            uiData = MenuManager.Data;
            if (uiData != null) return;
            CreateData();
        }

        private void OnDisable()
        {
            Save();
        }

        private void Load()
        {
            activeWindow = EditorPrefs.GetInt("activeWindowKey", 0);
        }

        private void Save()
        {
            EditorPrefs.SetInt("activeWindowKey", activeWindow);
        }

        private void CreateData()
        {
            uiData = CreateInstance<UiMenuData>();
            AssetDatabase.CreateAsset(uiData, "Assets/Resources/UiMenuData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            MenuManager.Data = uiData;

            LoadPreMadePrefabs();
        }

        private void LoadPreMadePrefabs()
        {
            var developmentPath = "Assets/Assets/Scripts/Tools/Ui System/Runtime/Prefabs";
            LoadPrefabsFromPath(developmentPath);

            if (uiData.prefabs.Count == 0)
            {
                var packagePath = "Packages/com.kabreetgames.uinewsystem/Runtime/Prefabs";
                LoadPrefabsFromPath(packagePath);
            }

            Debug.Log("Prefabs loaded: " + uiData.prefabs.Count);
        }

        private void LoadPrefabsFromPath(string path)
        {
            var guids = AssetDatabase.FindAssets("t:GameObject", new[] { path });
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab != null)
                {
                    AddPrefabToList(prefab);
                }
            }
        }

        private void OnGUI()
        {
            DrawLabelHeader();
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = activeWindow != 0;

            if (GUILayout.Button("Prefabs Manager"))
            {
                activeWindow = 0;
            }

            GUI.enabled = activeWindow != 1;

            if (GUILayout.Button("Default Prefabs"))
            {
                activeWindow = 1;
            }

            GUI.enabled = activeWindow != 2;
            if (GUILayout.Button("Ui Menu Stack"))
            {
                activeWindow = 2;
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            switch (activeWindow)
            {
                case 0:
                    HandleDragAndDropToWindow();
                    ShowPrefabData();
                    break;
                case 1:
                    ShowDefaultData();
                    break;
                case 2:
                    ShowStackData();
                    break;
            }
        }

        private void ShowDefaultData()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(100));

            var guiContent = new GUIContent("Menu Prefab:",
                "The Ui Menu that will be displayed when you create a new panel.");
            EditorGUILayout.LabelField(guiContent, EditorStyles.boldLabel, GUILayout.Width(100));
            if (uiData.menuPrefab.prefab == null)
            {
                uiData.menuPrefab.prefab = (GameObject)EditorGUILayout.ObjectField("Menu Prefab",
                    uiData.menuPrefab.prefab, typeof(GameObject), false);
            }
            else
            {
                DrawObjectPreview(uiData.menuPrefab);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            guiContent = new GUIContent("Action Button:",
                "The Ui button that will be spawned automatically when you ask for an action on a action panel.");
            EditorGUILayout.LabelField(guiContent, EditorStyles.boldLabel, GUILayout.Width(100));
            if (uiData.actionButtonPrefab.prefab == null)
            {
                uiData.actionButtonPrefab.prefab = (GameObject)EditorGUILayout.ObjectField("Action Button Prefab",
                    uiData.actionButtonPrefab.prefab,
                    typeof(GameObject), false);
            }
            else
            {
                DrawObjectPreview(uiData.actionButtonPrefab);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void ShowPrefabData()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "box");

            if (uiData.prefabs.Count == 0)
            {
                DrawToolTip();
            }

            var columns = Mathf.FloorToInt(position.width / (PreviewSize + 10));

            for (var i = 0; i < uiData.prefabs.Count; i++)
            {
                if (i % columns == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                DrawObjectPreview(uiData.prefabs[i]);

                if (i % columns == columns - 1 || i == uiData.prefabs.Count - 1)
                {
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            if (AssetPreview.IsLoadingAssetPreviews())
            {
                Repaint();
            }
        }

        private static void DrawToolTip()
        {
            var style = new GUIStyle("box")
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                padding = new RectOffset(5, 5, 5, 5),
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = new Color(0.3f, 0.3f, 0.3f, 1f)
                },
            };
            style.hover = style.normal;
            style.focused = style.normal;
            style.onNormal = style.normal;
            style.onHover = style.hover;
            style.onFocused = style.focused;
            style.onActive = style.onFocused;

            GUILayout.Box("Drop Prefabs Here", style, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        }


        private void DrawObjectPreview(GameObjectData obj)
        {
            if (obj == null || obj.prefab == null) return;
            GUILayout.BeginVertical("window", GUILayout.Width(100), GUILayout.Height(100));

            var preview = LoadPreviewTexture(obj);

            var dragArea = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.Height(100));

            GUI.DrawTexture(dragArea, preview);

            HandleEventsOnTheWindow(obj, dragArea);

            GUILayout.FlexibleSpace();
            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    background = EditorGUIUtility.isProSkin
                        ? MakeColorTexture(new Color(0.28f, 0.28f, 0.28f))
                        : MakeColorTexture(new Color(0.7f, 0.7f, 0.7f))
                }
            };
            var objectName = AddSpacesToCamelCase(obj.prefab.name);
            GUILayout.Label(objectName, labelStyle, GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();
        }

        private void HandleEventsOnTheWindow(GameObjectData obj, Rect dragArea)
        {
            var currentEvent = Event.current;

            if (currentEvent.type == EventType.MouseDown && dragArea.Contains(currentEvent.mousePosition))
            {
                switch (Event.current.button)
                {
                    case 1:
                        ShowContextMenu(obj);
                        Event.current.Use();
                        break;
                    case 0:
                        if (Time.realtimeSinceStartup - lastClickTime < DoubleClickThreshold)
                        {
                            OnSlotDoubleClick(obj);
                        }
                        else
                        {
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = new Object[] { obj.prefab };
                            DragAndDrop.StartDrag("Dragging UI Prefab");
                            currentEvent.Use();
                        }

                        lastClickTime = Time.realtimeSinceStartup;
                        break;
                }
            }

            if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.Repaint)
            {
                EditorGUIUtility.AddCursorRect(dragArea, MouseCursor.MoveArrow);
            }
        }

        private void OnSlotDoubleClick(GameObjectData gameObjectData)
        {
            if (gameObjectData == null || gameObjectData.prefab == null) return;
            Selection.activeObject = gameObjectData.prefab;
            EditorGUIUtility.PingObject(gameObjectData.prefab);
        }

        private static Texture LoadPreviewTexture(GameObjectData obj)
        {
            var preview = AssetPreview.GetAssetPreview(obj.prefab) ?? obj.thumbnail;
            if (preview == null)
            {
                preview = EditorGUIUtility.IconContent("Canvas Icon").image as Texture2D;
            }

            return preview;
        }

        private static Texture2D MakeColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void HandleDragAndDropToWindow()
        {
            var dropArea = new Rect(0, 0, position.width, position.height);
            var currentEvent = Event.current;
            if (currentEvent.type != EventType.DragUpdated && currentEvent.type != EventType.DragPerform) return;
            if (!dropArea.Contains(currentEvent.mousePosition)) return;
            var isDraggedFromProjectWindow = DragAndDrop.objectReferences.Length > 0 &&
                                             DragAndDrop.objectReferences[0] != null &&
                                             !AssetDatabase.Contains(DragAndDrop.objectReferences[0]);

            if (isDraggedFromProjectWindow)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                return;
            }

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type != EventType.DragPerform) return;
            DragAndDrop.AcceptDrag();

            foreach (var draggedObject in DragAndDrop.objectReferences)
            {
                if (draggedObject is GameObject draggedPrefab)
                {
                    AddPrefabToList(draggedPrefab);
                }
            }

            currentEvent.Use();
        }

        private void AddPrefabToList(GameObject prefab)
        {
            uiData.AddPrefab(prefab);
            EditorUtility.SetDirty(uiData);
        }


        private void ShowContextMenu(GameObjectData objectData)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Generate Thumbnail"), false, () => GenerateThumbnail(objectData));
            menu.AddItem(new GUIContent("Delete"), false, () => DeletePrefab(objectData));
            menu.ShowAsContext();
        }

        private void GenerateThumbnail(GameObjectData objectData)
        {
            if (!Directory.Exists(ThumbnailDir))
            {
                Directory.CreateDirectory(ThumbnailDir);
            }

            RenderThumbnail(objectData, ThumbnailDir);
            AssetDatabase.Refresh();
            var filePath = Path.Combine(ThumbnailDir, objectData.prefab.name + ".png");
            var thumbnail = AssetDatabase.LoadAssetAtPath<Texture2D>(
                filePath.Replace("\\", "/")[filePath.IndexOf("Assets/", StringComparison.Ordinal)..]);
            objectData.thumbnail = thumbnail;
            EditorUtility.SetDirty(uiData);
        }

        private void DeletePrefab(GameObjectData prefabToDelete)
        {
            if (!EditorUtility.DisplayDialog("Confirm Delete",
                    $"Are you sure you want to delete '{prefabToDelete.prefab.name}'?", "Yes", "No")) return;
            uiData.prefabs.Remove(prefabToDelete);
            if (prefabToDelete.thumbnail != null)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefabToDelete.thumbnail));
            AssetDatabase.SaveAssets();
        }

        private void DrawLabelHeader()
        {
            var centeredStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.UpperCenter,
                richText = true,
                fontSize = 27,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = Color.cyan
                }
            };
            const float labelWidth = 500f;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Ui Window", centeredStyle, GUILayout.Width(labelWidth),
                GUILayout.Height(35));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void ShowStackData()
        {
            var uiWindowStack = MenuManager.activePanel;
            EditorGUILayout.LabelField(
                uiWindowStack == null ? "No Active Panel" : "Active Panel :   " + uiWindowStack.Name, "",
                EditorStyles.boldLabel);


            var history = MenuManager.HistoryStack;
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("History Stack", EditorStyles.boldLabel);
            foreach (var panel in history)
            {
                EditorGUILayout.LabelField(panel.Name);
            }

            var panels = MenuManager.Panels;
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Panels", EditorStyles.boldLabel);
            foreach (var panel in panels)
            {
                EditorGUILayout.LabelField(panel.Key.Name);
            }
        }

        private static string AddSpacesToCamelCase(string camelCaseString)
        {
            return Regex.Replace(camelCaseString, "(?<=[a-z])([A-Z])", " $1");
        }


        private void GenerateThumbnails()
        {
            if (!Directory.Exists(ThumbnailDir))
            {
                Directory.CreateDirectory(ThumbnailDir);
            }

            foreach (var prefab in uiData.prefabs)
            {
                RenderThumbnail(prefab, ThumbnailDir);
            }

            AssetDatabase.Refresh();

            foreach (var prefab in uiData.prefabs)
            {
                var filePath = Path.Combine(ThumbnailDir, prefab.prefab.name + ".png");
                var thumbnail = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    filePath.Replace("\\", "/")[filePath.IndexOf("Assets/", StringComparison.Ordinal)..]);
                prefab.thumbnail = thumbnail;
                EditorUtility.SetDirty(uiData);
            }
        }

        private void RenderThumbnail(GameObjectData prefab, string thumbnailDir)
        {
            var instance = PrefabUtility.InstantiatePrefab(prefab.prefab) as GameObject;
            if (instance == null) return;
            var canvas = instance.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Canvas").AddComponent<Canvas>();

                instance.transform.SetParent(canvas.transform);
                canvas.gameObject.layer = LayerMask.NameToLayer("UI");
                var rectTransform = canvas.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(960, 540);
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            var camera = new GameObject("ThumbnailCamera").AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 100;
            camera.transform.position = new Vector3(0, 0, -10);
            camera.transform.LookAt(instance.transform);
            camera.backgroundColor = new Color(0.24f, 0.24f, 0.24f);
            camera.clearFlags = CameraClearFlags.Color;
            camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            canvas.worldCamera = camera;

            var renderTexture = new RenderTexture(Width, Height, 24);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;
            var texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            texture.Apply();
            var bytes = texture.EncodeToPNG();
            var filePath = Path.Combine(thumbnailDir, instance.name + ".png");
            File.WriteAllBytes(filePath, bytes);

            camera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(renderTexture);
            DestroyImmediate(camera.gameObject);
            DestroyImmediate(canvas.gameObject);
        }

        private void ClearAllPreviews()
        {
            foreach (var prefab in uiData.prefabs.Where(prefab => prefab.thumbnail != null))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefab.thumbnail));
            }

            uiData.prefabs.Clear();
            EditorUtility.SetDirty(uiData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Generate All Thumbnails"), false, GenerateThumbnails);
            menu.AddItem(new GUIContent("Clear All Previews"), false, ClearAllPreviews);
        }
    }
}