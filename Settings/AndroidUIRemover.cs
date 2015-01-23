using UnityEngine;

public class AndroidUIRemover
{
#if UNITY_ANDROID
    static AndroidJavaObject activityInstance;
    static AndroidJavaObject windowInstance;
    static AndroidJavaObject viewInstance;

    const int SYSTEM_UI_FLAG_HIDE_NAVIGATION = 2;
    const int SYSTEM_UI_FLAG_LAYOUT_STABLE = 256;
    const int SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION = 512;
    const int SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN = 1024;
    const int SYSTEM_UI_FLAG_IMMERSIVE = 2048;
    const int SYSTEM_UI_FLAG_IMMERSIVE_STICKY = 4096;
    const int SYSTEM_UI_FLAG_FULLSCREEN = 4;

    public delegate void RunPtr();

    public static void Run()
    {
        if (viewInstance != null)
        {
            viewInstance.Call("setSystemUiVisibility",
                              SYSTEM_UI_FLAG_LAYOUT_STABLE
                              | SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                              | SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                              | SYSTEM_UI_FLAG_HIDE_NAVIGATION
                              | SYSTEM_UI_FLAG_FULLSCREEN
                              | SYSTEM_UI_FLAG_IMMERSIVE_STICKY);
        }
    }

    public static void DisableNavUI()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            windowInstance = activityInstance.Call<AndroidJavaObject>("getWindow");
            viewInstance = windowInstance.Call<AndroidJavaObject>("getDecorView");

            AndroidJavaRunnable RunThis;
            RunThis = new AndroidJavaRunnable(new RunPtr(Run));
            activityInstance.Call("runOnUiThread", RunThis);
        }
    }
#endif
}