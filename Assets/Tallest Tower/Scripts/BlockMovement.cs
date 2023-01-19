using UnityEngine;
using System.Collections;

public class BlockMovement : MonoBehaviour 
{
	public float speed = 2f;
	public float speedIncrement = 1f;
	public float maxSpeed = 6f;
	public float maxMovementLeft = -2f;
	public float maxMovementRight = 2f;

	[HideInInspector]
	public int direction = 1;
	[HideInInspector]
	public bool shouldMove = false;
	[HideInInspector]
	public bool shouldIncreaseSpeed = false;

	void OnEnable()
	{
		GameManager.onGamePausedEvent += stopMovement;
	}

	void OnDisable()
	{
		GameManager.onGamePausedEvent -= stopMovement;
	}

	void Start() 
	{
		StartCoroutine (move ());
	}

	private IEnumerator move()
	{
		while (true)
		{
			if (shouldMove)
			{
				float xPos = transform.position.x;
				float deltaX = Time.deltaTime * speed * direction;
				float newX = xPos + deltaX;

				if (newX > maxMovementRight || newX < maxMovementLeft)
				{
					deltaX = 0;
					direction *= -1;
					if (shouldIncreaseSpeed && speed < maxSpeed)
					{
						speed += speedIncrement;
					}
				}
				
				transform.Translate (Vector3.right * deltaX);
			}

			yield return 0;
		}
	}

	public void startMovement()
	{
		shouldMove = true;
	}


	public void stopMovement()
	{
		shouldMove = false;
	}
}
