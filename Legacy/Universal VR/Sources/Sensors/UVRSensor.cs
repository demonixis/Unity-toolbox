using UnityEngine;

namespace Demonixis.VR.Sensors
{
    public enum SensorType
    {
         Auto, Orientation, Inclinometer, Gyroscope
    }

	public class UVRSensor : MonoBehaviour
	{
        protected Vector3 mOriginalRotation = new Vector3(90.0f, 0.0f, 0.0f);

        public Vector3 OriginalRotation
        {
            get { return mOriginalRotation; }
        }

        public virtual void GetRotation(ref Quaternion quaternion) 
		{
		}
	}
}