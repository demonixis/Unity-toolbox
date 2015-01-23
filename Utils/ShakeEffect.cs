using UnityEngine;
using System.Collections;

public class ShakeEffect : MonoBehaviour 
{   
    public Transform target;
    public float shake = 2.5f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;
	public bool removeAfterEnd = false;
	private Vector3 initialLocalPosition;
	
	void Start()
	{
		if (target == null)
		{
			target = Camera.main.transform;
			initialLocalPosition = target.localPosition;
		}
	}
	
    void Update() 
    {	
        if (shake > 0.0f) 
        {
            target.localPosition = Random.insideUnitSphere * shakeAmount;
            shake -= Time.deltaTime * decreaseFactor;
			
			if (shake <= 0.0f)
			{
				shake = 0.0f;
				target.localPosition = initialLocalPosition;
				
				if (removeAfterEnd)
					Destroy(this);
			}
        }
    }
} 