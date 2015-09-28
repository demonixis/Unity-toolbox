#if UNITY_METRO && !UNITY_EDITOR
#define WINDOWS_STORE_APP
#endif

using UnityEngine;
#if WINDOWS_STORE_APP
using Windows.Devices.Sensors;
#endif

namespace Demonixis.VR.Sensors
{
    public class UVROrientationSensor : UVRSensor
    {
        private Quaternion _quaternion = Quaternion.identity;
        private bool _hasSensor = false;

#if WINDOWS_STORE_APP
        private Vector3 _rotation = Vector3.zero;
        private OrientationSensor _sensor;
        private SensorQuaternion _sQuaternion;
#endif

        public static bool IsAvailable
        {
            get
            {
#if WINDOWS_STORE_APP
                return OrientationSensor.GetDefault() != null;
#else
                return false;
#endif
            }
        }

        void Start()
        {
#if WINDOWS_STORE_APP
            _sensor = OrientationSensor.GetDefault();
            _hasSensor = _sensor != null;
#endif
        }

        public override void GetRotation(ref Quaternion quaternion)
        {
            if (_hasSensor)
                ReadOrientationRotation2(ref quaternion);
            else
                quaternion = Quaternion.identity;
        }

        private void ReadOrientationRotation(ref Quaternion quaternion)
        {
#if WINDOWS_STORE_APP
            _sQuaternion = _sensor.GetCurrentReading().Quaternion;
            _quaternion.Set(_sQuaternion.X, _sQuaternion.Y, _sQuaternion.Z, _sQuaternion.W);
            _rotation = _quaternion.eulerAngles;
#endif
            quaternion = _quaternion;
        }

        private void ReadOrientationRotation2(ref Quaternion quaternion)
        {
#if WINDOWS_STORE_APP
            _sQuaternion = _sensor.GetCurrentReading().Quaternion;
            _quaternion.Set(_sQuaternion.X, _sQuaternion.Y, _sQuaternion.Z, _sQuaternion.W);
            _rotation = _quaternion.eulerAngles;

            var alpha = _rotation.z;
            var beta = _rotation.y - 180.0f;
            var gamma = (_rotation.x / 2.0f) - 90.0f;
            // a: Z (0/360)
            // b: Y: (-180/180)
            // g: X: (-90/90)

            var degtorad = Mathf.PI / 180.0f;
            var z = alpha * degtorad / 2.0f;
            var x = beta * degtorad / 2.0f;
            var y = gamma * degtorad / 2.0f;
            var cX = Mathf.Cos(x);
            var cY = Mathf.Cos(y);
            var cZ = Mathf.Cos(z);
            var sX = Mathf.Sin(x);
            var sY = Mathf.Sin(y);
            var sZ = Mathf.Sin(z);

            // ZXY quaternion construction.
            var w = cX * cY * cZ - sX * sY * sZ;
            x = sX * cY * cZ - cX * sY * sZ;
            y = cX * sY * cZ + sX * cY * sZ;
            z = cX * cY * sZ + sX * sY * cZ;

            quaternion.Set(x, y, z, w);

            //var deviceQuaternion = new Quaternion(x, y, z, w);
            
            // Correct for the screen orientation.
            /*var screenOrientation = (GetOrientation() * degtorad) / 2.0f;
            var screenTransform = new Quaternion(0.0f, 0.0f, -Mathf.Sin(screenOrientation), Mathf.Cos(screenOrientation));

            var deviceRotation = deviceQuaternion * screenTransform;
            var r22 = Mathf.Sqrt(0.5f);
            deviceRotation = new Quaternion(-r22, 0.0f, 0.0f, r22) * deviceRotation;

            var rot = deviceQuaternion.eulerAngles;
            var temp = rot.x;
            rot.z = 0;
            rot.x = 0;
            rot.y = rot.x;
            deviceQuaternion = Quaternion.Euler(rot);

            quaternion = deviceRotation;*/
#endif
        }

        private float GetOrientation()
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
                return -90.0f;
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
                return 90.0f;
            else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                return 180.0f;
            else
                return 0.0f;
        }
    }
}