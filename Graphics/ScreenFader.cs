using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Demonixis.Toolbox.Graphics
{
    [RequireComponent(typeof(Camera))]
    public sealed class ScreenFader : MonoBehaviour
    {
        public enum FadeMode
        {
            FadeIn = 0, FadeOut
        }

        private static ScreenFader instance = null;
        private bool m_enabled = false;
        private short m_sign = 1;
        private CommandBuffer m_fadeCommand = null;
        private MaterialPropertyBlock m_fadePropertyBlock = null;
        private int m_materialFadeID = 0;
        private Mesh m_planeMesh;
        private float m_alpha = 1.0f;

        [SerializeField]
        private float m_fadeSpeed = 2.5f;
        [SerializeField]
        private Material FadeMaterial = null;
        [SerializeField]
        private bool _fadeOnStart = true;

        public static ScreenFader Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<ScreenFader>();

                return instance;
            }
        }

        public event Action<int, float> FadeUpdate = null;
        public event Action FadeCompleted = null;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("Only one ScreenFader component is allowed.");
                DestroyImmediate(this);
                return;
            }

            var verts = new Vector3[]
            {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, -1, 0)
            };

            m_planeMesh = new Mesh
            {
                vertices = verts,
                triangles = new int[] { 0, 1, 2, 0, 2, 3 }
            };
            m_planeMesh.RecalculateBounds();

            m_materialFadeID = Shader.PropertyToID("_Fade");
            m_fadeCommand = new CommandBuffer();
            m_fadePropertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            if (_fadeOnStart)
                Fade(-1);
        }

        private void Update()
        {
            if (!m_enabled)
                return;

            m_alpha += m_fadeSpeed * (Time.deltaTime > 0 ? Time.deltaTime : Time.unscaledDeltaTime) * m_sign;

            if (FadeUpdate != null)
                FadeUpdate.Invoke(m_sign, m_alpha);

            if ((m_sign == 1 && m_alpha >= 1) || (m_sign == -1 && m_alpha <= 0))
            {
                m_alpha = m_sign == 1 ? 1 : 0;

                if (m_alpha <= 0)
                    m_enabled = false;

                if (FadeCompleted != null)
                    FadeCompleted.Invoke();
            }
        }

        private void OnPostRender()
        {
            if (!m_enabled)
                return;

            var local = Matrix4x4.TRS(Vector3.forward * 0.3f, Quaternion.identity, Vector3.one);
            m_fadePropertyBlock.SetFloat(m_materialFadeID, m_alpha);
            m_fadeCommand.Clear();
            m_fadeCommand.DrawMesh(m_planeMesh, transform.localToWorldMatrix * local, FadeMaterial, 0, 0, m_fadePropertyBlock);
            UnityEngine.Graphics.ExecuteCommandBuffer(m_fadeCommand);
        }

        public void StartFade(short sign)
        {
            m_sign = sign;
            m_alpha = m_sign == 1 ? 0 : 1;
            m_enabled = true;
        }

        public void StopFade()
        {
            m_alpha = m_sign == 1 ? 0 : 1;
            m_enabled = false;
        }

        public void ResetFade(bool fill)
        {
            m_alpha = fill ? 1.0f : 0.0f;
            m_enabled = false;
        }

        public static void FadeIn(Action completed = null)
        {
            Fade(-1, 2.5f, completed);
        }

        public static void FadeIn(float speed = 2.5f, Action completed = null)
        {
            Fade(-1, speed, completed);
        }

        public static void FadeOut(Action completed = null)
        {
            Fade(1, 2.5f, completed);
        }

        public static void FadeOut(float speed, Action completed = null)
        {
            Fade(1, speed, completed);
        }

        private static void Fade(short sign, float fadeSpeed = 0.5f, Action completed = null)
        {
            var fade = Instance;
            fade.m_fadeSpeed = fadeSpeed;
            fade.FadeCompleted = completed;
            fade.StartFade(sign);
        }
    }
}