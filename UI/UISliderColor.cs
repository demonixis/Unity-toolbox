using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[ExecuteInEditMode]
public class UISliderColor : MonoBehaviour
{
    private Slider _slider;
    private Image _foreground;
    public Color fullColor = Color.green;
    public Color middleColor = Color.yellow;
    public Color lowColor = Color.red;

    void Start()
    {
        _slider = GetComponent(typeof(Slider)) as Slider;
        //_slider.onValueChanged.AddListener(OnValueChanged);

        var imgs = GetComponentsInChildren<Image>();

        if (imgs.Length == 2)
            _foreground = imgs[1] as Image;
    }

    void Update()
    {
        OnValueChanged(_slider.value);
    }

    private void OnValueChanged(float value)
    {
#if UNITY_EDITOR
        if (_foreground == null)
            return;
#endif
        if (value <= 0.33f)
            _foreground.color = lowColor;
        else if (value <= 0.66f)
            _foreground.color = middleColor;
        else
            _foreground.color = fullColor;
    }
}
