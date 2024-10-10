using UnityEditor;
using UnityEngine;

namespace KabreetGames.ProjectBuilder.Window
{
    public class InstallInnoSetupWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            if (HasOpenInstances<InstallInnoSetupWindow>())
            {
                return;
            }

            var window = CreateInstance<InstallInnoSetupWindow>();
            var windowSize = new Vector2(160, 60);
            window.ShowAsDropDown(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 0, 0), windowSize);
        }

        private void OnGUI()
        {
            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 17
            };
            EditorGUILayout.LabelField("Install Inno Setup ?", style, GUILayout.ExpandWidth(true));

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Ok", GUILayout.Width(60)))
            {
                Application.OpenURL("https://jrsoftware.org/isdl.php");
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Cancel",GUILayout.Width(60)))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}