using UnityEngine;

public class LifeTimeParticle : MonoBehaviour
{
    private float _elapsedTime = 0;
    public float lifeTime = 2.5f;

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= lifeTime)
            Destroy(gameObject);
    }
}
