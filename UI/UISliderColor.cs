using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public sealed class UISliderColor : MonoBehaviour
{
    private Slider _slider;
    public Color fullColor = Color.green;
    public Color middleColor = Color.yellow;
    public Color lowColor = Color.red;
    public Image foreground;

    void Start()
    {
        _slider = GetComponent(typeof(Slider)) as Slider;
        //_slider.onValueChanged.AddListener(OnValueChanged);

        if (foreground == null)
        {
            var imgs = GetComponentsInChildren<Image>();
            foreground = imgs[imgs.Length - 1];
        }
    }

    void Update()
    {
        OnValueChanged(_slider.value);
    }

    private void OnValueChanged(float value)
    {
        if (value <= 0.33f)
            foreground.color = lowColor;
        else if (value <= 0.66f)
            foreground.color = middleColor;
        else
            foreground.color = fullColor;
    }
}
