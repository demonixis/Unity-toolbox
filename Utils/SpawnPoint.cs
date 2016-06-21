using Demonixis.Toolbox;
using UnityEngine;

public sealed class SpawnPoint : MonoBehaviour
{
    public Vector3 size = Vector3.one;
    public Color color = Color.green;
    public bool useSphereRender = false;
    public bool isWayPoint = false;
    public bool isSpawnPoint = false;

    void Start()
    {
        if (isSpawnPoint)
            Destroy(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<SimplePlayer>();
            if (player != null)
            {
                if (isWayPoint)
                    player.SetSpawnPoint(transform.position, transform.rotation);
            }

            Destroy(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (useSphereRender)
            Gizmos.DrawSphere(transform.position, size == Vector3.zero ? transform.localScale.x : size.x);
        else
            Gizmos.DrawCube(transform.position, size == Vector3.zero ? transform.localScale : size);
    }
}
