using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace KabreetGames.ProjectBuilder.Window
{
    public class InstallerResourceSetup : EditorWindow
    {
        private Texture2D setupIcon;
        private Texture2D wizardImage;
        private Texture2D wizardSmall;

        private const string SetUpIconDescription =
            "Specifies a custom program icon to use for Setup/Uninstall. The file must be located in your installation's source directory when running the compiler, unless a fully qualified pathname is specified or the pathname is prefixed by \"compiler:\", in which case it looks for the file in the compiler directory.\n\nIt is recommended to include at least the following sizes in your icon: 16x16, 32x32, 48x48, 64x64, and 256x256.\n\nIf this directive is not specified or is blank, a built-in icon supporting the above sizes will be used.";

        private const string WizardImageDescription =
            "Specifies the name(s) of the bitmap file(s) to display on the left side of the wizard. Wildcards are supported and the files(s) must be located in your installation's source directory when running the compiler, unless a fully qualified pathname is specified or the pathname is prefixed by \"compiler:\", in which case it looks for the file in the compiler directory.\n\n256-color bitmaps may not display correctly in 256-color mode, since it does not handle palettes.\n\nWhen multiple files are specified, Setup will automatically select the one which best matches the system's DPI setting. The recommended size of the bitmap per DPI setting is:\n100%\t164x314\n125%\t192x386\n150%\t246x459\n175%\t273x556\n200%\t328x604\n225%\t355x700\n250%\t410x797\n";

        private const string WizardSmallDescription =
            "Specifies the name(s) of the bitmap file(s) to display in the upper right corner of the wizard. Wildcards are supported and the file(s) must be located in your installation's source directory when running the compiler, unless a fully qualified pathname is specified or the pathname is prefixed by \"compiler:\", in which case it looks for the file in the compiler directory.\n\n256-color bitmaps may not display correctly in 256-color mode, since it does not handle palettes.\n\nWhen multiple files are specified, Setup will automatically select the one which best matches the system's DPI setting. The recommended size of the bitmap per DPI setting is:\n100%\t55x55\n125%\t64x68\n150%\t83x80\n175%\t92x97\n200%\t110x106\n225%\t119x123\n250%\t138x140\n";

        private readonly string saveSetupIconPath =
            // "Assets/Packages/ProjectBuilder/Editor/Installer System/Resources/SetupIcon.ico";
            "Packages/com.kabreetgames.projectbuilder/Editor/Installer System/Resources/SetupIcon.ico";

        private readonly string wizardImagePath =
            // "Assets/Packages/ProjectBuilder/Editor/Installer System/Resources/WizardImage.bmp";
            "Packages/com.kabreetgames.projectbuilder/Editor/Installer System/Resources/WizardImage.bmp";

        private readonly string wizardSmallPath =
            // "Assets/Packages/ProjectBuilder/Editor/Installer System/Resources/WizardSmallImage.bmp";
            "Packages/com.kabreetgames.projectbuilder/Editor/Installer System/Resources/WizardSmallImage.bmp";

        [MenuItem("Kabreet Games/Build/Installer Resources")]
        private static void ShowWindow()
        {
            var window = GetWindow<InstallerResourceSetup>();
            window.titleContent = new GUIContent("Installer Resources");
            window.Show();
        }

        private void OnEnable()
        {
            LoadTextures();
        }

        private void LoadTextures()
        {
            setupIcon = ConvertIconToTexture(saveSetupIconPath);
            wizardImage = ConvertIconToTexture(wizardImagePath);
            wizardSmall = ConvertIconToTexture(wizardSmallPath);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Convert", GUILayout.MaxWidth(200))) HandleSaving();
            GUILayout.Space(20);
            setupIcon = AddIconSlot("Setup Icon", SetUpIconDescription, setupIcon,1);
            wizardImage = AddIconSlot("Wizard Image", WizardImageDescription, wizardImage,0.5f);
            wizardSmall = AddIconSlot("Wizard Small Image", WizardSmallDescription, wizardSmall,1);
        }

        private void HandleSaving()
        {
            if (setupIcon)
            {
                ConvertToIcon(setupIcon, saveSetupIconPath);
            }
            else
            {
                DeleteAtPath(saveSetupIconPath);
            }

            if (wizardSmall)
            {
                ConvertTextureToBmp(wizardSmall, wizardSmallPath);
            }
            else
            {
                DeleteAtPath(wizardSmallPath);
            }


            if (wizardImage)
            {
                ConvertTextureToBmp(wizardImage, wizardImagePath);
            }
            else
            {
                DeleteAtPath(wizardImagePath);
            }
        }

        private Texture2D AddIconSlot(string tName, string description, Texture2D icon, float ratio)
        {
            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                richText = true,
                normal =
                {
                    textColor = Color.red
                },
                active =
                {
                    textColor = Color.red,
                },
                focused =
                {
                    textColor = Color.red
                },
                hover =
                {
                    textColor = Color.red
                }
            };
            GUILayout.BeginHorizontal("box");
            GUILayout.BeginVertical(GUILayout.MaxWidth(position.width * 0.75f));
            GUILayout.Label(tName, labelStyle);
            GUILayout.Space(10);
            GUILayout.Label("Description:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.Label(description, EditorStyles.wordWrappedLabel);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUILayout.MaxWidth(position.width * 0.25f));
            GUILayout.Space(10);
            icon = (Texture2D)EditorGUILayout.ObjectField("", icon, typeof(Texture2D), false,
                GUILayout.Width(position.width * 0.25f*ratio), GUILayout.Height(position.width * 0.25f),
                GUILayout.MaxWidth(200), GUILayout.MaxHeight(200));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            return icon;
        }

        private void DeleteAtPath(string path)
        {
            if (!File.Exists(path)) return;
            File.Delete(path);
        }

        private void ConvertToIcon(Texture2D icon, string path)
        {
            string loadPath;
            try
            {
                loadPath = Path.GetFullPath(AssetDatabase.GetAssetPath(icon));
            }
            catch (Exception)
            {
                return;
            }

            var savePath = Path.GetFullPath(path);
            using var stream = File.OpenWrite(savePath);
            var bitmap = (Bitmap)Image.FromFile(loadPath);
            Icon.FromHandle(bitmap.GetHicon()).Save(stream);
        }

        private void ConvertTextureToBmp(Texture2D texture, string path)
        {
            string loadPath;
            try
            {
                loadPath = Path.GetFullPath(AssetDatabase.GetAssetPath(texture));
            }
            catch (Exception)
            {
                return;
            }
            var savePath = Path.GetFullPath(path);
            var bitmap = (Bitmap)Image.FromFile(loadPath);
            bitmap.Save(savePath, ImageFormat.Bmp);
        }
        private Texture2D ConvertIconToTexture(string path)
        {
            var loadPath = Path.GetFullPath(path);
            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(loadPath);
            }
            catch (Exception)
            {
                return null;
            }

            var width = bitmap.Width;
            var height = bitmap.Height;

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var data = new Color[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    data[(height - 1 - y) * width + x] =
                        new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                }
            }

            texture.SetPixels(data);
            texture.Apply();

            return texture;
        }
    }
}