using System.Collections;
using UnityEngine;

namespace Demonixis.VR
{
    public class UVRController : MonoBehaviour
    {
        // Caches
        private Transform _headTransform;
        private Transform _eyePosition;         // Define the UI position
        private Transform _playerTransform;     // The current transform
        private Transform _socleTransform;
        private Renderer _renderer;
        private bool _isLookingSocle = false;
        private bool _autoWalk = false;
        private bool _isActive = false;
        private string _lastHitName = string.Empty;

        [Tooltip("Rotate the player with the movements of the head. It turns the players on Y axis.")]
        public bool rotateBodyWithHead = false;

        [Header("Socle look")]
        public GameObject socle;
        public Material walkOnMaterial;
        public Material walkOffMaterial;

        [Header("Movement and collisions")]
        public float moveSpeed = 5.0f;
        public float waitBeforeActive = 2.0f;
        public bool stopOnFirstCollision = false;
        public string[] stopTags = new string[] { "Obstacle" };
        
        void Start()
        {
            _headTransform = UVRManager.SDK.head;
            _playerTransform = GetComponent<Transform>();

            // Gets the first eye.
            var cam = GetComponentInChildren<Camera>();
            _eyePosition = cam.GetComponent<Transform>();

            // Build the socle object.
            if (socle == null)
            {
                socle = GameObject.CreatePrimitive(PrimitiveType.Quad);
                socle.name = "UVR_Socle";

                _socleTransform = socle.GetComponent<Transform>();
                _socleTransform.parent = _playerTransform;

                Destroy(socle.GetComponent<Collider>());
                var socleCollider = socle.AddComponent<BoxCollider>();
                socleCollider.isTrigger = true;
            }
            else
                _socleTransform = socle.GetComponent<Transform>();

            // And the materials.
            if (walkOnMaterial == null)
            {
                walkOnMaterial = new Material(Shader.Find("Standard"));
                walkOnMaterial.color = Color.blue;
            }

            if (walkOffMaterial == null)
            {
                walkOffMaterial = new Material(Shader.Find("Standard"));
                walkOffMaterial.color = Color.red;
            }

            _renderer = socle.GetComponent<Renderer>();
            _renderer.enabled = false;

            StartCoroutine("EnableComponent");
        }

        void Update()
        {
            if (!_isActive)
                return;

            _lastHitName = string.Empty;

            if (_headTransform.localEulerAngles.x > 70.0f && _headTransform.localEulerAngles.x < 100.0f)
            {
                _socleTransform.rotation = _headTransform.rotation;
                _socleTransform.position = _eyePosition.position;
                _socleTransform.Translate(0, 0, 0.7f);
                _lastHitName = socle.name;
            }                

            if (_lastHitName == socle.name)
            {
                if (!_isLookingSocle)
                {
                    _autoWalk = !_autoWalk;
                    _renderer.enabled = true;
                    _renderer.material = _autoWalk ? walkOnMaterial : walkOffMaterial;
                    _isLookingSocle = true;
                }
            }
            else if (_isLookingSocle)
            {
                _renderer.enabled = false;
                _isLookingSocle = false;
            }

            if (!_isLookingSocle && _autoWalk)
                _playerTransform.Translate(0.0f, 0.0f, moveSpeed * Time.deltaTime);
        
            if (rotateBodyWithHead)
            {
                var headRotation = _headTransform.eulerAngles;
                var bodyRotation = _playerTransform.eulerAngles;
                _playerTransform.rotation = Quaternion.Euler(bodyRotation.x, headRotation.y, bodyRotation.z);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (stopOnFirstCollision)
            {
                if (System.Array.IndexOf(stopTags, collision.collider.tag) > -1)
                    _autoWalk = false;
            }
        }

        public void Walk()
        {
            if (_isLookingSocle)
                UVRManager.SDK.Recenter();

            _renderer.enabled = false;
            _isLookingSocle = false;

            _autoWalk = true;
        }

        public void Stop()
        {
            _autoWalk = false;
        }

        private IEnumerator EnableComponent()
        {
            yield return new WaitForSeconds(waitBeforeActive);
            _isActive = true;
        }
    }
}
