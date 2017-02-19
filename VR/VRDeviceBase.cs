/// VRDevice
/// Last Modified Date: 01/07/2017

using System;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// The VR Device will create all the structure and add required scripts
    /// to bring VR support to a specific type of HMD. 
    /// </summary>
    [RequireComponent(typeof(VRManager))]
    public abstract class VRDeviceBase : MonoBehaviour, IComparable
    {
        protected Transform m_headTransform = null;

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
        /// Indicates if the device is available.
        /// </summary>
        public abstract bool IsAvailable { get; }

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

        public abstract int EyeTextureWidth
        {
            get;
        }

        public abstract int EyeTextureHeight
        {
            get;
        }

        /// <summary>
        /// Gets the head position.
        /// </summary>
        public virtual Vector3 HeadPosition
        {
            get { return Camera.main.transform.localPosition; }
        }

        /// <summary>
        /// Stops the VR device and destroy the component.
        /// </summary>
        public virtual void Dispose()
        {
            Destroy(this);
        }

        /// <summary>
        /// Enable or disable the VR Mode for the device.
        /// </summary>
        /// <param name="isEnabled">Sets to true to enable and false to disable.</param>
        public abstract void SetActive(bool isEnabled);

        /// <summary>
        /// Recenter the HMD position.
        /// </summary>
        public abstract void Recenter();

        /// <summary>
        /// Compares the priority of this manager to others.
        /// </summary>
        /// <param name="obj">A VR Manager.</param>
        /// <returns>Returns 1 if the priority of the other manager is higher, -1 if lower, otherwise it returns 0.</returns>
        public virtual int CompareTo(object obj)
        {
            var other = obj as VRDeviceBase;

            if (other == null)
                return -1;

            if (other.Priority < Priority)
                return -1;
            else if (other.Priority > Priority)
                return 1;

            return 0;
        }

        protected static Component CopyComponent(Component component, GameObject destination)
        {
            var type = component.GetType();
            var copy = destination.AddComponent(type);
            var fields = type.GetFields();

            for (int i = 0, l = fields.Length; i < l; i++)
                fields[i].SetValue(copy, fields[i].GetValue(component));

            return copy;
        }

        protected static void Destroy<T>(bool multiple = true) where T : MonoBehaviour
        {
            if (multiple)
            {
                var scripts = FindObjectsOfType<T>();
                for (int i = 0; i < scripts.Length; i++)
                    Destroy(scripts[i]);
            }
            else
                Destroy(FindObjectOfType<T>());
        }
    }
}
