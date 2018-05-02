using UnityEngine;

namespace Demonixis.Effects
{
    [ImageEffectAllowedInSceneView]
    [ExecuteInEditMode]
    public class CRTEffect : MonoBehaviour
    {
        private bool m_Supported = true;
        private Material m_Material;

        [SerializeField]
        private Shader m_Shader = null;

        public float Distortion = 0.1f;
        public float InputGamma = 2.4f;
        public float OutputGamma = 2.2f;
        public float TextureSize = 768f;

        private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if (m_Material == null)
            {
                m_Material = new Material(m_Shader);

                if (!m_Shader.isSupported)
                {
                    Debug.Log("This shader is not supported.");
                    enabled = false;
                    return;
                }
            }

            if (m_Material != null)
            {
                m_Material.SetFloat("_Distortion", Distortion);
                m_Material.SetFloat("_InputGamma", InputGamma);
                m_Material.SetFloat("_OutputGamma", OutputGamma);
                m_Material.SetVector("_TextureSize", new Vector2(TextureSize, TextureSize));
                Graphics.Blit(sourceTexture, destTexture, m_Material);
            }
            else
                Graphics.Blit(sourceTexture, destTexture);
        }
    }
}