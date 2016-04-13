using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public class CardboardManager : MonoBehaviour
    {
        private static CardboardManager _instance = null;

        [SerializeField]
        private bool m_vrEnabled = true;

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

        #region Default Unity pattern

        void Awake()
        {
            CheckInstance();
        }

        void OnEnabled()
        {
            CheckInstance();
        }

        void Start()
        {
            if (m_vrEnabled)
                SetVREnabled(true);
        }

        private void CheckInstance()
        {
            if (_instance != null && _instance != this)
                Destroy(this);
            else if (_instance == null)
                _instance = this;
        }

        #endregion

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

        public static void SetVREnabled(bool vrEnabled)
        {
#if UNITY_ANDROID
            if (UnityEngine.VR.VRDevice.isPresent)
                return;

            var cardboard = _instance.cardboard;

            if (vrEnabled)
            {
                if (cardboard == null)
                {
                    var camera = Camera.main.gameObject;
                    var parent = camera.transform.parent.gameObject;

                    camera.AddComponent<StereoController>();
                    parent.AddComponent<CardboardHead>();
                    _instance.gameObject.AddComponent<Cardboard>();
                }
                else
                    cardboard.VRModeEnabled = true;
            }
            else if (cardboard != null)
                cardboard.VRModeEnabled = false;
#endif
        }
    }

    #endregion
}