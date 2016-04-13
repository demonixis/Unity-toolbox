using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.Toolbox.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class PlaySoundOnClick : MonoBehaviour
    {
        private Button _button;
        private AudioSource _audio;
        public AudioClip sound;

        void Start()
        {
            if (Camera.main != null)
            {
                _audio = Camera.main.GetComponent(typeof(AudioSource)) as AudioSource;
                if (_audio == null)
                    _audio = (AudioSource)Camera.main.gameObject.AddComponent(typeof(AudioSource));
            }

            _button = GetComponent(typeof(Button)) as Button;
            _button.onClick.AddListener(PlaySound);
        }

        private void PlaySound()
        {
            if (_audio == null)
                AudioSource.PlayClipAtPoint(sound, Vector3.zero);
            else
                _audio.PlayOneShot(sound);
        }
    }
}