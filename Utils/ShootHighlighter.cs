using System.Collections;
using UnityEngine;

public sealed class ShootHighlighter : MonoBehaviour
{
    private bool _enabled = true;
    private Renderer[] _renderers;
    private int _renderersCount = 0;
    private bool _isHighlighted = false;
    public bool hasManyRenderers = true;
    public float highlightDuration = 1.5f;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;
    public bool keepRendererColor = false;
    public bool preserveAlpha = true;

    public Renderer[] Renderers
    {
        get { return _renderers; }
    }

    public int CountRenderers
    {
        get { return _renderersCount; }
    }

    void Start()
    {
        if (!hasManyRenderers)
        {
            _renderers = new Renderer[1];
            _renderers[0] = GetComponent<Renderer>();
        }
        else
            _renderers = GetComponentsInChildren<Renderer>();

        _renderersCount = _renderers.Length;

        if (!_renderers[0].material.HasProperty("_Color"))
        {
            _enabled = false;
            return;
        }

        if (keepRendererColor)
            normalColor = _renderers[0].material.color;

        if (preserveAlpha)
            highlightColor.a = normalColor.a;
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

    private IEnumerator SetColor(bool hl)
    {
        if (_enabled)
        {
            for (int i = 0; i < _renderersCount; i++)
                _renderers[i].material.color = hl ? normalColor : highlightColor;
        }

        yield return new WaitForSeconds(highlightDuration);

        SetNormal();
    }
}
