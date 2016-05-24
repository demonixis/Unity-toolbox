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
        private List<RaycastResult> _raycasts = null;
        private PointerEventData _pointer = null;
        private EventSystem _eventSystem = null;
        private RectTransform _selected = null;
        private Transform _transform = null;
        private bool _ready = false;
        private Image _crosshair = null;
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

        [Header("Input")]
#if USE_INCONTROL
        [SerializeField]
        private int[] _validationActions = new int[] { 1 };
        [SerializeField]
        private int[] _validationBumpers = null;
        [SerializeField]
        private int[] _validationTriggers = null;
#else
        [SerializeField]
        private KeyCode[] _validationKeys = new KeyCode[] { KeyCode.Space, KeyCode.Return };
        [SerializeField]
        private string[] _validationButtons = new string[] { "Fire1", "Jump" };
#endif

        [Header("Settings")]
        [SerializeField]
        private bool _autoClickEnabled = false;
        [SerializeField]
        private float _animationTime = 0.65f;

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if (EventSystem.current == null)
                throw new UnityException("[VRGazeUI] EventSystem is null.");

            var scaling = GameVRSettings.RenderScale;
            var screenCenter = new Vector2(Screen.width * 0.5f * scaling, Screen.height * 0.5f * scaling);

            _eventSystem = EventSystem.current;
            _pointer = new PointerEventData(_eventSystem);
            _transform = GetComponent<Transform>();
            _transform.localScale = Vector3.one;
            _raycasts = new List<RaycastResult>();
            _pointer.position = screenCenter;
            _crosshair = GetComponent<Image>();
            _originalColor = _crosshair.color;
            _ready = true;

#if USE_INCONTROL
            Destroy(_eventSystem.GetComponent<InControlInputModule>());
#endif
        }

        void Update()
        {
            if (!_ready)
                return;

            _eventSystem.RaycastAll(_pointer, _raycasts);
            _selected = GetFirstValidUI();

            if (_selected != null)
            {
                if (IsInputDown())
                    Click(_selected.gameObject);
                else if (_eventSystem.currentSelectedGameObject != _selected.gameObject)
                    SelectGameObject(_selected.gameObject);
            }
            else if (_eventSystem.currentSelectedGameObject != null)
                SelectGameObject(null);
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
                uiElement.OnPointerClick(_pointer);

            if (clicked)
            {

                if (_changeCursorColor)
                    StartCoroutine(ChangeCursorColor());

                if (_clickSound != null)
                    AudioSource.PlayClipAtPoint(_clickSound, _transform.position);

                SelectGameObject(null);
            }
        }

        private RectTransform GetFirstValidUI()
        {
            IPointerClickHandler pointer = null;

            for (int i = 0, l = _raycasts.Count; i < l; i++)
            {
                pointer = _raycasts[i].gameObject.GetComponent(typeof(IPointerClickHandler)) as IPointerClickHandler;
                if (pointer != null)
                    return _raycasts[i].gameObject.GetComponent(typeof(RectTransform)) as RectTransform;

                pointer = _raycasts[i].gameObject.GetComponentInParent(typeof(IPointerClickHandler)) as IPointerClickHandler;
                if (pointer != null)
                    return _raycasts[i].gameObject.transform.parent.GetComponent(typeof(RectTransform)) as RectTransform;
            }

            return null;
        }

        private void SelectGameObject(GameObject go)
        {
            _eventSystem.SetSelectedGameObject(go);

            var targetScale = go == null ? 1.0f : _cursorOverScale;

#if USE_DOTTWEEN
            if (DOTween.IsTweening(_transform))
                DOTween.KillAll();

            _transform.DOScale(targetScale, go == null ? 0.35f : _animationTime);
#else
            _transform.localScale = new Vector3(targetScale, targetScale, targetScale); 
#endif

            if (go != null && _autoClickEnabled)
                StartCoroutine(TryAutoClick(go));
        }

        private IEnumerator TryAutoClick(GameObject target)
        {
            yield return new WaitForSeconds(_animationTime);

            if (_selected != null && _selected.gameObject == target)
                Click(_selected.gameObject);
        }

        private IEnumerator ChangeCursorColor()
        {
            _crosshair.color = _clickColor;

            yield return new WaitForSeconds(0.6f);

            _crosshair.color = _originalColor;
        }
    }
}