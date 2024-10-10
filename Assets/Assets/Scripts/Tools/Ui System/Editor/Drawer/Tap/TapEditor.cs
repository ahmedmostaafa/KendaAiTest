// using UnityEditor;
// using UnityEditor.UI;
//
// namespace KabreetGames.UiSystem.Editor.Drawer
// {
//     [CustomEditor(typeof(Tap), true)]
//     public class TapEditor : SelectableEditor
//     {
//         private SerializedProperty onTapEventSerializedProperty;
//         private SerializedProperty tapGroupSerializedProperty;
//         private SerializedProperty isOnSerializedProperty;
//
//         protected override void OnEnable()
//         {
//             base.OnEnable();
//             isOnSerializedProperty = serializedObject.FindProperty("isOn");
//             tapGroupSerializedProperty = serializedObject.FindProperty("tapGroup");
//             onTapEventSerializedProperty = serializedObject.FindProperty("onValueChanged");
//         }
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             EditorGUILayout.PropertyField(isOnSerializedProperty);
//             EditorGUILayout.PropertyField(tapGroupSerializedProperty);
//             EditorGUILayout.PropertyField(onTapEventSerializedProperty);
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }