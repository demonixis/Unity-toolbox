using UnityEngine;

namespace Demonixis.VR
{
    [RequireComponent(typeof(Canvas))]
    public class UVRUserInterface : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public int size = 600;

        void Start()
        {
            var cam = UVRManager.SDK.GetComponentInChildren<Camera>();

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localScale.Set(0.02f, 0.02f, 0.02f);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            _rectTransform.position = new Vector3(0.0f, cam.transform.position.y, (float)size / 1000.0f);
        }
    }
}