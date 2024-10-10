using System.Reflection;
using UnityEngine;
using UnityEditor;
namespace KabreetGames.Editor.HierarchyTool
{
    [InitializeOnLoad]

    public class HierarchyWindowEnhancements
    {
        #region Fields - State
 
        private static Color? backgroundColor;
 
        #endregion
 
        #region Properties

        private static Color BackgroundColor
        {
            get
            {
                // check if we haven't cached the ui background color yet
                if (backgroundColor.HasValue) return backgroundColor.Value;
                // make a reflection call to get the current ui background color in the editor
                var method = typeof(EditorGUIUtility).GetMethod("GetDefaultBackgroundColor", BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null) backgroundColor = (Color)method.Invoke(null, null);

                // return the cached value
                return backgroundColor ?? Color.black;
            }
        }
 
        #endregion
 
        #region Constructor
 
        static HierarchyWindowEnhancements()
        {
            // hook in to the rendering of hierarchy items
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }
 
        #endregion
 
        #region Event handlers

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            // grab the associated game object
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!gameObject)
            {
                return;
            }
 
            // check if the game object is a heading/section

            // check for components on the game object
            var allComponents = gameObject.GetComponents<Component>();
            if (allComponents == null || allComponents.Length == 0)
            {
                return;
            }
 
            // find the first with an icon
            Texture firstComponentImage = null;
            foreach (var comp in allComponents)
            {
                // Debug.Log(comp + "   " + comp.GetType().);
                if (comp is Transform or RectTransform or CanvasRenderer or null)//.Assembly.ToString().StartsWith("UnityEngine"))
                {
                    continue;
                }

                firstComponentImage = EditorGUIUtility.ObjectContent(comp, comp.GetType()).image;
                if (firstComponentImage)
                {
                    break;
                }
            }
            if (!firstComponentImage)
            {
                firstComponentImage = EditorGUIUtility.ObjectContent(gameObject, gameObject.GetType()).image;
            }

            if (gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
            {
                EditorGUI.DrawRect(selectionRect, gameObject.activeSelf ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.28f, 0.28f, 0.28f) );
                var imageRect = new Rect(selectionRect);
                const float offset = 5f;
                imageRect.width = imageRect.height;
                imageRect.x += offset;
                GUI.DrawTexture(imageRect, firstComponentImage);
                var labelRect = new Rect(selectionRect);
                labelRect.y -= offset;
                labelRect.height += offset;
                var oldContentColor = GUI.contentColor;
                GUI.contentColor = new Color(1f, 0.7f, 0f);
                EditorGUI.DropShadowLabel(labelRect, gameObject.name.Replace("-", "").ToUpperInvariant());
                GUI.contentColor = oldContentColor;
            }
            else
            {
                // if an icon was found for a component on the game object, render the background color over the existing icon and then render the new one
                var iconRect = new Rect(selectionRect);
                iconRect.width = iconRect.height;
                EditorGUI.DrawRect(iconRect, BackgroundColor);
                GUI.DrawTexture(iconRect, firstComponentImage);
            }
        }
 
        #endregion
    }
}
