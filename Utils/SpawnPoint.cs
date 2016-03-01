using UnityEngine;

public sealed class SpawnPoint : MonoBehaviour
{
    public Vector3 size = Vector3.one;
    public Color color = Color.green;
    public PrimitiveType primitiveType = PrimitiveType.Cube;
    public bool isWayPoint = false;
    public bool isLevelEnd = false;
    public bool isSpawnPoint = false;
    public string message = string.Empty;

    void Start()
    {
        if (isSpawnPoint)
            Destroy(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                if (message != string.Empty)
                {
                    var evt = new DisplayMessage(message, true, player.PlayerIndex, 2.5f);
                    Messenger.Notify("ui.message.show", evt);
                }

                if (isWayPoint)
                    player.Waypoint = transform.position;
                else if (isLevelEnd)
                    player.SetLevelCompleted();
            }

            Destroy(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (primitiveType == PrimitiveType.Cube)
            Gizmos.DrawCube(transform.position, size == Vector3.zero ? transform.localScale : size);

        else if (primitiveType == PrimitiveType.Sphere)
            Gizmos.DrawSphere(transform.position, size == Vector3.zero ? transform.localScale.x : size.x);
    }
}
