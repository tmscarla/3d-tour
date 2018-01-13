#pragma strict

var anim:Animator;
var speed:float=10.0F;
var rotationSpeed=100.0F;
function Start () {
	anim=GetComponent.<Animator>();
}

function Update () {
	var translation:float;
	var rotation:float;
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
