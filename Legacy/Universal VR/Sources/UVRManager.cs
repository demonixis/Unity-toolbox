using Demonixis.VR.Sensors;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.VR
{
    public class UVRManager : MonoBehaviour
    {
        private static UVRManager _instance = null;
        private UVRSensor _sensor = null;
        private Quaternion _headRotation = Quaternion.identity;

        [Tooltip("The head that contains cameras.")]
        public Transform head;

        [Tooltip("The parent to use to define the finale rotation.")]
        public Transform parentObject;

        [Tooltip("A text element that will display a message if an error occurs.")]
        [SerializeField]
        private Text warningText;

        [Tooltip("Enable or disable the distortion correction post process. It is recommended to disable this effect on low end devices.")]
        [SerializeField]
        private bool distortionCorrection = true;

        [Tooltip("Sets the desired framerate. A great framerate is best but it consume more battery. Default is 60.")]
        [SerializeField]
        private int targetFramerate = 60;

        [Tooltip("Enable or disable the sleep timeout. It is recommended to keep this value to true.")]
        [SerializeField]
        private bool neverSleepPhone = true;

#if UNITY_EDITOR
        [Header("Mouse Emulation")]
        public bool emulateWithMouse = true;
        public float sensitivityX = 15.0f;
        public float sensitivityY = 15.0f;
        public float rotationX = 0.0f;
        public float rotationY = 0.0f;
        public int mouseButtonTrigger = 1;
#endif

        /// <summary>
        /// Enable or disable the distortion correction effect.
        /// </summary>
        public bool DistortionCorrection
        {
            get { return distortionCorrection; }
            set
            {
                if (distortionCorrection != value)
                {
                    distortionCorrection = value;
                    SetDistortionCorrection(value);
                }
            }
        }

        /// <summary>
        /// Gets the head quaternion.
        /// </summary>
        public Quaternion HeadRotation
        {
            get { return _headRotation; }
        }

        /// <summary>
        /// Gets the current instance of the manager.
        /// </summary>
        public static UVRManager SDK
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UVRManager>();

                    if (_instance == null)
                    {
                        Debug.LogWarning("SDK not found. An instance is created...");
                        var go = new GameObject("UVRManager");
                        _instance = go.AddComponent<UVRManager>();
                    }
                }
                return _instance;
            }
        }

        void OnEnable()
        {
            if (neverSleepPhone)
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void OnDisable()
        {
            if (neverSleepPhone)
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
                throw new UnityException("Only one instance of the UVRManager is allowed.");

            _instance = this;

            Application.targetFrameRate = targetFramerate;

            if (head == null)
                head = GetComponent<Transform>();

            if (warningText != null)
                warningText.enabled = false;

            SetSensor(SensorType.Auto);
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            if (emulateWithMouse)
            {
                if (Input.GetMouseButton(mouseButtonTrigger))
                {
                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, -90.0f, 90.0f);
                    _headRotation = Quaternion.Euler(-rotationY, rotationX, 0.0f);
                }
            }

            head.eulerAngles = Vector3.zero;
#else
			_sensor.GetRotation(ref _headRotation);

            head.eulerAngles = _sensor.OriginalRotation;
#endif

            if (parentObject != null)
                head.localRotation *= parentObject.rotation * _headRotation;
            else
                head.localRotation *= _headRotation;
        }

        /// <summary>
        /// Enable or disable the distortion correction effect.
        /// </summary>
        /// <param name="isEnabled">Sets to true to enable the effect or false to disable.</param>
        private void SetDistortionCorrection(bool isEnabled)
        {
            var eyes = GetComponentsInChildren<UVREye>();

            for (int i = 0, l = eyes.Length; i < l; i++)
                eyes[i].DistortionCorrection = isEnabled;
        }

        /// <summary>
        /// Recenter the head to its default coordinates.
        /// </summary>
        public void Recenter()
        {
#if UNITY_EDITOR
            rotationX = 0.0f;
            rotationY = 0.0f;
#endif
            if (parentObject != null)
                head.localRotation = parentObject.rotation;
            else
                head.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Set the sensor used for the head tracking.
        /// </summary>
        /// <param name="sensor">The desired sensor.</param>
        public void SetSensor(SensorType sensor)
        {
            if (_sensor != null)
                DestroyImmediate(_sensor);

            if ((sensor == SensorType.Auto && UVRGyroscope.IsAvailable) || sensor == SensorType.Gyroscope)
                _sensor = gameObject.AddComponent<UVRSensor>();

            else if ((sensor == SensorType.Auto && UVRInclinometer.IsAvailable) || sensor == SensorType.Inclinometer)
                _sensor = gameObject.AddComponent<UVRSensor>();

            else if ((sensor == SensorType.Auto && UVROrientationSensor.IsAvailable) || sensor == SensorType.Orientation)
                _sensor = gameObject.AddComponent<UVROrientationSensor>();

            else
            {
                _sensor = gameObject.AddComponent<UVRSensor>();

                if (warningText != null)
                {
                    warningText.text = "Head Tracking not available";
                    warningText.enabled = true;
                }
            }
        }
    }
}