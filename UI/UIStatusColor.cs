using UnityEngine;
using UnityEngine.UI;

public sealed class UIStatusColor
{
    private static Color[] StatusColors = new Color[]
    {
        new Color(1.0f, 1.0f, 1.0f, 1.0f),
        new Color(0.8f, 1.0f, 0.8f, 0.65f),
        new Color(0.8f, 1.0f, 0.8f, 0.30f)
    };

    public enum UIStatusColorState
    {
        Normal = 0, Warning, Danger
    }

    private Slider _slider;
    private Image[] _images;
    private int _imgCount;
    private UIStatusColorState _state;

    public UIStatusColor(Slider slider)
    {
        _slider = slider;
        _images = _slider.GetComponentsInChildren<Image>();
        _imgCount = _images.Length;
        _state = UIStatusColorState.Normal;
    }

    public void SetValue(string strValue)
    {
        _slider.value = int.Parse(strValue) * 0.01f;

        if (_slider.value < 0.33f && _state != UIStatusColorState.Danger)
            SetColorAndState(2, UIStatusColorState.Danger);

        else if (_slider.value < 0.66f && _state != UIStatusColorState.Warning)
            SetColorAndState(1, UIStatusColorState.Warning);

        else if (_slider.value > 0.66f && _state != UIStatusColorState.Normal)
            SetColorAndState(0, UIStatusColorState.Normal);
    }

    private void SetColorAndState(int colorIndex, UIStatusColorState state)
    {
        for (int i = 0; i < _imgCount; i++)
            _images[i].color = StatusColors[colorIndex];

        _state = state;
    }
}