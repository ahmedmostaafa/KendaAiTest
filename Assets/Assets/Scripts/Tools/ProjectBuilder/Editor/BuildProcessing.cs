using System;
using System.Diagnostics;
using System.IO;
using KabreetGames.ProjectBuilder.Window;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace KabreetGames.ProjectBuilder
{
    public class BuildProcessing : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public static bool IsPackage =>
            UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                System.Reflection.Assembly.GetExecutingAssembly()) != null;
        
        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.result is BuildResult.Failed or BuildResult.Cancelled) return;
            ExportDataFile();
            if (report.summary.platformGroup is not (BuildTargetGroup.Standalone)) return;
            if (!EditorUtility.DisplayDialog("Create Installer", "Create the installer For this Build", "Yes",
                    "No")) return;
            RunInstaller();
        }

        [MenuItem("Kabreet Games/Build/Increment Build Version/Increment Build", false, 4)]
        private static void IncrementBuildBuildVersion()
        {
            Debug.Log("Incrementing Build Version");
            if (!Version.TryParse(PlayerSettings.bundleVersion, out var version)) return;
            version = new Version(version.Major, version.Minor, version.Build + 1);
            PlayerSettings.bundleVersion = version.ToString();
        }

        [MenuItem("Kabreet Games/Build/Increment Build Version/Increment Minor", false, 5)]
        private static void IncrementMinorBuildVersion()
        {
            if (!Version.TryParse(PlayerSettings.bundleVersion, out var version)) return;
            version = new Version(version.Major, version.Minor + 1, version.Build);
            PlayerSettings.bundleVersion = version.ToString();
        }

        [MenuItem("Kabreet Games/Build/Increment Build Version/Increment Major", false, 6)]
        private static void IncrementMajorBuildVersion()
        {
            if (!Version.TryParse(PlayerSettings.bundleVersion, out var version)) return;
            version = new Version(version.Major + 1, version.Minor, version.Build);
            PlayerSettings.bundleVersion = version.ToString();
        }

        private static void ExportDataFile()
        {
            var path = IsPackage
                ? "Packages/com.kabreetgames.projectbuilder/Editor/Installer System"
                : "Assets/Packages/ProjectBuilder/Editor/Installer System";
            var dataPath = Path.Combine(path, "data.ini");
            var projectPath = Application.dataPath.Replace("/Assets", "");
            var buildPath =
                $"{projectPath}/Builds/";
            var outputPath =
                $"{projectPath}/Builds/standaloneWindows/{PlayerSettings.productName} {PlayerSettings.bundleVersion}";
            var content =
                $"[Version] \nVer={PlayerSettings.bundleVersion} \n \n[AppName] \nName={PlayerSettings.productName} \n \n[AppPublisher] \nPublisher={PlayerSettings.companyName} \n \n[OutPath] \nPath={buildPath}\n\n[ReadPath]\nreadPath={outputPath}";
            Debug.Log(PlayerSettings.bundleVersion.ToString());
            File.WriteAllText(dataPath, content);
        }


        [MenuItem("Kabreet Games/Build/Run Installer", false, 6)]
        private static void RunInstaller()
        {
            const string innoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 6\Compil32.exe";

            var installationScriptRelativePath = IsPackage
                ? "Packages/com.kabreetgames.projectbuilder/Editor/Installer System/Installer.iss"
                : "Assets/Packages/ProjectBuilder/Editor/Installer System/Installer.iss";
            var installationScriptPath = Path.GetFullPath(installationScriptRelativePath);

            var arguments = $"/cc \"{installationScriptPath}\"";

            if (!File.Exists(installationScriptPath) || !File.Exists(innoSetupCompilerPath))
            {
                Debug.LogError($"Error: Inno Setup compiler or installation script not found at {innoSetupCompilerPath}.");
                InstallInnoSetupWindow.ShowWindow();
                return;
            }

            Debug.Log("Compiling installation script...");
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = innoSetupCompilerPath,
                    Arguments = arguments,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using var process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                // Read the output (or the error)
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                // Wait for the process to exit and handle any errors
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Debug.Log("Inno Setup compilation successful.");
                    Debug.Log(output); // Print output if needed
                }
                else
                {
                    Debug.LogError($"Inno Setup compilation failed with error: {error}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"An exception occurred: {e.Message}");
            }
        }


        [MenuItem("Kabreet Games/Build/Build/Build Web", false, -1)]
        public static void BuildWeb()
        {
            Build(BuildTarget.WebGL);
        }

        [MenuItem("Kabreet Games/Build/Build/Build Windows", false, -4)]
        public static void BuildWindows()
        {
            Build(BuildTarget.StandaloneWindows);
        }

        [MenuItem("Kabreet Games/Build/Build/Build Mac", false, -2)]
        public static void BuildMac()
        {
            Build(BuildTarget.StandaloneOSX);
        }


        [MenuItem("Kabreet Games/Build/Build/Build Linux", false, -3)]
        public static void BuildLinux()
        {
            Build(BuildTarget.StandaloneLinux64);
        }

        [MenuItem("Kabreet Games/Build/Build/Build Android Apk", false, 0)]
        public static void BuildAndroid()
        {
            EditorUserBuildSettings.buildAppBundle = false;
            Build(BuildTarget.Android);
        }

        [MenuItem("Kabreet Games/Build/Build/Build Android Aab", false, 1)]
        public static void BuildAndroidAap()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            Build(BuildTarget.Android);
        }

        [MenuItem("Kabreet Games/Build/Build/Build IOS", false, 2)]
        public static void BuildIos()
        {
            Build(BuildTarget.iOS);
        }

        private static void Build(BuildTarget target)
        {
            var extension = target switch
            {
                BuildTarget.Android => EditorUserBuildSettings.buildAppBundle ? "app" : "apk",
                BuildTarget.iOS => "ipa",
                BuildTarget.StandaloneLinux64 => "exe",
                BuildTarget.StandaloneWindows => "exe",
                BuildTarget.StandaloneOSX => "app",
                _ => ""
            };
            var result = EditorUtility.DisplayDialogComplex("Increment Build Version", "Do you want to increment the build version?",
                "Build",
                "No", "Minor");
            switch (result)
            {
                case 0:
                    IncrementBuildBuildVersion();
                    break;
                case 1:
                    break;
                case 2:
                    IncrementMinorBuildVersion();
                    break;
            }
            
            var projectPath = Application.dataPath.Replace("/Assets", "");
            var buildPath =
                $"{projectPath}/Builds/{target}/{PlayerSettings.productName} {PlayerSettings.bundleVersion}";

            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            if (target is not BuildTarget.WebGL)
            {
                buildPath = $"{buildPath}/{PlayerSettings.productName} {PlayerSettings.bundleVersion}.{extension}";
            }

            var scenes = EditorBuildSettings.scenes;
            // Create an array to store scene paths
            var scenePaths = new string[scenes.Length];

            for (var i = 0; i < scenes.Length; i++)
            {
                scenePaths[i] = scenes[i].path;
            }

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = buildPath,
                target = target,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                    EditorUtility.RevealInFinder(buildPath);
                    break;
                case BuildResult.Failed:
                    Debug.Log("Build failed");
                    break;
                case BuildResult.Unknown:
                    break;
                case BuildResult.Cancelled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}