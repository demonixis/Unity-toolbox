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

        [SerializeField]
        protected int m_health = 100;

        [SerializeField]
        protected int m_score = 0;

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

        public int Health
        {
            get { return m_health; }
            set { m_health = value; }
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
            m_audioSource = (AudioSource)GetComponent(typeof(AudioSource));
            m_transform = (Transform)GetComponent(typeof(Transform));
            m_spawnPosition = m_transform.position;
            m_spawnRotation = m_transform.rotation;
        }

        public virtual void SetControlsActive(bool active)
        {
        }

        #region Live Management

        public void SetDamage(int damage)
        {
            m_health -= damage;

            if (m_health < 0)
                m_health = 0;
        }

        public void AddHealth(int value)
        {
            m_health = value;
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

        public void PlaySound(AudioClip sound)
        {
            m_audioSource.PlayOneShot(sound);
        }

        public void PlayMusic(AudioClip music, bool loop = true)
        {
            m_audioSource.clip = music;
            m_audioSource.loop = loop;
            m_audioSource.Play();
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