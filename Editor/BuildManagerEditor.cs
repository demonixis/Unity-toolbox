#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Demonixis.Toolbox.Editor
{
    public class BuildManagerEditor : EditorWindow
    {
        private string _gameName = "EXEC_FILENAME";
        private string _prefixBuildName = "FOLDERNAME";
        private string _version = "0.0.0.0";
        private bool _demo = false;

        private string[] _completeLevels = new string[]
        {
            "Assets/Scenes/SplashScreen.unity",
            "Assets/Scenes/Menu.unity",
            "Assets/Scenes/Loading.unity",
            "Assets/Scenes/Levels/Level_0/Main_Level_0.unity",
            "Assets/Scenes/Levels/Level_1/Main_Level_1.unity",
            "Assets/Scenes/Levels/Level_2/Main_Level_2.unity"
        };

        [MenuItem("Demonixis/Build Manager")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BuildManagerEditor));
        }

        void Start()
        {
            _version = GamePrefs.Version;
        }

        void OnGUI()
        {
            GUILayout.Label("Build Settings", EditorStyles.boldLabel);

            _gameName = EditorGUILayout.TextField("Game Name", _gameName);
            _prefixBuildName = EditorGUILayout.TextField("Prefix Build Name", _prefixBuildName);
            _version = EditorGUILayout.TextField("Version", _version);
            _demo = EditorGUILayout.Toggle("Demo build", _demo);

            GUILayout.Label("Desktop", EditorStyles.boldLabel);
            if (GUILayout.Button("Build"))
                MakeDesktopBuilds();
        }

        private void MakeDesktopBuilds()
        {
            PlayerSettings.virtualRealitySupported = true;

            // Get filename.
            var path = EditorUtility.SaveFolderPanel("Choose Builds Folder", "", "");

            var buildNames = new string[]
            {
                _prefixBuildName + "_" + _version + "-windows_x86" + (_demo ? "-SHAREWARE" : ""),
                _prefixBuildName + "_" + _version + "-windows_x64" + (_demo ? "-SHAREWARE" : ""),
                _prefixBuildName + "_" + _version + "-linux" + (_demo ? "-SHAREWARE" : ""),
                _prefixBuildName + "_" + _version + "-mac" + (_demo ? "-SHAREWARE" : "")
            };

            // Build Windows x86
            BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[0] + "/" + _gameName + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);

            // Build Windows x64
            BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[1] + "/" + _gameName + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

            // Build Linux x86 and x64
            BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[2] + "/" + _gameName + ".x86", BuildTarget.StandaloneLinuxUniversal, BuildOptions.None);

            // Build Mac
            BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[3] + "/" + _gameName + ".app", BuildTarget.StandaloneOSXUniversal, BuildOptions.None);
        }
    }
}
#endif