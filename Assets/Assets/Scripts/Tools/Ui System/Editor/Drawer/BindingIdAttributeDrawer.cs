using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace KabreetGames.UiSystem.Editor.Drawer
{
    [CustomPropertyDrawer(typeof(BindingIdAttribute))]
    public class BindingIdAttributeDrawer : PropertyDrawer
    {
        private readonly GUIContent bindingLabel = new("Binding");
        private GUIContent[] bindingOptions;
        private string[] bindingOptionValues;
        private int selectedBindingOption;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property?.serializedObject == null)
            {
                return;
            }

            if (bindingOptionValues == null) RefreshBindingOptions(property);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Action", new GUIStyle("MiniBoldLabel"));
            using (new EditorGUI.IndentLevelScope())
            {
                var newSelectedBinding =
                    EditorGUILayout.Popup(bindingLabel, selectedBindingOption, bindingOptions);
                if (newSelectedBinding != selectedBindingOption)
                {
                    if (bindingOptionValues != null)
                    {
                        var bindingId = bindingOptionValues[newSelectedBinding];
                        property.stringValue = bindingId;
                    }

                    selectedBindingOption = newSelectedBinding;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                RefreshBindingOptions(property);
            }
        }

        private void RefreshBindingOptions(SerializedProperty property)
        {
            var bindingIdAttribute =
                Attribute.GetCustomAttribute(fieldInfo, typeof(BindingIdAttribute)) as BindingIdAttribute;
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
                bindingOptions = Array.Empty<GUIContent>();
                bindingOptionValues = Array.Empty<string>();
                selectedBindingOption = -1;
                return;
            }

            var bindings = action.bindings;
            var bindingCount = bindings.Count;

            bindingOptions = new GUIContent[bindingCount];
            bindingOptionValues = new string[bindingCount];
            selectedBindingOption = -1;

            var currentBindingId = property.stringValue;
            for (var i = 0; i < bindingCount; ++i)
            {
                var binding = bindings[i];
                var bindingId = binding.id.ToString();
                var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

                // If we don't have a binding groups (control schemes), show the device that if there are, for example,
                // there are two bindings with the display string "A", the user can see that one is for the keyboard
                // and the other for the gamepad.
                var displayOptions =
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames |
                    InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
                if (!haveBindingGroups)
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

                // Create display string.
                var displayString = action.GetBindingDisplayString(i, displayOptions);

                // If binding is part of a composite, include the part name.
                if (binding.isPartOfComposite)
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

                // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
                // by instead using a backlash.
                displayString = displayString.Replace('/', '\\');

                // If the binding is part of control schemes, mention them.
                if (haveBindingGroups)
                {
                    var asset = action.actionMap?.asset;
                    if (asset != null)
                    {
                        var controlSchemes = string.Join(", ",
                            binding.groups.Split(InputBinding.Separator)
                                .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                bindingOptions[i] = new GUIContent(displayString);
                bindingOptionValues[i] = bindingId;

                if (currentBindingId == bindingId)
                    selectedBindingOption = i;
            }
        }
    }
}