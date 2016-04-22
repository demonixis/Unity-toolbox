using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public class CardboardManager : VRManager
    {
        private static CardboardManager _instance = null;
#if UNITY_ANDROID
        private Cardboard cardboard = null;
#endif

#region Singleton

        public static CardboardManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CardboardManager>();

                    if (_instance == null)
                    {
                        var go = new GameObject("CardboardManager");
                        _instance = go.AddComponent<CardboardManager>();
                    }
                }

                return _instance;
            }
        }

#endregion

        protected override void CheckInstance()
        {
            if (_instance != null && _instance != this)
                Destroy(this);
            else if (_instance == null)
                _instance = this;
        }

        public override void SetVREnabled(bool isEnabled)
        {
#if UNITY_ANDROID
            if (UnityEngine.VR.VRDevice.isPresent)
                return;

            if (isEnabled)
            {
                if (cardboard == null)
                {
                    var camera = Camera.main.gameObject;
                    var parent = camera.transform.parent.gameObject;

                    camera.AddComponent<StereoController>();
                    parent.AddComponent<CardboardHead>();
                    cardboard = gameObject.AddComponent<Cardboard>();
                }
                else
                    cardboard.VRModeEnabled = true;
            }
            else if (cardboard != null)
                cardboard.VRModeEnabled = false;

            vrEnabled = isEnabled;
#endif
        }

#region Static Methods

        public static Vector3 GetLocalPosition(byte viewerIndex)
        {
#if UNITY_ANDROID
            return Cardboard.SDK.HeadPose.Position;
#else
            return Vector3.zero;
#endif
        }

        public static Quaternion GetLocalRotation(uint viewerIndex)
        {
#if UNITY_ANDROID
            return Cardboard.SDK.HeadPose.Orientation;
#else
            return Quaternion.identity;
#endif
        }

        public static void Recenter()
        {
#if UNITY_ANDROID
            Cardboard.SDK.Recenter();
#endif
        }

        public static void SetRenderScale(float scale)
        {
#if UNITY_ANDROID
            Cardboard.SDK.StereoScreenScale = scale;
#endif
        }
    }

#endregion
}