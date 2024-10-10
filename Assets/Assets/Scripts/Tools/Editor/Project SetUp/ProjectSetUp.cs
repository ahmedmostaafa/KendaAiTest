using System.IO;
using UnityEditor;

namespace KabreetGames.Tools
{
    public class ProjectFolderSetup : EditorWindow
    {
        [MenuItem("Kabreet Games/Setup/Create Folder Setup", false, 100)]
        private static void CreateFolderSetup()
        {
            CreateFolder("Assets", "Animations");
            CreateFolder("Assets", "Audio");
            CreateFolder("Assets", "Materials");
            CreateFolder("Assets", "Meshes");
            CreateFolder("Assets", "Prefabs");
            CreateFolder("Assets", "Scripts");
            CreateFolder("Assets", "Shaders");
            CreateFolder("Assets", "Scenes");
            CreateFolder("Assets", "Sprites");
            CreateFolder("Assets", "Textures");
            CreateFolder("Assets", "Fonts");
            CreateFolder("Editor");
            CreateFolder("Resources");
            CreateFolder("StreamingAssets");
            CreateFolder("Documentation");
            CreateFolder("../Builds");
            CreateFolder("../Builds", "Windows");
            CreateFolder("Plugins");
            CreateFolder("Tests");
            CreateFolder("Graphics");
            CreateFolder("../Fmod Project");
        }
        
        [MenuItem("Kabreet Games/Setup/Create Folder Setup", true)]
        private static bool ValidateMyOption()
        {
            return CanExecuteMyOption();
        }
    
        private static bool CanExecuteMyOption()
        {
            return true; // This will disable the menu item.
        }
    
        private static void CreateFolder(params string[] folderNames)
        {
            var path = "Assets";
            foreach (var folderName in folderNames)
            {
                path = Path.Combine(path, folderName);
                if (!AssetDatabase.IsValidFolder(path))
                    AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(path));
            }
        }
    }
}

