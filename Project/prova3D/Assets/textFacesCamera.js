#pragma strict

public var cameraToLookAt : Camera;
   
function Start() {
     //transform.Rotate( 180,0,0 );
 }
 
function Update() {
      var v : Vector3 = cameraToLookAt.transform.position - transform.position;
      v.x = v.z = 0.0f;
      transform.LookAt( cameraToLookAt.transform.position - v ); 
      transform.Rotate(0,180,0);
 }