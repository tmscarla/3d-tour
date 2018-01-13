#pragma strict

 var speed : float = 6.0;
    var jumpSpeed : float = 8.0;
    var gravity : float = 20.0;

    private var moveDirection : Vector3 = Vector3.zero;

    function Update() {
        var controller : CharacterController = GetComponent.<CharacterController>();
        if (controller.isGrounded) {
            // We are grounded, so recalculate
            // move direction directly from axes
            moveDirection = Vector3(Input.GetAxis("Horizontal"), 0,
                                    Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButton ("Jump")) {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
    }