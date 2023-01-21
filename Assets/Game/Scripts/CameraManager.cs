using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
	public float speed = 1f;
	public float movementYOffset = 2f;

	public static float downSpeed = 2f;
	private Vector3 initialPosition;
	public static bool shouldResetPosition = false;


	void Start () 
	{
		initialPosition = transform.position;
		StartCoroutine (move ());
	}
	
	private IEnumerator move()
	{
		while (true)
		{
			if (shouldResetPosition == false)
			{
				if (GameManager.Instance.activeBlock != null)
				{
					float yPos = transform.position.y;
					float deltaY = Time.deltaTime * speed;
					float newY = yPos + deltaY;

					float maxMovement = GameManager.Instance.activeBlock.transform.position.y - movementYOffset;

					if (newY <= maxMovement)
					{
						transform.Translate (Vector3.up * deltaY);
					}
				}
			} 
			else
			{
				float yPos = transform.position.y;
				float deltaY = Time.deltaTime * downSpeed;
				float newY = yPos - deltaY;

				if (newY > initialPosition.y)
				{
					transform.Translate (Vector3.down * deltaY);
				} 
				else
				{
					shouldResetPosition = false;
					transform.position = initialPosition;
				}
			}

			yield return 0;
		}
	}
}
