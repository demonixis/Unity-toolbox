using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public sealed class Translation : MonoBehaviour
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
            string trans = string.Empty;

            if (Array.IndexOf<string>(AvailableLanguages, lang) == -1)
                lang = AvailableLanguages[0];

            trans = Resources.Load<TextAsset>(string.Format("Translations/Texts.{0}", lang.ToLower())).text;

            s_gameTexts = ParseFile(trans);
        }
    }

    // Returns the translation for this key.
    public static string Get(string key)
    {
        if (s_gameTexts.ContainsKey(key))
            return s_gameTexts[key];

#if UNITY_EDITOR
        Debug.Log(string.Format("The key {0} is missing", key));
#endif

        return key;
    }

    public void Reload()
    {
        s_initialized = false;
        Awake();
    }

    public static Dictionary<string, string> ParseFile(string text)
    {
        var content = new Dictionary<string, string>();

        using (var stream = new StringReader(text))
        {
            var line = stream.ReadLine();
            var temp = new string[2];
            var key = string.Empty;
            var value = string.Empty;
            var i = 0;
            var l = 0;

            while (line != null)
            {
                temp = line.Split('=');

                if (temp.Length > 0)
                {
                    key = temp[0].Trim();

                    if (temp.Length > 2)
                    {
                        value = temp[1];
                        for (i = 2, l = temp.Length; i < l; i++)
                            value += "=" + temp[i];

                        value = value.Trim();
                    }
                    else if (temp.Length == 2)
                        value = temp[1].Trim();
                    else
                        value = key;

                    if (content.ContainsKey(key))
                        content[key] = value;
                    else
                        content.Add(key, value);
                }
                else if (key != string.Empty)
                {
                    temp = line.Split('\\');

                    if (temp.Length > 0)
                        content[key] += temp[0];
                }
                else
                    key = string.Empty;

                line = stream.ReadLine();
            }
        }

        return content;
    }
}
