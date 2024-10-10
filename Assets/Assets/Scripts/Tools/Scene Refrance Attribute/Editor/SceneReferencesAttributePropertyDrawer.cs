using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace KabreetGames.SceneReferences.Editor
{
    [CustomPropertyDrawer(typeof(SceneReferenceAttribute), true)]
    public class SceneReferencesAttributePropertyDrawer : PropertyDrawer
    {
        private SceneReferenceAttribute SceneRefAttribute => (SceneReferenceAttribute)attribute;
        private bool Editable => SceneRefAttribute.HasFlags(Flag.Editable);
        private bool ShowInspector => SceneRefAttribute.HasFlags(Flag.ShowInInspector) || SceneRefAttribute.HasFlags(Flag.Editable);

        private bool drawInspector;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                Debug.LogWarning(
                    $"SceneReferenceAttribute can only be used on {nameof(SerializedPropertyType.ObjectReference)} properties" +
                    $" please change {property.serializedObject.targetObject}.{property.propertyPath} to {nameof(SerializedPropertyType.ObjectReference)} or remove the attribute");
                EditorGUI.PropertyField(position, property, label, true);
                drawInspector = true;
                return;
            }

            var anywhere = SceneRefAttribute.Location == ReferencesLocation.Anywhere;
            var hasValue = property.objectReferenceValue != null;
            var isInCollection = fieldInfo.FieldType.IsArray || fieldInfo.FieldType.IsGenericType &&
                fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>);
            drawInspector = ShowInspector || !hasValue || isInCollection || anywhere;
            if (!drawInspector) return;
            var wasEnabled = GUI.enabled;
            GUI.enabled = Editable || anywhere;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return drawInspector ? base.GetPropertyHeight(property, label) : 0;
        }
    }
}