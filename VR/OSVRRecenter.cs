using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public sealed class OSVRRecenter : MonoBehaviour
    {
        private static OSVRRecenter _instance = null;

        public void SetRoomRotationUsingHead()
        {
            var clientKit = OSVR.Unity.ClientKit.instance;
            var player = GameObject.FindWithTag("Player");

            if (player != null && clientKit != null)
            {
                var displayController = player.GetComponentInChildren<OSVR.Unity.DisplayController>();

                if (displayController != null && displayController.UseRenderManager)
                    displayController.RenderManager.SetRoomRotationUsingHead();
                else if (clientKit != null)
                    clientKit.context.SetRoomRotationUsingHead();
            }
        }

        public static void Recenter()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OSVRRecenter>();

                if (_instance == null)
                {
                    var go = new GameObject("OSVRRecenter");
                    _instance = (OSVRRecenter)go.AddComponent(typeof(OSVRRecenter));
                }
            }

            _instance.SetRoomRotationUsingHead();
        }
    }
}
