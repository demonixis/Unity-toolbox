using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAlphaFade : MonoBehaviour
{
    private Color _color = Color.white;
    private bool _fading = false;
    public Image overlay;
    public float duration = 0.5f;

    public bool IsFading
    {
        get { return _fading; }
    }

    public event EventHandler<EventArgs> Completed = null;

    void Update()
    {
        if (_fading)
        {
            _color.a += Mathf.Clamp01(Time.unscaledDeltaTime * duration);
            overlay.color = _color;

            if (_color.a >= 1.0f)
            {
                _fading = false;

                if (Completed != null)
                    Completed(this, EventArgs.Empty);
            }
        }
    }

    public void Reset(bool disable)
    {
        _fading = disable;
        _color = overlay.color;
        _color.a = 0.0f;
        overlay.color = _color;
        overlay.gameObject.SetActive(disable);
    }

    public void Begin()
    {
        Reset(true);
    }
}
