using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YControlCs : MonoBehaviour {

	static Animator anim;
	float  speed=10.0F;
	float rotationSpeed=100.0F;
	void Start () {
		anim=GetComponent<Animator>();
	}

	void  Update () {
		float translation;
		float  rotation;
		translation=Input.GetAxis("Vertical") *speed;
		rotation=Input.GetAxis("Horizontal")* rotationSpeed;
		translation*=Time.deltaTime;
		rotation*=Time.deltaTime;
		transform.Translate(0,0, translation);
		transform.Rotate(0, rotation, 0);


		if(Input.GetButtonDown("Jump")){
			anim.SetTrigger("isJumping");
		}

		if(translation !=0){
			anim.SetBool("isRunning",true);

		}
		else{
			anim.SetBool("isRunning",false);
		}
	}

}
