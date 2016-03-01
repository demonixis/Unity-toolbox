using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public sealed class OSVRManager : MonoBehaviour
    {
        void Awake()
        {
            if (!GameVRSettings.HasOSVRHMDEnabled(true))
            {
                DestroyImmediate(GetComponent<OSVR.Unity.DisplayController>());
                DestroyImmediate(GetComponentInChildren<OSVR.Unity.VRViewer>());
                DestroyImmediate(this);
            }
        }
    }
}
