using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed = 25.0f;
    private Transform _transform;

    void Start()
    {
        _transform = GetComponent(typeof(Transform)) as Transform;
    }

    void Update()
    {
        _transform.Rotate(axis, Time.deltaTime * speed);
    }
}