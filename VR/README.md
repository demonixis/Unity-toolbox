# Demonixis.VR

This namespace contains an abstraction of few VR SDKs. Without any code, it's easy to add VR support for any of the supported SDKs, you just have to attach these scripts on your player object.

## Requirement
- Enable the VR support in `PlayerPrefs`
- Download VR SDKs of your choice
    - [Cardboard SDK](https://developers.google.com/cardboard/unity/)
	- [Oculus SDK](https://developer.oculus.com/downloads/)
	- [OSVR SDK](https://github.com/OSVR/OSVR-Unity)
	- [OpenVR SDK](https://www.assetstore.unity3d.com/en/#!/content/32647)
	
## Setup
Your player object **must** respect the same hierarchy as the Oculus SDK Camera Rig prefab.

Player (`GameObject`)
    : Head (`Transform`)
        : TrackingSpace (`Transform`)
            : EyeCenterAnchor (`Camera`)
            
All scripts can be put on the `GameObject` of you choice because they use the `Camera.main` helper to find the **Main Camera**. However I recommand you to put them on the Head GameObject.

Note that you can change the Transform of the Head object. It's not recommanded to change the TrackingSpace Transform. Finally the MainCamera Transform is overrided by Unity or the VR SDK with the HMD's rotations & positions.

If you don't want to use a specific SDK, just remove the adapted `define` in `GameVRSettings`.
- `USE_CARDBOARD_SDK`
- `USE_OSVR_SDK`
- `USE_OPENVR_SDK`

### About OpenVR

There is a small bug in the SteamVR plugin, you have to manually change a line in `SteamVR_Camera.cs`
```CSharp
// SteamVR_Camera.cs line 184
var camera = head.GetComponent<Camera>();
// Become
var camera = head.gameObject.AddComponent<Camera>();
```

## Using it

```csharp
// Somewhere in your code
using Demonixis.VR;

void Start()
{
    // Move your 2D UI in WorldSpace UI if the VR mode is enabled.
    if (GameVRSettings.VREnabled)
        MoveUIToVRUI();
        
    // Enable a specific script is SteamVR is enabled.
    if (GameVRSettings.OpenVREnabled)
        EnableSteamVRSpecificFeatures();
    
    // Type of APIs : UnityEngine.VR or external SDK.
    switch(GameVRSettings.VRDeviceType)
    {
        case VRDeviceType.OSVR:
            Debug.Log("Hello OSVR");
            break;
            
        case VRDevice.Cardboard:
            Debug.Log("Hello Cardboard");
            break;
    }
    
    // Work with the basic manager
    var manager = GameVRSettings.ActiveManager;
    Debug.Log(manager.RenderScale);
    manager.RenderScale = 0.89f;
    
    // Work with a specific manager
    var oculusManager = GameVRSettings.ActiveManager as OculusManager;
    if (oculusManager != null)
    {
        // GearVR Specifics
        oculusManager.CPULevel = 2;
        oculusManager.GPULevel = 3;
    }
}

void Input()
{
    // Recenter the camera
    if (Input.GetKeyDown(KeyCode.R))
        GameVRSettings.Recenter();
       
    // Toggle VR mode. 
    if (Input.GetKeyDown(KeyCode.F1))
    {
        var manager = GameVRSettings.ActiveManager;
        manager.SetVREnabled(!manager.IsEnabled);
    }
}
```

Don't hesitate to contribute by sending pull request or opening issues.

## License
The same license as the entire repository: MIT.