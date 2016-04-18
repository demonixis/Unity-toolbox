using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public abstract class VRManager : MonoBehaviour
    {
        protected bool vrEnabled = false;

        [SerializeField]
        private bool startInVR = true;

        public bool IsEnabled
        {
            get { return vrEnabled; }
        }

        protected virtual void Awake()
        {
            CheckInstance();
        }

        protected virtual void OnEnabled()
        {
            CheckInstance();
        }

        protected virtual void Start()
        {
            if (startInVR)
                SetVREnabled(true);
        }

        protected abstract void CheckInstance();

        public abstract void SetVREnabled(bool isEnabled);
    }
}
