using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace KabreetGames.UiSystem
{
    public static class DynamicUIMenu
    {
        private const string BaseMenuPath = "GameObject/UI/Add Menu/";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // Delay the menu creation to ensure Unity's menus are initialized
            EditorApplication.delayCall += CreateMenuItems;
        }

        private static void CreateMenuItems()
        {
            var menuPanelType = typeof(IMenuPanel);
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => menuPanelType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToArray();


            if (allTypes.Length == 0)
            {
                Debug.LogWarning("No types implementing IMenuPanel found.");
                return;
            }

            foreach (var type in allTypes)
            {
                var menuPath = $"{BaseMenuPath}{type.Name}";

                // Check if menu item already exists
                if (!Menu.MenuItemExists(menuPath))
                {
                    Menu.AddMenuItem(
                        menuPath,
                        "",
                        false,
                        0,
                        () => CreateUIMenu(type),
                        () => true
                    );
                }
            }
        }

        private static void CreateUIMenu(Type menuType)
        {
            GameObject uiMenu;
            if (MenuManager.Data != null && MenuManager.Data.menuPrefab != null)
            {
                uiMenu = PrefabUtility.InstantiatePrefab(MenuManager.Data.menuPrefab.prefab) as GameObject;
                if (uiMenu != null)
                {
                    uiMenu.name = menuType.Name;
                    uiMenu.AddComponent(menuType);
                }
            }
            else
            {
                uiMenu = new GameObject($"{menuType.Name}", typeof(Canvas), typeof(CanvasScaler),
                    typeof(GraphicRaycaster), typeof(CanvasGroup), menuType);
                var canvas = uiMenu.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            if (uiMenu == null) return;
            var parent = Selection.activeGameObject;
            if (parent != null) uiMenu.transform.SetParent(parent.transform, false);

            Undo.RegisterCreatedObjectUndo(uiMenu, $"Create Menu {menuType.Name}");

            // Find or create a Canvas in the scene
            var eventSystem = Object.FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                //Add EventSystem
                var eventSystemObj = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(eventSystemObj, "Create EventSystem");
            }

            Selection.activeGameObject = uiMenu;
        }

        // [MenuItem("GameObject/UI/Add Menu/Action Menu", false, -1)]
        // public static void AddActionMenu()
        // {
        //     GameObject uiMenu;
        //     if (MenuManager.Data != null && MenuManager.Data.prefabs.Any(x => x.prefab.GetComponent<ModalWindow>()))
        //     {
        //         uiMenu = Object
        //             .Instantiate(MenuManager.Data.prefabs.First(x => x.prefab.GetComponent<ModalWindow>()).prefab,
        //                 Selection.activeGameObject.transform)
        //             .gameObject;
        //     }
        //     else
        //     {
        //         uiMenu = new GameObject($"{nameof(ModalWindow)}", typeof(Canvas), typeof(CanvasScaler),
        //             typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(ModalWindow));
        //         var canvas = uiMenu.GetComponent<Canvas>();
        //         canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //         var parent = Selection.activeGameObject;
        //         if (parent != null) uiMenu.transform.SetParent(parent.transform, false);
        //         var container = new GameObject("Container", typeof(RectTransform));
        //         container.transform.SetParent(uiMenu.transform, false);
        //         var text = new GameObject("question text", typeof(RectTransform), typeof(TextMeshProUGUI));
        //         text.transform.SetParent(uiMenu.transform, false);
        //         Undo.RegisterCreatedObjectUndo(container, "Create Container");
        //     }
        //
        //     Undo.RegisterCreatedObjectUndo(uiMenu, $"Create Menu {nameof(ModalWindow)}");
        //
        //     // Find or create a Canvas in the scene
        //     var eventSystem = Object.FindObjectOfType<EventSystem>();
        //
        //     if (eventSystem == null)
        //     {
        //         //Add EventSystem
        //         var eventSystemObj = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        //         Undo.RegisterCreatedObjectUndo(eventSystemObj, "Create EventSystem");
        //     }
        //
        //     Selection.activeGameObject = uiMenu;
        // }
    }
}