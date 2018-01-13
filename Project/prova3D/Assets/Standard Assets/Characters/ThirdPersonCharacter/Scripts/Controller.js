#pragma strict

var m_MovingTurnSpeed : float = 360;
var m_StationaryTurnSpeed : float = 180;
var m_JumpPower : float = 12f;
@Range(1.0, 4.0) var m_GravityMultiplier : float  = 2f;
var m_RunCycleLegOffset : float  = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
var m_MoveSpeedMultiplier : float  = 1f;
var m_AnimSpeedMultiplier : float  = 1f;
var m_GroundCheckDistance : float  = 0.1f;

private var m_Rigidbody : Rigidbody;
private var m_Animator : Animator;
private var m_IsGrounded : boolean;
private var m_OrigGroundCheckDistance : float;
private var k_Half : float  = 0.5f;
private var m_TurnAmount : float ;
private var m_ForwardAmount : float ;
private var m_GroundNormal : Vector3;
private var m_CapsuleHeight : float ;
private var m_CapsuleCenter : Vector3;
private var m_Capsule : CapsuleCollider;
private var m_Crouching : boolean;


function Start()
{
	m_Animator = GetComponent(Animator);
	m_Rigidbody = GetComponent(Rigidbody);
	m_Capsule = GetComponent(CapsuleCollider);
	m_CapsuleHeight = m_Capsule.height;
	m_CapsuleCenter = m_Capsule.center;

	m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	m_OrigGroundCheckDistance = m_GroundCheckDistance;
}


function Move(move : Vector3, crouch : boolean, jump : boolean)
{

	// convert the world relative moveInput vector into a local-relative
	// turn amount and forward amount required to head in the desired
	// direction.
	if (move.magnitude > 1f) move.Normalize();
	move = transform.InverseTransformDirection(move);
	CheckGroundStatus();
	move = Vector3.ProjectOnPlane(move, m_GroundNormal);
	m_TurnAmount = Mathf.Atan2(move.x, move.z);
	m_ForwardAmount = move.z;

	ApplyExtraTurnRotation();

	// control and velocity handling is different when grounded and airborne:
	if (m_IsGrounded)
	{
		HandleGroundedMovement(crouch, jump);
	}
	else
	{
		HandleAirborneMovement();
	}

	ScaleCapsuleForCrouching(crouch);
	PreventStandingInLowHeadroom();

	// send input and other state parameters to the animator
	UpdateAnimator(move);
}


function ScaleCapsuleForCrouching(crouch : boolean)
{
	if (m_IsGrounded && crouch)
	{
		if (m_Crouching) return;
		m_Capsule.height = m_Capsule.height / 2f;
		m_Capsule.center = m_Capsule.center / 2f;
		m_Crouching = true;
	}
	else
	{
		var crouchRay : Ray = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
		var crouchRayLength : float = m_CapsuleHeight - m_Capsule.radius * k_Half;
		if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength))
		{
			m_Crouching = true;
			return;
		}
		m_Capsule.height = m_CapsuleHeight;
		m_Capsule.center = m_CapsuleCenter;
		m_Crouching = false;
	}
}

function PreventStandingInLowHeadroom()
{
	// prevent standing up in crouch-only zones
	if (!m_Crouching)
	{
		var crouchRay : Ray = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
		var crouchRayLength : float = m_CapsuleHeight - m_Capsule.radius * k_Half;
		if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength))
		{
			m_Crouching = true;
		}
	}
}


function UpdateAnimator(move : Vector3)
{
	// update the animator parameters
	m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
	m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
	m_Animator.SetBool("Crouch", m_Crouching);
	m_Animator.SetBool("OnGround", m_IsGrounded);
	if (!m_IsGrounded)
	{
		m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
	}

	// calculate which leg is behind, so as to leave that leg trailing in the jump animation
	// (This code is reliant on the specific run cycle offset in our animations,
	// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
	var runCycle : float = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
	var jumpLeg : float = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
	
	if (m_IsGrounded)
	{
		m_Animator.SetFloat("JumpLeg", jumpLeg);
	}

	// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
	// which affects the movement speed because of the root motion.
	if (m_IsGrounded && move.magnitude > 0)
	{
		m_Animator.speed = m_AnimSpeedMultiplier;
	}
	else
	{
		// don't use that while airborne
		m_Animator.speed = 1;
	}
}


function HandleAirborneMovement()
{
	// apply extra gravity from multiplier:
	var extraGravityForce : Vector3= (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
	m_Rigidbody.AddForce(extraGravityForce);

	m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
}


function HandleGroundedMovement(crouch : boolean, jump : boolean)
{
	// check whether conditions are right to allow a jump:
	if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
	{
		// jump!
		m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
		m_IsGrounded = false;
		m_Animator.applyRootMotion = false;
		m_GroundCheckDistance = 0.1f;
	}
}

function ApplyExtraTurnRotation()
{
	// help the character turn faster (this is in addition to root rotation in the animation)
	var turnSpeed : float = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
	transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
}


function OnAnimatorMove()
{
	// we implement this function to override the default root motion.
	// this allows us to modify the positional speed before it's applied.
	if (m_IsGrounded && Time.deltaTime > 0)
	{
		var v : Vector3= (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

		// we preserve the existing y part of the current velocity.
		v.y = m_Rigidbody.velocity.y;
		m_Rigidbody.velocity = v;
	}
}


function CheckGroundStatus()
{
	var hitInfo : RaycastHit;
#if UNITY_EDITOR
	// helper to visualise the ground check ray in the scene view
	Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
	// 0.1f is a small offset to start the ray from inside the character
	// it is also good to note that the transform position in the sample assets is at the base of the character
	if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, hitInfo, m_GroundCheckDistance))
	{
		m_GroundNormal = hitInfo.normal;
		m_IsGrounded = true;
		m_Animator.applyRootMotion = true;
	}
	else
	{
		m_IsGrounded = false;
		m_GroundNormal = Vector3.up;
		m_Animator.applyRootMotion = false;
	}
}
