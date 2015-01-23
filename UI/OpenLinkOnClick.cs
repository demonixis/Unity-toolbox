using UnityEngine;

public class OpenLinkOnClick : MonoBehaviour 
{
    public string url;

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}