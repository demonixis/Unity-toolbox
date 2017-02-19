/// GameVRSettings
/// Last Modified Date: 08/10/2016

#define USE_DOTTWEEN
#if USE_DOTTWEEN
using DG.Tweening;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// Display a crosshair in a world space canvas and use it to interact with the UI.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class VRGazeUI : MonoBehaviour
    {
        public delegate bool IsActionPressedDelegate();

        private List<IsActionPressedDelegate> m_inputCallbacks = new List<IsActionPressedDelegate>(1);
        private List<RaycastResult> m_raycasts = null;
        private PointerEventData m_pointer = null;
        private EventSystem m_eventSystem = null;
        private RectTransform m_selected = null;
        private Transform m_transform = null;
        private Image m_crosshair = null;
        private Color _originalColor = Color.white;

        [Header("Audio")]
        [SerializeField]
        private AudioClip _clickSound = null;

        [Header("Cursor")]
        [SerializeField]
        private float _cursorOverScale = 1.5f;
        [SerializeField]
        private bool _changeCursorColor = true;
        [SerializeField]
        private Color _clickColor = new Color(0.95f, 0.9f, 0.1f, 0.3f);

        [Header("Settings")]
        [SerializeField]
        private bool _autoClickEnabled = false;
        [SerializeField]
        private float _animationTime = 0.65f;

        void Start()
        {
            if (EventSystem.current == null)
                throw new UnityException("[VRGazeUI] EventSystem is null.");

            var scaling = VRManager.RenderScale;
            var screenCenter = new Vector2(Screen.width * 0.5f * scaling, Screen.height * 0.5f * scaling);

#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            if (UnityEngine.VR.VRSettings.enabled)
            {
                screenCenter.x = UnityEngine.VR.VRSettings.eyeTextureWidth * 0.5f * scaling;
                screenCenter.y = UnityEngine.VR.VRSettings.eyeTextureHeight * 0.5f * scaling;
            }
#endif

            m_eventSystem = EventSystem.current;
            m_pointer = new PointerEventData(m_eventSystem);
            m_transform = GetComponent<Transform>();
            m_transform.localScale = Vector3.one;
            m_raycasts = new List<RaycastResult>();
            m_pointer.position = screenCenter;
            m_crosshair = GetComponent<Image>();
            _originalColor = m_crosshair.color;
        }

        void Update()
        {
            m_eventSystem.RaycastAll(m_pointer, m_raycasts);
            m_selected = GetFirstValidUI();

            if (m_selected != null)
            {
                if (IsInputDown())
                    Click(m_selected.gameObject);
                else if (m_eventSystem.currentSelectedGameObject != m_selected.gameObject)
                    SelectGameObject(m_selected.gameObject);
            }
            else if (m_eventSystem.currentSelectedGameObject != null)
                SelectGameObject(null);
        }

        public void SetActive(bool isActive)
        {
            StopAllCoroutines();
            gameObject.SetActive(isActive);
        }

        public void AddInputListener(IsActionPressedDelegate inputFunction)
        {
            if (!m_inputCallbacks.Contains(inputFunction))
                m_inputCallbacks.Add(inputFunction);
        }

        public void RemoveInputListener(IsActionPressedDelegate inputFunction)
        {
            if (m_inputCallbacks.Contains(inputFunction))
                m_inputCallbacks.Remove(inputFunction);
        }

        private bool IsInputDown()
        {
            var hasClicked = false;

            for (int i = 0, l = m_inputCallbacks.Count; i < l; i++)
                hasClicked |= m_inputCallbacks[i]();

            return hasClicked;
        }

        private void Click(GameObject selected)
        {
            var uiElement = selected.GetComponent<IPointerClickHandler>();
            var clicked = uiElement != null;

            if (!clicked)
            {
                var toggle = selected.GetComponent(typeof(Toggle)) as Toggle;
                if (toggle == null)
                    toggle = selected.GetComponentInParent(typeof(Toggle)) as Toggle;

                if (toggle != null)
                {
                    toggle.isOn = !toggle.isOn;
                    clicked = true;
                }
            }
            else
                uiElement.OnPointerClick(m_pointer);

            if (clicked)
            {
                if (_changeCursorColor && gameObject.activeSelf)
                    StartCoroutine(ChangeCursorColor());

                if (_clickSound != null)
                    AudioSource.PlayClipAtPoint(_clickSound, m_transform.position);

                SelectGameObject(null);
            }
        }

        private RectTransform GetFirstValidUI()
        {
            IPointerClickHandler pointer = null;

            for (int i = 0, l = m_raycasts.Count; i < l; i++)
            {
                pointer = m_raycasts[i].gameObject.GetComponent(typeof(IPointerClickHandler)) as IPointerClickHandler;
                if (pointer != null)
                    return m_raycasts[i].gameObject.GetComponent(typeof(RectTransform)) as RectTransform;

                pointer = m_raycasts[i].gameObject.GetComponentInParent(typeof(IPointerClickHandler)) as IPointerClickHandler;
                if (pointer != null)
                    return m_raycasts[i].gameObject.transform.parent.GetComponent(typeof(RectTransform)) as RectTransform;
            }

            return null;
        }

        private void SelectGameObject(GameObject go)
        {
            m_eventSystem.SetSelectedGameObject(go);

            var targetScale = go == null ? 1.0f : _cursorOverScale;

#if USE_DOTTWEEN
            if (DOTween.IsTweening(m_transform))
                DOTween.KillAll();

            m_transform.DOScale(targetScale, go == null ? 0.35f : _animationTime);
#else
            _transform.localScale = new Vector3(targetScale, targetScale, targetScale); 
#endif

            if (go != null && _autoClickEnabled)
                StartCoroutine(TryAutoClick(go));
        }

        private IEnumerator TryAutoClick(GameObject target)
        {
            yield return new WaitForSeconds(_animationTime);

            if (m_selected != null && m_selected.gameObject == target)
                Click(m_selected.gameObject);
        }

        private IEnumerator ChangeCursorColor()
        {
            m_crosshair.color = _clickColor;

            yield return new WaitForSeconds(0.6f);

            m_crosshair.color = _originalColor;
        }
    }
}