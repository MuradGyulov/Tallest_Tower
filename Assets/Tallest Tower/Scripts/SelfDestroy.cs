using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour 
{
	public float delay = 1f;

	void Update () 
	{
		delay -= Time.deltaTime;

		if (delay <= 0)
		{
			Destroy (gameObject);
		}
	}
}
