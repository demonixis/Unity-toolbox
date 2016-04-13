using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.Toolbox.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class OpenLinkOnClick : MonoBehaviour
    {
        private Button _button;
        public string url;

        void Start()
        {
            _button = GetComponent(typeof(Button)) as Button;
            _button.onClick.AddListener(OpenURL);
        }

        void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OpenURL);
        }

        public void OpenURL()
        {
            Application.OpenURL(url);
        }
    }
}