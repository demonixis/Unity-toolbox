using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System;

public class Translation : MonoBehaviour
{
    public static readonly string[] AvailableLanguages = { "English", "French" };
    private static Dictionary<string, string> s_gameTexts = new Dictionary<string, string>();
    private static bool s_initialized = false;

    public string language = string.Empty;

    void Awake()
    {
        if (!s_initialized)
        {
            s_initialized = true;

            string lang = language != string.Empty ? language : Application.systemLanguage.ToString();
            string jsonString = string.Empty;

            if (Array.IndexOf<string>(AvailableLanguages, lang) == -1)
                lang = AvailableLanguages[0];

            jsonString = Resources.Load<TextAsset>(string.Format("i18n/translations.{0}", lang.ToLower())).text;

            JSONNode json = JSON.Parse(jsonString);
            int size = json.Count;

            s_gameTexts = new Dictionary<string, string>(size);

            JSONArray array;
            for (int i = 0; i < size; i++)
            {
                array = json[i].AsArray;
                s_gameTexts.Add(array[0].Value, array[1].Value);
            }
        }
    }

    // Returns the translation for this key.
    public static string Get(string key)
    {
        if (s_gameTexts.ContainsKey(key))
            return s_gameTexts[key];

        return key;
    }

    public void Reload()
    {
        s_initialized = false;
        Awake();
    }
}
