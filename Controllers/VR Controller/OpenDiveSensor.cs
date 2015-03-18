using System.Runtime.InteropServices;
using UnityEngine;

//Dive Head Tracking - Copyright by Shoogee GmbH & Co. KG Refer to LICENCE.txt 
//[ExecuteInEditMode]
public class OpenDiveSensor : MonoBehaviour
{
    private Quaternion _rotation;
    private Matrix4x4 _leftProjection;
    private Matrix4x4 _rightProjection;
    private float _aspectRatio;
    private bool _isActive = true;

    // This is used for rotating the camera with another object
    //for example tilting the camera while going along a racetrack or rollercoaster
    public bool addRotationGameObject = false;
    public Transform rotationGameObject = null;
    public Transform target = null;

    // mouse emulation
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public enum UpdateType { Update = 0, FixedUpdate = 1, LateUpdate = 2 }
    public bool emulateMouseInEditor = true;
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public Camera cameraleft;
    public Camera cameraright;
    public UpdateType updateType = UpdateType.Update;
    public float zoom = 0.1f;
    public float IPDCorrection = 0.0f;
    public float znear = 0.1f;
    public float zfar = 10000.0f;
    public int targetFrameRate = 60;

    public bool IsActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
    private float q0, q1, q2, q3;
    private float m0, m1, m2;
#endif

#if UNITY_EDITOR
    private float sensitivityX = 15.0f;
    private float sensitivityY = 15.0f;
    private float minimumY = -90f;
    private float maximumY = 90f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
#elif UNITY_ANDROID
    private AndroidJavaObject mConfig;
    private AndroidJavaObject mWindowManager;

    private float timeSinceLastFullscreen = 0;

	private static AndroidJavaClass javadivepluginclass;
	private static AndroidJavaClass javaunityplayerclass;
	private static AndroidJavaObject currentactivity;
	private static AndroidJavaObject javadiveplugininstance;

	[DllImport("divesensor")]	private static extern void initialize_sensors();
	[DllImport("divesensor")]	private static extern int get_q(ref float q0,ref float q1,ref float q2,ref float q3);
	[DllImport("divesensor")]	private static extern int get_m(ref float m0,ref float m1,ref float m2);
	[DllImport("divesensor")]	private static extern int get_error();
	[DllImport("divesensor")]   private static extern void dive_command(string command);
#elif UNITY_IPHONE
	[DllImport("__Internal")]	private static extern void initialize_sensors();
	[DllImport("__Internal")]	private static extern float get_q0();
	[DllImport("__Internal")]	private static extern float get_q1();
	[DllImport("__Internal")]	private static extern float get_q2();
	[DllImport("__Internal")]	private static extern float get_q3();
	[DllImport("__Internal")]	private static extern void DiveUpdateGyroData();
    [DllImport("__Internal")]	private static extern int get_q(ref float q0,ref float q1,ref float q2,ref float q3);
#endif

    public static void divecommand(string command)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
		dive_command(command);
#endif
    }

    public static void setFullscreen()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
		string answer = javadiveplugininstance.Call<string>("setFullscreen");		
