using System;
using UnityEngine;

namespace Demonixis.VR
{
    public class UVRGaze : MonoBehaviour
    {
        private RaycastHit _raycastHit;
        private Transform _transform;
        private Color _lastColor;
        private Renderer _lastRenderer;
        private string _lastName = string.Empty;
        private float _elapsedTime = 0.0f;
        public float maxDistance = 15.0f;
        public string[] tags = new string[] { "Obstacle" };
        public Color hightlightColor = Color.red;
        public float checkInterval = 0.95f;

        void Start()
        {
            _transform = GetComponent<Transform>();
        }

        private void SetLastMaterial()
        {
            RestoreLastMaterial();

            _lastRenderer = _raycastHit.collider.GetComponent<Renderer>();

            if (_lastRenderer != null)
            {
                _lastName = _lastRenderer.name;
                _lastColor = _lastRenderer.material.color;
                _lastRenderer.material.color = hightlightColor;
            }
        }

        private void RestoreLastMaterial()
        {
            if (_lastRenderer != null)
                _lastRenderer.material.color = _lastColor;

            _lastName = string.Empty;
        }

        private void Check()
        {
            if (Physics.Raycast(_transform.position, _transform.forward, out _raycastHit, maxDistance))
            {
                if (_raycastHit.collider.name != _lastName && Array.IndexOf(tags, _raycastHit.collider.tag) > -1)
                    SetLastMaterial();
            }
            else
                RestoreLastMaterial();
        }

        void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= checkInterval)
            {
                _elapsedTime = 0.0f;
                Check();
            }
        }
    }
}
