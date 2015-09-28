using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demonixis.VR
{
    public class UVRGazeUI : MonoBehaviour
    {
        private List<RaycastResult> _raycasts;
        private PointerEventData _pointer;
        private EventSystem _eventSystem;
        private RectTransform _cacheTransform;
        private RectTransform _selected;
        private RectTransform _lastSelected;
        private Vector3 _uiTargetScale;
        private bool _canInteract = false;

        public float uiTargetScale = 1.5f;
        public float animationSpeed = 1.5f;
        public float unselectSpeed = 3.5f;
        public float waitBeforeStart = 1.5f;

        void Start()
        {
            _eventSystem = EventSystem.current;
            _pointer = new PointerEventData(_eventSystem);
            _pointer.position = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);
            _raycasts = new List<RaycastResult>();
            _uiTargetScale = new Vector3(uiTargetScale, uiTargetScale, uiTargetScale);

            Invoke("AllowInteraction", waitBeforeStart);
        }

        void Update()
        {
            if (_canInteract)
            {
                EventSystem.current.RaycastAll(_pointer, _raycasts);

                _cacheTransform = GetFirstValidUI();

                if (_cacheTransform != null)
                {
                    if (_selected != _cacheTransform)
                    {
                        if (_lastSelected != null)
                            _lastSelected.localScale = Vector3.one;

                        _lastSelected = _selected;
                        _selected = _cacheTransform;
                    }
                }
                else
                {
                    if (_selected != null)
                    {
                        _lastSelected = _selected;
                        _selected = null;
                    }
                }

                if (_selected != null)
                {
                    _selected.localScale = Vector3.Lerp(_selected.localScale, _uiTargetScale, animationSpeed * Time.deltaTime);

                    if (_uiTargetScale.x - _selected.localScale.x <= 0.1f)
                    {
                        var button = _selected.GetComponent(typeof(Button)) as Button;

                        if (button == null)
                        {
                            var toggle = _selected.GetComponentInParent(typeof(Toggle)) as Toggle;
                            if (toggle != null)
                                toggle.isOn = !toggle.isOn;
                        }
                        else
                            button.OnPointerClick(_pointer);

                        _lastSelected = _selected;
                        _canInteract = false;

                        Invoke("AllowInteraction", 1.0f);
                    }
                }
            }

            if (_lastSelected != null)
            {
                _lastSelected.localScale = Vector3.Lerp(_lastSelected.localScale, Vector3.one, unselectSpeed * Time.deltaTime);

                if (_lastSelected.localScale.x < 1.1f)
                {
                    _lastSelected.localScale = Vector3.one;
                    _lastSelected = null;
                }
            }
        }

        private RectTransform GetFirstValidUI()
        {
            for (int i = 0, l = _raycasts.Count; i < l; i++)
            {
                if (_raycasts[i].gameObject.tag == "GazeUI")
                    return _raycasts[i].gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
            }

            return null;
        }

        private void AllowInteraction()
        {
            _canInteract = true;
        }
    }
}