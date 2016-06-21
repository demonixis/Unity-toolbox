using UnityEngine;

public class FogTransition : MonoBehaviour
{
    public FogMode fogMode = FogMode.Linear;
    public float density = 0.03f;
    public float startDistance = 0;
    public float endDistance = 100;
    public Color fogStartColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);
    public Color fogEndColor = new Color(1.0f, 0.9f, 0.5f, 1.0f);
    public float fogTransition = 0.0f;
    private Color fogTargetColor;

    void Awake()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogDensity = density;
        RenderSettings.fogColor = fogStartColor;
        RenderSettings.fogStartDistance = startDistance;
        RenderSettings.fogEndDistance = endDistance;
    }

    void Start()
    {
        if (fogTransition == 0.0f)
        {
            RenderSettings.fog = false;
            Destroy(this);
        }

        fogTargetColor = fogEndColor;
    }

    void Update()
    {
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogTargetColor, Time.deltaTime * fogTransition);

        if (RenderSettings.fogColor == fogEndColor)
            fogTargetColor = fogStartColor;
        else if (RenderSettings.fogColor == fogStartColor)
            fogTargetColor = fogEndColor;
    }
}