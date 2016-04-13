using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.Toolbox.UI
{
    public sealed class UISpinner : MonoBehaviour
    {
        private int _counter;

        [SerializeField]
        private Text _target = null;
        [SerializeField]
        private int _minimum = 0;
        [SerializeField]
        private int _maximum = 255;
        [SerializeField]
        private bool _interactable = true;

        public void Increment()
        {
            _counter++;

            if (_counter > _maximum)
                _counter = _minimum;

            UpdateText();
        }

        public void Decrement()
        {
            _counter--;

            if (_counter < _minimum)
                _counter = _maximum;

            UpdateText();
        }

        public void SetInteractable(bool isInteractable)
        {
            var buttons = GetComponentsInChildren<Button>();
            for (int i = 0, l = buttons.Length; i < l; i++)
                buttons[i].interactable = isInteractable;

            _interactable = isInteractable;
            enabled = _interactable;
        }

        public void UpdateText()
        {
            if (_target != null)
                _target.text = _counter.ToString();
        }
    }
}