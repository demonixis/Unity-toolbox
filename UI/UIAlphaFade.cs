using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIAlphaFade : MonoBehaviour
{
    public static bool Fading = false;
    private Color _color = Color.white;
    public Image overlay;
    public float speed = 1.2f;
    public float minFade = 0.0f;
    public float maxFade = 1.0f;

    public bool IsFading
    {
        get { return Fading; }
    }

    public event EventHandler<EventArgs> Completed = null;

    void Awake()
    {
        Fading = false;
    }

    void Update()
    {
        if (Fading)
        {
            _color.a += Mathf.Clamp01(Time.unscaledDeltaTime * speed);
            overlay.color = _color;

            if (_color.a >= maxFade)
            {
                Fading = false;

                if (Completed != null)
                    Completed(this, EventArgs.Empty);
            }
        }
    }

    public void Reset(bool disable)
    {
        Fading = disable;
        _color = overlay.color;
        _color.a = minFade;
        overlay.color = _color;
        overlay.gameObject.SetActive(disable);
    }

    public void Begin()
    {
        Reset(true);
    }
}
