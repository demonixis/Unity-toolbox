using UnityEngine;

namespace UnityToolbox.Utils
{
    public sealed class AutoRotation : MonoBehaviour
    {
        private Transform _transform;
        public Vector3 axis = Vector3.up;
        public float speed = 25.0f;

        void Start()
        {
            _transform = GetComponent(typeof(Transform)) as Transform;
        }

        void Update()
        {
            _transform.Rotate(axis, Time.deltaTime * speed);
        }
    }
}