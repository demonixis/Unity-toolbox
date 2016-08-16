/// GameVRSettings
/// Last Modified Date: 08/16/2016

using System;
using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// Defines the type of SDK.
    /// </summary>
    public enum VRDeviceType
    {
        None = 0,
        GoogleVR,
        UnityVR,
        OSVR
    }

    /// <summary>
    /// The VR Device Manager will create all the structure and add required scripts
    /// to bring VR support to a specific type of HMD. 
    /// </summary>
    [RequireComponent(typeof(GameVRSettings))]
    public abstract class VRDeviceManager : MonoBehaviour, IComparable
    {
        protected Vector3 m_originalHeadPosition = Vector3.zero;
        protected Transform m_headTransform = null;

        [Header("Default VR Settings")]
        [SerializeField]
        protected int m_priority = 0;
        [SerializeField]
        protected bool _fixHeadPosition = false;
        [SerializeField]
        protected Vector3 _headFixAxis = Vector3.up;

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

        /// <summary>
        /// Gets the head position.
        /// </summary>
        public virtual Vector3 HeadPosition
        {
            get { return Camera.main.transform.localPosition; }
        }

        /// <summary>
        /// If the VR Device is managed by Unity, it returns the name given by Unity.
        /// </summary>
        public virtual string UnityVRName
        {
            get { return string.Empty; }
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
        public abstract void SetVREnabled(bool isEnabled);

        /// <summary>
        /// Recenter the HMD position.
        /// </summary>
        public abstract void Recenter();

        /// <summary>
        /// Recenter the head position relative to the initial position.
        /// </summary>
        /// <param name="waitTime">Time to wait before reseting.</param>
        public void RecenterHeadPosition(float waitTime = 0.0f)
        {
            StartCoroutine(ResetHeadPosition(waitTime));
        }

        /// <summary>
        /// Recenter the head position relative to the head position
        /// </summary>
        /// <param name="time">Time to wait before reseting</param>
        /// <returns></returns>
        protected IEnumerator ResetHeadPosition(float time = 0.5f)
        {
            yield return new WaitForSeconds(0.5f);

            var targetPosition = HeadPosition;

            if (_headFixAxis.x < 1)
                targetPosition.x = 0.0f;

            if (_headFixAxis.y < 1)
                targetPosition.y = 0.0f;

            if (_headFixAxis.z < 1)
                targetPosition.z = 0.0f;

            m_headTransform.localPosition = m_originalHeadPosition - targetPosition;
        }

        /// <summary>
        /// Compares the priority of this manager to others.
        /// </summary>
        /// <param name="obj">A VR Manager.</param>
        /// <returns>Returns 1 if the priority of the other manager is higher, -1 if lower, otherwise it returns 0.</returns>
        public virtual int CompareTo(object obj)
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

        protected static void CopyComponent(Component component, GameObject destination, bool destroy = true)
        {
            if (component == null)
                return;

            var type = component.GetType();
            var copy = destination.AddComponent(type);
            var fields = type.GetFields();

            for (int i = 0, l = fields.Length; i < l; i++)
                fields[i].SetValue(copy, fields[i].GetValue(component));

            if (destroy)
                Destroy(component);
        }
    }
}
