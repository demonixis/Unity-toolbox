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
        if (other.tag == "Player")
        {
            var script = other.GetComponent<Player>();

            if (script != null)
            {
                if (isWayPoint)
                {
                    script.Waypoint = transform.position;
                    
                    if (message != string.Empty)
                        Messenger.Notify("ui.message.show", new GenericMessage<float>(Translation.Get(message), 2.5f));
                }
                else
                    script.Done();
            }

            Destroy(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, size == Vector3.zero ? transform.localScale : size);
    }
}
