using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
	private static  T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				// First try to find the object already in the scene.
				_instance = GameObject.FindObjectOfType<T> ();

				if (_instance == null)
				{
					// Couldn't find the singleton in the scene, so make it.
					GameObject singleton = new GameObject (typeof(T).Name);
					_instance = singleton.AddComponent<T> ();
				}
			}	

			return _instance;
		}
	}

	public virtual void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

		if (_instance == null)
		{
			// If the instance is null, create or find it.
			_instance = this as T;
			// Do not destroy it while changing scene.
			DontDestroyOnLoad (gameObject);
		} 
		else
		{
			// If the instance is already exists, destoy this one.
			Destroy (gameObject);
		}

	}
}