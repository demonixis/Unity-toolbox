using UnityEngine;
using System.Collections;

public class SixDOFController : MonoBehaviour 
{
	// Cache
	private Rigidbody _rigidbody;
	private Vector3 _translation;
	private Vector3 _rotation;

	// Speed
	public float translationSpeed = 165.0f;
	public float rotationSpeed = 45.0f;
	public float rollSpeed = 0.5f;
	
	// Input axis
	public string rotationXAxis = "Mouse Y";
	public string rotationYAxis = "Mouse X";
	public string rotationZAxis = "Roll";
	public string translationXAxis = "Horizontal";
	public string translationYAxis = "MoveTop";
	public string translationZAxis = "Vertical";

	void Start () 
	{
		_rigidbody = GetComponent<Rigidbody>();
		Screen.lockCursor = true;
		Cursor.visible = false;
	}

    void Update()
    {
        _rotation.x = -Input.GetAxis(rotationXAxis);
        _rotation.y = Input.GetAxis(rotationYAxis);
        _rotation.z = Input.GetAxis(rotationZAxis) * rollSpeed;

        _translation.x = Input.GetAxis(translationXAxis);
        _translation.y = Input.GetAxis(translationYAxis);
        _translation.z = Input.GetAxis(translationZAxis);
    }

	void FixedUpdate()
	{
		_rigidbody.AddRelativeForce(_translation * translationSpeed);
		_rigidbody.AddRelativeTorque(_rotation * rotationSpeed);

	}
}