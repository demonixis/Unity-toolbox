using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    public enum RotationAxes
    {
        MouseXAndY = 0, MouseX = 1, MouseY = 2
    }

    private float _rotationY = 0F;
    private Transform _transform;

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;


    void Start()
    {
        _transform = GetComponent(typeof(Transform)) as Transform;

        var rBody = GetComponent(typeof(Rigidbody)) as Rigidbody;

        // Make the rigid body not change rotation
        if (rBody != null)
            rBody.freezeRotation = true;
    }

    void Update()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = _transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX * Time.timeScale;

            _rotationY += Input.GetAxis("Mouse Y") * sensitivityY * Time.timeScale;
            _rotationY = Mathf.Clamp(_rotationY, minimumY, maximumY);

            _transform.localEulerAngles = new Vector3(-_rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            _transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX * Time.timeScale, 0);
        }
        else
        {
            _rotationY += Input.GetAxis("Mouse Y") * sensitivityY * Time.timeScale;
            _rotationY = Mathf.Clamp(_rotationY, minimumY, maximumY);

            _transform.localEulerAngles = new Vector3(-_rotationY, _transform.localEulerAngles.y, 0);
        }
    }
}