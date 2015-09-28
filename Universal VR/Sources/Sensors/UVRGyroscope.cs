using UnityEngine;

namespace Demonixis.VR.Sensors
{
	public class UVRGyroscope : UVRSensor
	{
        private Vector3 _rotation;
        private Gyroscope _gyro;

        public static bool IsAvailable
        {
            get { return SystemInfo.supportsGyroscope; }
        }

        void Start()
        {
            _gyro = Input.gyro;
            _gyro.enabled = true;
        }

        void OnEnabled()
        {
            Input.gyro.enabled = true;
        }

        void OnDisabled()
        {
            Input.gyro.enabled = false;
        }

        public override void GetRotation(ref Quaternion quaternion)
        {
            quaternion = _gyro.attitude;
            _rotation = quaternion.eulerAngles;
            _rotation.z += 180.0f;
            quaternion.eulerAngles = _rotation;
        }
    }
}