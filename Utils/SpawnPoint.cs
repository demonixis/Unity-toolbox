using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 size = Vector3.one;
    public Color color = Color.green;
    public bool isWayPoint = false;
    public bool isLevelEnd = false;
    public string message = string.Empty;

    void Start()
    {
        if (!isWayPoint && !isLevelEnd)
            Destroy(this);
    }

    void OnTriggerEnter(Collider other)
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, size);
    }
}
