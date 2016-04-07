using UnityEngine;

namespace Demonixis.Toolbox.VR
	/// <summary>
	/// OsvrManager - Manages all aspect of the VR from this singleton.
	/// </summary>
	public sealed class OsvrManager : MonoBehaviour
	{
		private static OsvrManager _instance = null;
		private DisplayController _displayController;

		[SerializeField]
		private bool _vrEnabled = true;

		public DisplayController DisplayController
		{
			get
			{
				if (_displayController == null)
					_displayController = FindObjectOfType<DisplayController>();

				return _displayController;
			}
		}

		#region Singleton

		public static OsvrManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<OsvrManager>();

					if (_instance == null)
					{
						var go = new GameObject("OsvrManager");
						_instance = go.AddComponent<OsvrManager>();
					}
				}

				return _instance;
			}
		}

		#endregion

		#region Default Unity pattern

		void Awake()
		{
			CheckInstance();
		}

		void OnEnabled()
		{
			CheckInstance();
		}

		void Start()
		{
			if (_vrEnabled)
				SetVREnabled(true);
		}

		private void CheckInstance()
		{
			if (_instance != null && _instance != this)
				Destroy(this);
			else if (_instance == null)
				_instance = this;
		}

		#endregion

		#region Static Methods

		public static Vector3 GetLocalRotation(byte viewerIndex)
		{
			var displayController = Instance.DisplayController;
			if (displayController)
			{
				var pose3 = displayController.DisplayConfig.GetViewerPose(viewerIndex);
				return new Vector3((float)pose3.translation.x, (float)pose3.translation.y, (float)pose3.translation.z);
			}

			return Vector3.zero;
		}

		public static Quaternion GetLocalRotation(uint viewerIndex)
		{
			var displayController = Instance.DisplayController;
			if (displayController)
			{
				var pose3 = displayController.DisplayConfig.GetViewerPose(viewerIndex);
				return new Quaternion((float)pose3.rotation.x, (float)pose3.rotation.y, (float)pose3.rotation.z, (float)pose3.rotation.w);
			}

			return Quaternion.identity;
		}

		public static void Recenter()
		{
			var manager = Instance;
			var clientKit = ClientKit.instance;
			var displayController = manager.DisplayController;

			if (displayController != null && clientKit != null)
			{
				if (displayController.UseRenderManager)
					displayController.RenderManager.SetRoomRotationUsingHead();
				else
					clientKit.context.SetRoomRotationUsingHead();
			}
		}

		public static bool IsServerConnected()
		{
			var clientKit = ClientKit.instance;
			return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
		}

		public static void SetRenderScale(float scale)
		{
			Debug.Log("[OsvrManager] SetRenderScale not yet supported");
		}

		public static void SetIPD(float ipd)
		{
			var displayController = Instance.DisplayController;

			if (displayController != null && displayController.UseRenderManager)
				displayController.RenderManager.SetIPDMeters(ipd);
		}

		public static void SetVREnabled(bool vrEnabled)
		{
			var clientKit = ClientKit.instance;
			var camera = Camera.main;

			if (camera != null && clientKit != null && clientKit.context != null && clientKit.context.CheckStatus())
			{
				if (vrEnabled)
				{
					camera.transform.parent.gameObject.AddComponent<DisplayController>();
					camera.gameObject.AddComponent<VRViewer>();
				}
				else
				{
					Destroy(FindObjectOfType<DisplayController>());

					var viewers = FindObjectsOfType<VRViewer>();
					for (int i = 0; i < viewers.Length; i++)
						Destroy(viewers[i]);
				}
			}
		}

		#endregion
	}
}