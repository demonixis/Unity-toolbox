using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class LevelManager
{
    private static Dictionary<string, object> _parameters = new Dictionary<string, object>();

    #region Scene parameters

    public static T GetParam<T>(string key)
    {
        return GetParam<T>(key, default(T), false);
    }

    public static T GetParam<T>(string key, T defaultValue)
    {
        return GetParam<T>(key, defaultValue, false);
    }

    public static T GetParam<T>(string key, T defaultValue, bool deleteParam)
    {
        if (_parameters.ContainsKey(key))
        {
            var value = (T)_parameters[key];

            if (deleteParam)
                _parameters.Remove(key);

            return value;
        }
        return defaultValue;
    }

    public static void SetParam(string key, object value)
    {
        if (_parameters.ContainsKey(key))
            _parameters[key] = value;
        else
            _parameters.Add(key, value);
    }

    public static void DeleteParam(string key)
    {
        if (_parameters.ContainsKey(key))
            _parameters.Remove(key);
    }

    public static void ClearParams()
    {
        _parameters.Clear();
    }

    #endregion

    #region Misc

    /// <summary>
    /// Prepare the level mode by reseting the time scale.
    /// </summary>
    private static void PrepareLevel()
    {
        Time.timeScale = 1.0f;
    }

    #endregion

    #region Synchrone Level Loading

    public static void LoadLevel(string level)
    {
        PrepareLevel();
        SceneManager.LoadScene(level);
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public static void Restart()
    {
        PrepareLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region Async Level Loading

    public static AsyncOperation LoadLevelAsync(string level)
    {
        PrepareLevel();
        return SceneManager.LoadSceneAsync(level);
    }

    public static AsyncOperation RestartAsync()
    {
        PrepareLevel();
        return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region Scene data

    public static string GetLevelNumber()
    {
        var temp = SceneManager.GetActiveScene().name.Split('_');

        if (temp.Length > 0)
            return temp[temp.Length - 1];

        return string.Empty;
    }

    public static bool IsLevel()
    {
        return SceneManager.GetActiveScene().name.Contains("Level");
    }

    public static int GetLevelId()
    {
        var temp = SceneManager.GetActiveScene().name.Split('_');
        int value;

        if (int.TryParse(temp[temp.Length - 1], out value))
            return value;
        else
            return IsLevel() ? 0 : -1;
    }

    public static string GetLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

    #endregion
}
