using UnityEngine;
using System.Collections;

public class BlockProperties : MonoBehaviour 
{
	private Vector3 originalSize;

	public void Awake()
	{
		originalSize = GetComponentInChildren<Renderer> ().bounds.size;
	}

	public Vector3 getOriginalSize()
	{
		return originalSize;
	}

	public float getWidth()
	{
		return GetComponentInChildren<Renderer> ().bounds.size.x;
	}

	public float getHeight()
	{
		return GetComponentInChildren<Renderer> ().bounds.size.y;
	}

	public Vector3 getScale()
	{
		return gameObject.transform.localScale;
	}

	public void setScale(Vector3 scale)
	{
		gameObject.transform.localScale = scale;
	}

	public Vector3 getChildScale()
	{
		return gameObject.transform.GetChild (0).gameObject.transform.localScale;
	}
}
