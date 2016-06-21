#define USE_INCONTROL_
#if USE_INCONTROL
using InControl;
#endif
using UnityEngine;

namespace Demonixis.Toolbox.Controllers
{
    /// <summary>
    /// FirstPersonController : A controller for first person games dedicated to VR (works great on non VR games too).
    /// It's a fork of the Oculus' OVRPlayerController script.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        private CharacterController _controller = null;
        private float _moveScale = 1.0f;
        private Vector3 _moveThrottle = Vector3.zero;
        private float _fallSpeed = 0.0f;
        private float _initialYRotation = 0.0f;
        private float _moveScaleMultiplier = 1.0f;
        private float _rotationScaleMultiplier = 1.0f;
        private bool _skipMouseRotation = false;
        private bool _haltUpdateMovement = false;
        private bool _prevHatLeft = false;
        private bool _prevHatRight = false;
        private float _simulationRate = 60f;
        private Transform _transform = null;

        public float Acceleration = 0.1f;
        public float Damping = 0.3f;
        public float BackAndSideDampen = 0.5f;
        public float JumpForce = 0.3f;
        public float RotationAmount = 1.5f;
        public float RotationRatchet = 45.0f;
        public bool HmdResetsY = true;
        public bool HmdRotatesY = true;
        public float GravityModifier = 0.379f;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _transform = GetComponent<Transform>();
            _initialYRotation = _transform.rotation.eulerAngles.y;
        }

        void Update()
        {
            UpdateMovement();
            UpdateController();
            UpdateTransform();
        }

        public virtual void UpdateMovement()
        {
            if (_haltUpdateMovement)
                return;

#if USE_INCONTROL
            var device = InputManager.ActiveDevice;
            var h = device.LeftStickX;
            var v = device.LeftStickY;
            var moveForward = v > 0;
            var moveBack = v < 0;
            var moveLeft = device.DPadLeft.IsPressed || h < 0;
            var moveRight = device.DPadRight.IsPressed || h > 0;
            var run = device.LeftTrigger.IsPressed || device.RightTrigger.IsPressed;
            var curHatLeft = device.LeftBumper.IsPressed;
            var curHatRight = device.RightBumper.IsPressed;
            var primaryAxis = device.LeftStick.Value;
            var secondaryAxis = device.RightStick.Value;
            var jump = device.Action2.WasPressed;

#else
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            var moveForward = v > 0;
            var moveBack = v < 0;
            var moveLeft = h < 0;
            var moveRight = h > 0;
            var run = Input.GetButtonDown("Fire 3");
            var curHatLeft = false;
            var curHatRight = false;
            var primaryAxis = Vector2.zero;
            var secondaryAxis = Vector2.zero;
            var jump = Input.GetButtonDown("Jump");
#endif

            if (jump)
                Jump();

            _moveScale = 1.0f;

            if ((moveForward && moveLeft) || (moveForward && moveRight) || (moveBack && moveLeft) || (moveBack && moveRight))
                _moveScale = 0.70710678f;

            _moveScale *= _simulationRate * Time.deltaTime;

            // Compute this for key movement
            var moveInfluence = Acceleration * 0.1f * _moveScale * _moveScaleMultiplier;

            // Run!
            if (run)
                moveInfluence *= 2.5f;

            var originalRotation = _transform.rotation;
            var originalEuler = originalRotation.eulerAngles;
            originalEuler.z = originalEuler.x = 0f;
            originalRotation = Quaternion.Euler(originalEuler);

            if (moveForward)
                _moveThrottle += originalRotation * (_transform.lossyScale.z * moveInfluence * Vector3.forward);

            else if (moveBack)
                _moveThrottle += originalRotation * (_transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);

            if (moveLeft)
                _moveThrottle += originalRotation * (_transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);

            else if (moveRight)
                _moveThrottle += originalRotation * (_transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);

            var euler = _transform.rotation.eulerAngles;

            if (curHatLeft && !_prevHatLeft)
                euler.y -= RotationRatchet;

            _prevHatLeft = curHatLeft;

            if (curHatRight && !_prevHatRight)
                euler.y += RotationRatchet;

            _prevHatRight = curHatRight;

            var rotateInfluence = _simulationRate * Time.deltaTime * RotationAmount * _rotationScaleMultiplier;

            moveInfluence = _simulationRate * Time.deltaTime * Acceleration * 0.1f * _moveScale * _moveScaleMultiplier;

            if (primaryAxis.y > 0.0f)
                _moveThrottle += originalRotation * (primaryAxis.y * _transform.lossyScale.z * moveInfluence * Vector3.forward);

            if (primaryAxis.y < 0.0f)
                _moveThrottle += originalRotation * (Mathf.Abs(primaryAxis.y) * _transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);

            if (primaryAxis.x < 0.0f)
                _moveThrottle += originalRotation * (Mathf.Abs(primaryAxis.x) * _transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);

            if (primaryAxis.x > 0.0f)
                _moveThrottle += originalRotation * (primaryAxis.x * _transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);

            euler.y += secondaryAxis.x * rotateInfluence;

            _transform.rotation = Quaternion.Euler(euler);
        }

        private void UpdateController()
        {
            var moveDirection = Vector3.zero;
            var motorDamp = (1.0f + (Damping * _simulationRate * Time.deltaTime));

            _moveThrottle.x /= motorDamp;
            _moveThrottle.y = (_moveThrottle.y > 0.0f) ? (_moveThrottle.y / motorDamp) : _moveThrottle.y;
            _moveThrottle.z /= motorDamp;

            moveDirection += _moveThrottle * _simulationRate * Time.deltaTime;

            // Gravity
            if (_controller.isGrounded && _fallSpeed <= 0)
                _fallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));
            else
                _fallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * _simulationRate * Time.deltaTime);

            moveDirection.y += _fallSpeed * _simulationRate * Time.deltaTime;

            // Offset correction for uneven ground
            var bumpUpOffset = 0.0f;

            if (_controller.isGrounded && _moveThrottle.y <= _transform.lossyScale.y * 0.001f)
            {
                bumpUpOffset = Mathf.Max(_controller.stepOffset, new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
                moveDirection -= bumpUpOffset * Vector3.up;
            }

            var predictedXZ = Vector3.Scale((_transform.localPosition + moveDirection), new Vector3(1, 0, 1));

            // Move contoller
            _controller.Move(moveDirection);

            var actualXZ = Vector3.Scale(_transform.localPosition, new Vector3(1, 0, 1));

            if (predictedXZ != actualXZ)
                _moveThrottle += (actualXZ - predictedXZ) / (_simulationRate * Time.deltaTime);
        }

        /// <summary>
        /// Invoked by OVRCameraRig's UpdatedAnchors callback. Allows the Hmd rotation to update the facing direction of the player.
        /// </summary>
        public void UpdateTransform()
        {
            if (HmdRotatesY)
            {
                var centerEye = Camera.main.transform;
                var root = centerEye.parent;
                var prevPos = root.position;
                var prevRot = root.rotation;

                _transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

                root.position = prevPos;
                root.rotation = prevRot;
            }
        }

        /// <summary>
        /// Jump! Must be enabled manually.
        /// </summary>
        public bool Jump()
        {
            if (!_controller.isGrounded)
                return false;

            _moveThrottle += new Vector3(0, _transform.lossyScale.y * JumpForce, 0);

            return true;
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop()
        {
            _controller.Move(Vector3.zero);
            _moveThrottle = Vector3.zero;
            _fallSpeed = 0.0f;
        }

        /// <summary>
        /// Gets the move scale multiplier.
        /// </summary>
        /// <param name="moveScaleMultiplier">Move scale multiplier.</param>
        public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
        {
            moveScaleMultiplier = _moveScaleMultiplier;
        }

        /// <summary>
        /// Sets the move scale multiplier.
        /// </summary>
        /// <param name="moveScaleMultiplier">Move scale multiplier.</param>
        public void SetMoveScaleMultiplier(float moveScaleMultiplier)
        {
            _moveScaleMultiplier = moveScaleMultiplier;
        }

        /// <summary>
        /// Gets the rotation scale multiplier.
        /// </summary>
        /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
        public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
        {
            rotationScaleMultiplier = _rotationScaleMultiplier;
        }

        /// <summary>
        /// Sets the rotation scale multiplier.
        /// </summary>
        /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
        public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
        {
            _rotationScaleMultiplier = rotationScaleMultiplier;
        }

        /// <summary>
        /// Gets the allow mouse rotation.
        /// </summary>
        /// <param name="skipMouseRotation">Allow mouse rotation.</param>
        public void GetSkipMouseRotation(ref bool skipMouseRotation)
        {
            skipMouseRotation = _skipMouseRotation;
        }

        /// <summary>
        /// Sets the allow mouse rotation.
        /// </summary>
        /// <param name="skipMouseRotation">If set to <c>true</c> allow mouse rotation.</param>
        public void SetSkipMouseRotation(bool skipMouseRotation)
        {
            _skipMouseRotation = skipMouseRotation;
        }

        /// <summary>
        /// Gets the halt update movement.
        /// </summary>
        /// <param name="haltUpdateMovement">Halt update movement.</param>
        public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
        {
            haltUpdateMovement = _haltUpdateMovement;
        }

        /// <summary>
        /// Sets the halt update movement.
        /// </summary>
        /// <param name="haltUpdateMovement">If set to <c>true</c> halt update movement.</param>
        public void SetHaltUpdateMovement(bool haltUpdateMovement)
        {
            _haltUpdateMovement = haltUpdateMovement;
        }

        /// <summary>
        /// Resets the player look rotation when the device orientation is reset.
        /// </summary>
        public void ResetOrientation()
        {
            if (HmdResetsY)
            {
                Vector3 euler = _transform.rotation.eulerAngles;
                euler.y = _initialYRotation;
                _transform.rotation = Quaternion.Euler(euler);
            }
        }
    }
}