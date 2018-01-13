using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

	private static T _instance;
	
	public static T Instance
	{
		get 
		{
			// Check if the instance is null.
			if (_instance == null)
			{
				// First try to find the object already in the scene.
				_instance = GameObject.FindObjectOfType<T>();
				
				if (_instance == null)
				{
					// Couldn't find the singleton in the scene, so make it.
					GameObject singleton = new GameObject(typeof(T).Name);
					_instance = singleton.AddComponent<T>();
				}
			}
			
			return _instance;
		}
	}

	public virtual void Awake()
	{
		if (_instance == null)
		{
			_instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy (gameObject);
		}
	}
}
