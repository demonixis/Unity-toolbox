using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox
{
    [RequireComponent(typeof(AudioSource))]
    public class SimplePlayer : MonoBehaviour
    {
        private AudioListener _audioListener = null;
        protected AudioSource m_audioSource = null;
        protected Transform m_transform = null;
        protected Vector3 m_spawnPosition = Vector3.zero;
        protected Quaternion m_spawnRotation = Quaternion.identity;
        protected int m_score = 0;

        [SerializeField]
        protected int m_health = 100;

        public bool AudioEnabled
        {
            get
            {
                if (_audioListener == null)
                    _audioListener = FindObjectOfType<AudioListener>();

                return _audioListener != null ? _audioListener.enabled : false;
            }
            set
            {
                if (_audioListener == null)
                    _audioListener = FindObjectOfType<AudioListener>();

                if (_audioListener != null)
                    _audioListener.enabled = value;
            }
        }

        public float Volume
        {
            get { return m_audioSource.volume; }
            set { m_audioSource.volume = value; }
        }

        public bool IsPlayingAudio
        {
            get { return m_audioSource.isPlaying; }
        }

        public int Health
        {
            get { return m_health; }
            set { m_health = value; }
        }

        public int MaxHealth
        {
            get; protected set;
        }

        public int Score
        {
            get { return m_score; }
            set { m_health = value; }
        }

        public virtual bool IsDead
        {
            get { return m_health <= 0; }
            set
            {
                if (value)
                    m_health = 0;
            }
        }

        protected virtual void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
            m_audioSource.spatialize = true;
            m_transform = GetComponent<Transform>();
            m_spawnPosition = m_transform.position;
            m_spawnRotation = m_transform.rotation;
            MaxHealth = Health;
        }

        public virtual void SetControlsActive(bool active)
        {
        }

        #region Live Management

        public virtual void SetDamage(int damage)
        {
            m_health -= damage;

            if (m_health < 0)
                m_health = 0;
        }

        public virtual void AddHealth(int value)
        {
            m_health += value;
        }

        public void Die()
        {
            m_health = 0;
        }

        #endregion

        #region Spawn

        public virtual void SetSpawnPoin(Transform spawnPoint)
        {
            SetSpawnPoint(spawnPoint.position, spawnPoint.rotation);
        }

        public void SetSpawnPoint(Vector3 position, Quaternion rotation)
        {
            m_spawnPosition = position;
            m_spawnRotation = rotation;
        }

        public virtual void Respawn(float delay = 1.5f)
        {
            if (delay > 0)
            {
                SetControlsActive(false);
                StartCoroutine(RespawnAfter(delay));
            }
            else
            {
                m_transform.position = m_spawnPosition;
                m_transform.rotation = m_spawnRotation;
            }
        }

        private IEnumerator RespawnAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            Respawn(0);
            SetControlsActive(true);
        }

        #endregion

        #region Sound Management

        public float PlaySound(AudioClip sound)
        {
            if (sound != null)
            {
                m_audioSource.PlayOneShot(sound);
                return sound.length;
            }

            return 0;
        }

        public float PlayRandomSound(ref AudioClip[] sounds, float volume = 1)
        {
            if (sounds != null && sounds.Length > 0)
            {
                var sound = sounds[Random.Range(0, sounds.Length - 1)];
                if (sound == null)
                    return 0;

                m_audioSource.volume = volume;
                m_audioSource.PlayOneShot(sound);
                return sound.length;
            }

            return 0;
        }

        public float PlayMusic(AudioClip music, float volume = 1.0f, bool loop = true)
        {
            if (music != null)
            {
                m_audioSource.clip = music;
                m_audioSource.loop = loop;
                m_audioSource.volume = volume;
                m_audioSource.Play();
                return music.length;
            }

            return 0;
        }

        public float PlayRandomMusic(ref AudioClip[] musics, bool loop, float volume)
        {
            if (musics != null && musics.Length > 0)
            {
                var music = musics[Random.Range(0, musics.Length - 1)];
                return PlayMusic(music, volume, loop);
            }

            return 0;
        }

        public bool IsPlayingClip(AudioClip clip)
        {
            return m_audioSource.clip == clip;
        }

        public void PauseMusic()
        {
            m_audioSource.Pause();
        }

        public void StopMusic()
        {
            m_audioSource.Stop();
        }

        #endregion
    }
}