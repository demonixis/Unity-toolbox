using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlaySoundOnClick : MonoBehaviour 
{
    private Button _button;
    public AudioClip sound;

	void Start () 
    {
        _button = GetComponent(typeof(Button)) as Button;
        _button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        AudioSource.PlayClipAtPoint(sound, Vector3.zero);
    }
}
