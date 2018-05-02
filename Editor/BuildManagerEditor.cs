#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class BuildManagerEditor : EditorWindow
{
    private string _gameName = "GAMENAME";
    private string _prefixBuildName = "GAMENAME";
    private string _version = "0.1.1";

    private string[] _completeLevels = new string[]
    {
        "Assets/Scenes/GameScene.unity"
    };

    [MenuItem("Demonixis/Build Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BuildManagerEditor));
    }

    void OnGUI()
    {
        GUILayout.Label("Build Settings", EditorStyles.boldLabel);

        _gameName = EditorGUILayout.TextField("Game Name", _gameName);
        _prefixBuildName = EditorGUILayout.TextField("Prefix Build Name", _prefixBuildName);
        _version = EditorGUILayout.TextField("Version", _version);

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
                _prefixBuildName + "_" + _version + "-windows_x86",
                _prefixBuildName + "_" + _version + "-windows_x64",
                _prefixBuildName + "_" + _version + "-linux",
                _prefixBuildName + "_" + _version + "-mac"
        };

        // Build Windows x86
        BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[0] + "/" + _gameName + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Build Windows x64
        BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[1] + "/" + _gameName + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

        // Build Linux x86 and x64
        BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[2] + "/" + _gameName + ".x86", BuildTarget.StandaloneLinuxUniversal, BuildOptions.None);

        // Build Mac
        BuildPipeline.BuildPlayer(_completeLevels, path + "/" + buildNames[3] + "/" + _gameName + ".app", BuildTarget.StandaloneOSX, BuildOptions.None);
    }
}
#endif