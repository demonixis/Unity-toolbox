#define HYDRA_
using UnityEngine;

namespace Demonixis.Toolbox.Controllers
{
    /// <summary>
    /// SimpleHydraRotator: Easily turn an object on X/Y axis.
    /// It's a fork of the Unity's version of SimpleMouseRotator for the Razer Hydra.
    /// </summary>
    public sealed class SimpleHydraRotator : SimpleAbstractRotator
    {
#if HYDRA
        private SixenseInput.Controller _controller;
        private float _deadZone = 0.2f;

        [Header("Sixens Settings")]
        private SixenseHands _hand = SixenseHands.RIGHT;

        public float DeadZone
        {
            get { return _deadZone; }
            set { _deadZone = value; }
        }

        protected override void UpdateInput(ref float horizontal, ref float vertical)
        {
            if (_controller == null)
                _controller = SixenseInput.GetController(_hand);

            if (_controller != null)
            {
                horizontal = _controller.JoystickX * _sensibilityX;
                vertical = _controller.JoystickY * _sensibilityY;
            }

            if (Mathf.Abs(horizontal) < _deadZone)
                horizontal = 0.0f;

            if (Mathf.Abs(vertical) < _deadZone)
                vertical = 0.0f;
        }
#else
        protected override void UpdateInput(ref float horizontal, ref float vertical)
        {
        }
#endif
    }
}