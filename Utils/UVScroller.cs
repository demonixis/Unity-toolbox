using UnityEngine;
using System.Collections;

public class UVScroller : MonoBehaviour 
{
    public Vector2 scrollSpeed = new Vector2(0.5f, 0.5f);
    public Material material;
    private Vector2 _offset = Vector2.zero;

    void Update()
    {
        _offset.x = Time.time * scrollSpeed.x;
        _offset.y = Time.time * scrollSpeed.y;
        material.SetTextureOffset("_MainTex", _offset);
    }
}
