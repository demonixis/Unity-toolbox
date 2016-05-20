using System;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// The VR Device Manager will create all the structure and add required scripts
    /// to bring VR support to a specific type of HMD. 
    /// </summary>
    [RequireComponent(typeof(GameVRSettings))]
    public abstract class VRDeviceManager : MonoBehaviour, IComparable
    {
        [SerializeField]
        protected int m_priority = 0;

        /// <summary>
        /// Gets the check priority of the manager.
        /// </summary>
        public int Priority
        {
            get { return m_priority; }
        }

        /// <summary>
        /// Indicates if the manager is enabled.
        /// </summary>
        public abstract bool IsEnabled { get; }

        /// <summary>
        /// Indicates if the device is available.
        /// </summary>
        public abstract bool IsPresent { get; }

        /// <summary>
        /// Gets or sets the type of device.
        /// </summary>
        public abstract VRDeviceType VRDeviceType { get; }

        /// <summary>
        /// Gets or sets the render scale.
        /// </summary>
        public abstract float RenderScale
        {
            get; set;
        }

        /// <summary>
        /// Stops the VR device and destroy the component.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Enable or disable the VR Mode for the device.
        /// </summary>
        /// <param name="isEnabled">Sets to true to enable and false to disable.</param>
        public abstract void SetVREnabled(bool isEnabled);

        /// <summary>
        /// Recenter the HMD position.
        /// </summary>
        public abstract void Recenter();

        /// <summary>
        /// Compares the priority of this manager to others.
        /// </summary>
        /// <param name="obj">A VR Manager.</param>
        /// <returns>Returns 1 if the priority of the other manager is higher, -1 if lower, otherwise it returns 0.</returns>
        public int CompareTo(object obj)
        {
            var other = obj as VRDeviceManager;

            if (other == null)
                return -1;

            if (other.Priority < Priority)
                return -1;
            else if (other.Priority > Priority)
                return 1;

            return 0;
        }
    }
}
