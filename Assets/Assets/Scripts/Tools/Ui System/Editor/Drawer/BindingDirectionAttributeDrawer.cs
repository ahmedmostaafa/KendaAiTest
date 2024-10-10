using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KabreetGames.UiSystem.Editor.Drawer
{
    [CustomPropertyDrawer(typeof(BindingDirectionAttribute))]
    public class BindingDirectionAttributeDrawer : PropertyDrawer
    {
        private readonly GUIContent directionLabel = new("Direction");
        private GUIContent[] directionOptions;
        private string[] directionOptionValues;
        private int selectedDirectionOption;

        private bool haveDirection = false;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property?.serializedObject == null)
            {
                return;
            }
            if (directionOptionValues == null) RefreshDirectionOptions(property);
            if (!haveDirection)
            {
                property.stringValue = string.Empty;
                return;
            }
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.IndentLevelScope())
            {
                var newSelectedDirection =
                    EditorGUILayout.Popup(directionLabel, selectedDirectionOption, directionOptions);
                if (newSelectedDirection != selectedDirectionOption)
                {
                    if (directionOptionValues != null)
                    {
                        var bindingId = directionOptionValues[newSelectedDirection];
                        property.stringValue = bindingId;
                    }

                    selectedDirectionOption = newSelectedDirection;
                }
            }
                
            if (EditorGUI.EndChangeCheck())
            {
                RefreshDirectionOptions(property);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return haveDirection ? base.GetPropertyHeight(property, label) : 0f;
        }

        private void RefreshDirectionOptions(SerializedProperty property)
        {
            var bindingIdAttribute =
                Attribute.GetCustomAttribute(fieldInfo, typeof(BindingDirectionAttribute)) as BindingDirectionAttribute;
            var actionName = string.Empty;
            if (bindingIdAttribute != null)
            {
                actionName = bindingIdAttribute.actionName;
            }

            var actionProperty = property.serializedObject.FindProperty(actionName);
            var actionReference = (InputActionReference)actionProperty.objectReferenceValue;
            var action = actionReference?.action;
            if (action == null)
            {
                directionOptions = Array.Empty<GUIContent>();
                directionOptionValues = Array.Empty<string>();
                selectedDirectionOption = -1;
                haveDirection = false;
                return;
            }

            haveDirection = action.bindings.Any(b => b.isPartOfComposite);

            if (!haveDirection) return;
            
            directionOptions = action.bindings
                .Where(b => b.isPartOfComposite)
                .Select(b => new GUIContent(b.name))
                .ToArray();
            directionOptionValues = directionOptions
                .Select(b => b.text)
                .ToArray();
            selectedDirectionOption = Array.IndexOf(directionOptionValues, property.stringValue);
        }
    }
}