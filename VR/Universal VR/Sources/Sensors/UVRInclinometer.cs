#if UNITY_METRO && !UNITY_EDITOR
#define WINDOWS_PHONE
#endif

using UnityEngine;
#if WINDOWS_PHONE
using Windows.Devices.Sensors;
#endif

using System;

namespace Demonixis.VR.Sensors
{
    public class UVRInclinometer : UVRSensor
    {
        private bool _hasSensor = false;

#if WINDOWS_PHONE
        private float Deg2Rad = Mathf.PI / 180.0f;
        private Inclinometer _sensor;
        private InclinometerReading _iReading;
#endif

        public static bool IsAvailable
        {
            get
            {
#if WINDOWS_PHONE
                return Inclinometer.GetDefault() != null;
#else
                return false;
#endif
            }
        }

        void Start()
        {
#if WINDOWS_PHONE
            _sensor = Inclinometer.GetDefault();
            _hasSensor = _sensor != null;

            if (_hasSensor)
                _sensor.ReportInterval = _sensor.MinimumReportInterval;
#else
            _hasSensor = false;
#endif
        }

        public override void GetRotation(ref Quaternion quaternion)
        {
            if (_hasSensor)
            {
#if WINDOWS_PHONE
                _iReading = _sensor.GetCurrentReading();

                var alpha = _iReading.YawDegrees;
                var beta = _iReading.RollDegrees;
                var gamma = _iReading.PitchDegrees;

                if (beta > 0)
                {
                    alpha = alpha - 180.0f;
                    beta = -180.0f + beta;
                    gamma = gamma - 180.0f;
                }

                var z = alpha * Deg2Rad / 2.0f;
                var x = beta * Deg2Rad / 2.0f;
                var y = gamma * Deg2Rad / 2.0f;
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
#else
                quaternion = Quaternion.identity;
#endif
            }
            else
                quaternion = Quaternion.identity;
        }
    }
}
