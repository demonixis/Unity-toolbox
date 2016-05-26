#define USE_DOTTWEEN
#define USE_INCONTROL
#if USE_DOTTWEEN
using DG.Tweening;
#endif
#if USE_INCONTROL
using InControl;
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
    public class VRGazeUI : MonoBehaviour
    {
        protected List<RaycastResult> m_raycasts = null;
        protected PointerEventData m_pointer = null;
        protected EventSystem m_eventSystem = null;
        protected RectTransform m_selected = null;
        protected Transform m_transform = null;
        protected bool _ready = false;
        protected Image m_crosshair = null;
        private Color _originalColor = Color.white;

        [Header("Audio")]
        [SerializeField]
        protected AudioClip _clickSound = null;

        [Header("Cursor")]
        [SerializeField]
        protected float _cursorOverScale = 1.5f;
        [SerializeField]
        protected bool _changeCursorColor = true;
        [SerializeField]
        protected Color _clickColor = new Color(0.95f, 0.9f, 0.1f, 0.3f);

        [Header("Input")]
#if USE_INCONTROL
        [SerializeField]
        protected int[] _validationActions = new int[] { 1 };
        [SerializeField]
        protected int[] _validationBumpers = null;
        [SerializeField]
        protected int[] _validationTriggers = null;
#else
        [SerializeField]
        protected KeyCode[] _validationKeys = new KeyCode[] { KeyCode.Space, KeyCode.Return };
        [SerializeField]
        protected string[] _validationButtons = new string[] { "Fire1", "Jump" };
#endif

        [Header("Settings")]
        [SerializeField]
        protected bool _autoClickEnabled = false;
        [SerializeField]
        protected float _animationTime = 0.65f;

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            InitializeGazeUI();
        }

        protected virtual void Update()
        {
            if (!_ready)
                return;

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

        protected virtual void InitializeGazeUI()
        {
            if (EventSystem.current == null)
                throw new UnityException("[VRGazeUI] EventSystem is null.");

            var scaling = GameVRSettings.RenderScale;
            var screenCenter = new Vector2(Screen.width * 0.5f * scaling, Screen.height * 0.5f * scaling);

            if (UnityEngine.VR.VRSettings.enabled)
            {
                screenCenter.x = UnityEngine.VR.VRSettings.eyeTextureWidth * 0.5f * scaling;
                screenCenter.y = UnityEngine.VR.VRSettings.eyeTextureHeight * 0.5f * scaling;
            }

            m_eventSystem = EventSystem.current;
            m_pointer = new PointerEventData(m_eventSystem);
            m_transform = GetComponent<Transform>();
            m_transform.localScale = Vector3.one;
            m_raycasts = new List<RaycastResult>();
            m_pointer.position = screenCenter;
            m_crosshair = GetComponent<Image>();
            _originalColor = m_crosshair.color;
            _ready = true;

#if USE_INCONTROL
            Destroy(m_eventSystem.GetComponent<InControlInputModule>());
#endif
        }

        protected virtual bool IsInputDown()
        {
            var hasClicked = false;
            var i = 0;
            var size = 0;

#if USE_INCONTROL
            var device = InputManager.ActiveDevice;

            if (_validationActions != null)
            {
                size = _validationActions.Length;
                for (i = 0; i < size; i++)
                {
                    switch (_validationActions[i])
                    {
                        case 1: hasClicked |= device.Action1.WasPressed; break;
                        case 2: hasClicked |= device.Action2.WasPressed; break;
                        case 3: hasClicked |= device.Action3.WasPressed; break;
                        case 4: hasClicked |= device.Action4.WasPressed; break;
                    }
                }
            }

            if (_validationBumpers != null)
            {
                size = _validationBumpers.Length;
                for (i = 0; i < size; i++)
                {
                    switch (_validationBumpers[i])
                    {
                        case 1: hasClicked |= device.LeftBumper.WasPressed; break;
                        case 2: hasClicked |= device.RightBumper.WasPressed; break;
                    }
                }
            }

            if (_validationTriggers != null)
            {
                size = _validationTriggers.Length;
                for (i = 0; i < size; i++)
                {
                    switch (_validationTriggers[i])
                    {
                        case 1: hasClicked |= device.LeftTrigger.WasPressed; break;
                        case 2: hasClicked |= device.RightTrigger.WasPressed; break;
                    }
                }
            }
#else
            if (_validationKeys != null)
            {
                size = _validationKeys.Length;
                for (i = 0; i < size; i++)
                    hasClicked |= Input.GetKeyDown(_validationKeys[i]);
            }

            if (_validationButtons != null)
            {
                size = _validationButtons.Length;
                for (i = 0; i < size; i++)
                    hasClicked |= Input.GetButtonDown(_validationButtons[i]);
            }
#endif

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

                if (_changeCursorColor)
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