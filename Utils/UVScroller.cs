using UnityEngine;

namespace UnityToolbox.Utils
{
    public sealed class UVScroller : MonoBehaviour
    {
        public Vector2 scrollSpeed = new Vector2(0.5f, 0.5f);
        public Material material;
        public bool useSharedMaterial = true;
        private Vector2 _offset = Vector2.zero;
        private float _elapsedTime = 0;

        void Start()
        {
            if (material == null)
            {
                var renderer = (Renderer)GetComponent(typeof(Renderer));
                material = useSharedMaterial ? renderer.sharedMaterial : renderer.material;
            }
        }

        void Update()
        {
            _elapsedTime += Time.deltaTime;
            _offset.x = _elapsedTime * scrollSpeed.x;
            _offset.y = _elapsedTime * scrollSpeed.y;
            material.SetTextureOffset("_MainTex", _offset);
        }
    }
}