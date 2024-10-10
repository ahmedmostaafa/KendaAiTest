using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KabreetGames.UiSystem.Editor.Drawer
{
    [CustomPropertyDrawer(typeof(MenuPanelProperty))]
    public class MenuPanelPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var menuPanelName = property.FindPropertyRelative("displayName");
            var menuPanelType = property.FindPropertyRelative("menuType");
            var panelType = typeof(MenuPanel);
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => panelType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToArray();

            if (allTypes.Length == 0) return;

            var selectedType = allTypes.FirstOrDefault(type => type.Name == menuPanelName.stringValue) ?? allTypes[0];
            var selectedTypeIndex = Array.IndexOf(allTypes, selectedType);
            selectedTypeIndex =
                EditorGUILayout.Popup(label, selectedTypeIndex, allTypes.Select(type => type.Name).ToArray());
            var typeName = allTypes[selectedTypeIndex].Name;
            menuPanelName.stringValue = typeName;
            var assemblyQualifiedName = allTypes[selectedTypeIndex].AssemblyQualifiedName;
            menuPanelType.stringValue = assemblyQualifiedName;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}