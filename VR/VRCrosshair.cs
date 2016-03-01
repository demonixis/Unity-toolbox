using Demonixis.Toolbox.Utils;
using DG.Tweening;
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
    public sealed class VRCrosshair : MonoBehaviour
    {
        private List<RaycastResult> _raycasts = null;
        private PointerEventData _pointer = null;
        private EventSystem _eventSystem = null;
        private RectTransform _selected = null;
        private Transform _transform = null;
        private bool _ready = false;
        private Image _crosshair = null;
        private Color _originalColor = Color.white;

        [Header("Animation")]
        [SerializeField]
        private float _normalScale = 0.8f;
        [SerializeField]
        private float _highlightScale = 1.5f;
        [SerializeField]
        private float _highlightTime = 0.65f;
        [SerializeField]
        private float _normalTime = 0.35f;

        [Header("Cursor")]
        [SerializeField]
        private Color _clickColor = new Color(0.95f, 0.9f, 0.1f, 0.3f);

        void Start()
        {
            StartCoroutine(InitializePointer());
        }

        void Update()
        {
            if (!_ready)
                return;

            _eventSystem.RaycastAll(_pointer, _raycasts);

            _selected = GetFirstValidUI();

            if (_selected != null)
            {
                if (Input.GetButtonDown("Fire1"))
                    Click(_selected.gameObject);
                else if (_eventSystem.currentSelectedGameObject != _selected.gameObject)
                    SelectGameObject(_selected.gameObject);
            }
            else if (_eventSystem.currentSelectedGameObject != null)
                SelectGameObject(null);
        }

        private void Click(GameObject selected)
        {
            var uiElement = selected.GetComponent<IPointerClickHandler>();
            if (uiElement == null)
            {
                var toggle = selected.GetComponent(typeof(Toggle)) as Toggle;
                if (toggle == null)
                    toggle = selected.GetComponentInParent(typeof(Toggle)) as Toggle;

                if (toggle != null)
                    toggle.isOn = !toggle.isOn;

                _crosshair.color = _clickColor;

                StartCoroutine(RestoreColor());
            }
            else
                uiElement.OnPointerClick(_pointer);
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

        private IEnumerator InitializePointer()
        {
            yield return new WaitForEndOfFrame();

            var scaling = GameVRSettings.GetRenderScale();
            var screenCenter = new Vector2(Screen.width * 0.5f * scaling, Screen.height * 0.5f * scaling);

            _eventSystem = EventSystem.current;
            _pointer = new PointerEventData(_eventSystem);
            _transform = (Transform)GetComponent(typeof(Transform));
            _transform.localScale = new Vector3(_normalScale, _normalScale, _normalScale);
            _raycasts = new List<RaycastResult>();
            _pointer.position = screenCenter;
            _crosshair = (Image)GetComponent(typeof(Image));
            _originalColor = _crosshair.color;
            _ready = true;

            if (_eventSystem == null)
            {
                Debug.LogError("EventSystem is null");
                Destroy(this);
            }
            else
                Destroy(_eventSystem.GetComponent<InControl.InControlInputModule>());
        }

        private void SelectGameObject(GameObject go)
        {
            _eventSystem.SetSelectedGameObject(go);

            var targetScale = go == null ? _normalScale : _highlightScale;
            _transform.DOScale(targetScale, go == null ? _normalTime : _highlightTime);
        }

        private IEnumerator RestoreColor()
        {
            yield return CoroutineHelper.UnscaledWaitForSeconds(0.6f);

            _crosshair.color = _originalColor;
        }
    }
}