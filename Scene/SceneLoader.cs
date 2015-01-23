using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader
{
	private static Dictionary<string, object> _parameters = new Dictionary<string, object>();

	public static T GetParam<T>(string key)
	{
		T t = default(T);

		if (_parameters.ContainsKey(key))
			return (T)_parameters[key];

		return t;
	}

	public static void SetParam(string key, object value)
	{
		if (_parameters.ContainsKey(key))
			_parameters[key] = value;
		else
			_parameters.Add(key, value);
	}

	public static void ClearParams()
	{
		_parameters.Clear();
	}

	public static void LoadLevel(string level)
	{
		Time.timeScale = 1.0f;
		Application.LoadLevel(level);
	}
}