#endif
    }

    void Start()
    {
        if (target == null)
            target = (Transform)GetComponent(typeof(Transform));

        if (addRotationGameObject && rotationGameObject == null)
            rotationGameObject = (Transform)GetComponent(typeof(Transform));

        _leftProjection = Matrix4x4.identity;
        _rightProjection = Matrix4x4.identity;
        _rotation = Quaternion.identity;

        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = targetFrameRate;

#if UNITY_EDITOR

        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

#elif UNITY_ANDROID
        javadivepluginclass = new AndroidJavaClass("com.shoogee.divejava.divejava");
        javaunityplayerclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentactivity = javaunityplayerclass.GetStatic<AndroidJavaObject>("currentActivity");
        javadiveplugininstance = javadivepluginclass.CallStatic<AndroidJavaObject>("instance");
        object[] args = { currentactivity };
        javadiveplugininstance.Call<string>("set_activity", args);

        initialize_sensors();

        string answer;
        answer = javadiveplugininstance.Call<string>("initializeDive");
        answer = javadiveplugininstance.Call<string>("getDeviceType");
        answer = javadiveplugininstance.Call<string>("setFullscreen");

        Network.logLevel = NetworkLogLevel.Full;
#elif UNITY_IPHONE
		initialize_sensors();
#elif UNITY_STANDALONE
        Destroy(this);
#endif
    }

    void Update()
    {
        if (updateType == UpdateType.Update)
            DoUpdate();
    }

    void FixedUpdate()
    {
        if (updateType == UpdateType.FixedUpdate)
            DoUpdate();
    }

    void LateUpdate()
    {
        if (updateType == UpdateType.LateUpdate)
            DoUpdate();
    }

    private void DoUpdate()
    {
        if (!_isActive)
            return;

        _aspectRatio = (Screen.height * 2.0f) / Screen.width;
        setIPDCorrection(IPDCorrection);

#if UNITY_EDITOR
        if (emulateMouseInEditor)
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                rotationX = target.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            }
            else if (axes == RotationAxes.MouseY)
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            }
            else if (axes == RotationAxes.MouseX)
                rotationY = Input.GetAxis("Mouse X") * sensitivityX;

            _rotation = Quaternion.Euler(-rotationY, rotationX, 0.0f);
        }
#elif UNITY_ANDROID
        timeSinceLastFullscreen += Time.deltaTime;

        if (timeSinceLastFullscreen > 8)
        {
            setFullscreen();
            timeSinceLastFullscreen = 0;
        }

        get_q(ref q0, ref q1, ref q2, ref q3);
        //get_m(ref m0, ref m1, ref m2);
        _rotation.x = -q2;
        _rotation.y = q3;
        _rotation.z = -q1;
        _rotation.w = q0;
#elif UNITY_IPHONE
		DiveUpdateGyroData();

		get_q(ref q0,ref q1,ref q2,ref q3);

		_rotation.x = -q2;
		_rotation.y = q3;
		_rotation.z = -q1;
		_rotation.w = q0;	
#endif

        if (addRotationGameObject)
            target.rotation = rotationGameObject.rotation * _rotation;
        else
            target.rotation = _rotation;
    }

    private void setIPDCorrection(float correction)
    {
        // not using camera nearclipplane value because that leads to problems with field of view in different projects
        CreatePerspectiveOffCenter((-zoom + correction) * (znear / 0.1f), (zoom + correction) * (znear / 0.1f), -zoom * (znear / 0.1f) * _aspectRatio, zoom * (znear / 0.1f) * _aspectRatio, znear, zfar, ref _leftProjection);
        CreatePerspectiveOffCenter((-zoom - correction) * (znear / 0.1f), (zoom - correction) * (znear / 0.1f), -zoom * (znear / 0.1f) * _aspectRatio, zoom * (znear / 0.1f) * _aspectRatio, znear, zfar, ref _rightProjection);

        cameraleft.projectionMatrix = _leftProjection;
        cameraright.projectionMatrix = _rightProjection;
    }

    private static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far, ref Matrix4x4 result)
    {
        result[0, 0] = 2.0f * near / (right - left);
        result[0, 1] = 0;
        result[0, 2] = (right + left) / (right - left);
        result[0, 3] = 0;
        result[1, 0] = 0;
        result[1, 1] = 2.0f * near / (top - bottom);
        result[1, 2] = (top + bottom) / (top - bottom);
        result[1, 3] = 0;
        result[2, 0] = 0;
        result[2, 1] = 0;
        result[2, 2] = -(far + near) / (far - near);
        result[2, 3] = -(2.0f * far * near) / (far - near);
        result[3, 0] = 0;
        result[3, 1] = 0;
        result[3, 2] = -1.0f;
        result[3, 3] = 0;
    }
}