#pragma strict

private var m_Character : Controller; // A reference to the ThirdPersonCharacter on the object
private var m_Cam : Transform;                  // A reference to the main camera in the scenes transform
private var m_CamForward : Vector3;             // The current forward direction of the camera
private var m_Move : Vector3;
private var m_Jump : boolean;                      // the world-relative desired move direction, calculated from the camForward and user input.

function Start()
{
    // get the transform of the main camera
    if (Camera.main != null)
    {
        m_Cam = Camera.main.transform;
    }
    else
    {
        Debug.LogWarning(
            "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
        // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
    }

    // get the third person character ( this should never be null due to require component )
    m_Character = GetComponent(Controller);
}


function Update()
{
    if (!m_Jump)
    {
        m_Jump = Input.GetButtonDown("Jump");
    }
}


// Fixed update is called in sync with physics
function FixedUpdate()
{
    // read inputs
    var h : float = Input.GetAxis("Horizontal");
    var v : float = Input.GetAxis("Vertical");
    var crouch : boolean = Input.GetKey(KeyCode.C);

    // calculate move direction to pass to character
    if (m_Cam != null)
    {
        // calculate camera relative direction to move:
        m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
        m_Move = v*m_CamForward + h*m_Cam.right;
    }
    else
    {
        // we use world-relative directions in the case of no main camera
        m_Move = v*Vector3.forward + h*Vector3.right;
    }
#if !MOBILE_INPUT
	// walk speed multiplier
    if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

    // pass all parameters to the character control script
    m_Character.Move(m_Move, crouch, m_Jump);
    m_Jump = false;
}
    

