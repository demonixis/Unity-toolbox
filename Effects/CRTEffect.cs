using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Demonixis.Effects
{
    [ExecuteInEditMode]
    public class CRTEffect : PostEffectsBase
    {
        public Shader shader = null;
        private Material material;
        public float Distortion = 0.1f;
        public float InputGamma = 2.4f;
        public float OutputGamma = 2.2f;
        public float TextureSize = 768f;

        public override bool CheckResources()
        {
            return shader.isSupported;
        }

        void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if (material == null)
                material = new Material(shader);

            if (material != null)
            {
                material.SetFloat("_Distortion", Distortion);
                material.SetFloat("_InputGamma", InputGamma);
                material.SetFloat("_OutputGamma", OutputGamma);
                material.SetVector("_TextureSize", new Vector2(TextureSize, TextureSize));
                Graphics.Blit(sourceTexture, destTexture, material);
            }
            else
                Graphics.Blit(sourceTexture, destTexture);
        }
    }
}