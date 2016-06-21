#define USE_INCONTROL_
#if USE_INCONTROL
using System;
using InControl;
#else
using UnityEngine;
#endif

namespace Demonixis.Toolbox.Controllers
{
    /// <summary>
    /// SimpleMouseRotator: Easily turn an object on X/Y axis.
    /// It's a fork of the Unity's version of SimpleMouseRotator.
    /// </summary>
    public sealed class SimpleMouseRotator : SimpleAbstractRotator
    {
        protected override void UpdateInput(ref float horizontal, ref float vertical)
        {
#if USE_INCONTROL
            var device = InputManager.ActiveDevice;
            horizontal = device.RightStickX * _sensibilityX;
            vertical = device.RightStickY * _sensibilityY;
#else
            horizontal = Input.GetAxis("Mouse X") * _sensibilityX;
            vertical = Input.GetAxis("Mouse Y") * _sensibilityY;
#endif
        }
    }
}