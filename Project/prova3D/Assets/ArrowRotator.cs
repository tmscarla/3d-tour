using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotator : MonoBehaviour {
	public float delta = 1.5f;  // Amount to move left and right from the start point
	public float speed = 3.0f; 
	private Vector3 startPos;

	void Start () {
		startPos = transform.position;
		startPos.y += 1.0f;
	}

	void Update () {
		Vector3 v = startPos;
		v.y += delta * Mathf.Sin (Time.time * speed);
		transform.position = v;
		transform.Rotate (0,60*Time.deltaTime,0);
	}
}
