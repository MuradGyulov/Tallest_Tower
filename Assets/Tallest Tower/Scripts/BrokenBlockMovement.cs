using UnityEngine;
using System.Collections;

public class BrokenBlockMovement : MonoBehaviour 
{
	public float speed = 2f;

	void Start() 
	{
		StartCoroutine (move ());
	}

	private IEnumerator move()
	{
		while (true)
		{
			float delta = Time.deltaTime * speed;
			transform.Translate (Vector3.down * delta);

			yield return 0;
		}
	}
}
