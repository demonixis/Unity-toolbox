using System;
using UnityEngine;

public class AutoConfigGraphics : MonoBehaviour
{
    private readonly string[] BlackListedGPU = { "HD Graphics 4600", "HD Graphics 5000" };

    void Start()
    {
#if !UNITY_ANDROID && !UNITY_WP8 && !UNITY_PSM && !UNITY_WP_8_1
        string cgName = SystemInfo.graphicsDeviceName;

        int i = 0;
        int size = BlackListedGPU.Length;
        bool modified = false;

        while (i < size && modified == false)
        {
            if (cgName.Contains(BlackListedGPU[i]))
            {
                QualitySettings.SetQualityLevel(1);
                modified = true;
            }
            i++;
        }
#endif

        Destroy(this);
    }
}