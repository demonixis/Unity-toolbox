using UnityEngine;
using System.Collections;

public class ShootHighlighter : MonoBehaviour
{
    private Renderer[] _renderers;
    private int _renderersCount = 0;
    private SimpleTimer _hlTimer;
    private bool _isHighlighted = false;
    public bool hasManyRenderers = true;
    public float highlightDuration = 1.5f;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;

    void Start()
    {
        if (!hasManyRenderers)
        {
            _renderers = new Renderer[0];
            _renderers[0] = GetComponent<Renderer>();
        }
        else
            _renderers = GetComponentsInChildren<Renderer>();

        _renderersCount = _renderers.Length;

        _hlTimer = gameObject.AddComponent<SimpleTimer>();
        _hlTimer.Duration = highlightDuration;
        _hlTimer.Completed += (s, e) => SetNormal();
    }

    public void Highlight()
    {
        if (!_isHighlighted)
        {
            SetColor(false);
            _isHighlighted = true;
        }
    }

    public void SetNormal()
    {
        if (_isHighlighted)
        {
            SetColor(true);
            _isHighlighted = false;
        }
    }

    private void SetColor(bool hl)
    {
        for (int i = 0; i < _renderersCount; i++)
            _renderers[i].material.color = hl ? normalColor : highlightColor;

        _hlTimer.Begin();
    }
}
