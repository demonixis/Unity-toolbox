using UnityEngine;

public sealed class LifeTimeParticle : MonoBehaviour
{
    private float _elapsedTime = 0;
    private float lifeTime = 2.5f;
    public bool disable = false;

    void Start()
    {
        var particle = GetComponent(typeof(ParticleSystem)) as ParticleSystem;
        lifeTime = particle.duration;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= lifeTime)
        {
            if (disable)
            {
                _elapsedTime = 0;
                gameObject.SetActive(false);
            }
            else
                Destroy(gameObject);
        }
    }
}

