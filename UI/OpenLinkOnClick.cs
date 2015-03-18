using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenLinkOnClick : MonoBehaviour 
{
    private Button _button;
    public string url;

    void Start()
    {
        _button = GetComponent(typeof(Button)) as Button;
        _button.onClick.AddListener(OpenURL);
    }

    void OnDestroy()
    {
        _button.onClick.RemoveListener(OpenURL);
    }

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}
