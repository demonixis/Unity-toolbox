using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox.Graphics
{
    [RequireComponent(typeof(Renderer))]
    public sealed class SwapMaterial : MonoBehaviour
    {
        [SerializeField]
        private Material m_material = null;

        [SerializeField]
        private int materialIndex = -1;

        void Start()
        {
#if UNITY_ANDROID
            StartCoroutine(Swap());
#else
            Destroy(this);
#endif
        }

        IEnumerator Swap()
        {
            yield return new WaitForEndOfFrame();

            var renderer = (Renderer)GetComponent(typeof(Renderer));

            if (materialIndex == -1)
                renderer.sharedMaterial = m_material;
            else
                renderer.sharedMaterials[materialIndex] = m_material;

            Destroy(this);
        }
    }
}
