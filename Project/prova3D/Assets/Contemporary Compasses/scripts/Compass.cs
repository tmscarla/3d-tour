using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public Transform player;

	void Start(){

	}

	void Update(){
		transform.rotation = Quaternion.AngleAxis(player.eulerAngles.y, Vector3.forward);
	}
}
