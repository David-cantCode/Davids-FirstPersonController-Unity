using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //general 
    private bool isMoving = false;
    private float xRotation = 0f;
    private float speed;
    private Vector3 velocity;

    //Head bob
    private float bobFrequency = 1.2f;
    private float bobAmplitude = 0.16f;
    private float tBob = 0.0f;

    //FOV variables
    public float baseFOV = 75.0f;
    public float fovChange = 2.5f;

    //settings
    private float walkSpeed = 3.0f;
    private float sprintSpeed = 6.0f;
    private float jumpVelocity = 5.8f;
    private float sensitivity = 2.0f;
    private float gravity = 14.8f;

    //objs 
    private CharacterController characterController;
    public Transform head;
    public Camera playerCamera;



void Start()
{
    playerCamera = head.GetComponentInChildren<Camera>();
    characterController = GetComponent<CharacterController>();
}




//************************************************************************
//**************************CAMERA STUFF ********************************
//*************************************************************************
private void HandleMouseLook()
{
    float mouseX = Input.GetAxis("Mouse X") * sensitivity;
    float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

    //rotate horizontal
    head.Rotate(Vector3.up, mouseX);

    //rotate vertical
    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -80f, 60f);

    playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
}

private void HandleHeadBob()
{
    if (characterController.isGrounded && isMoving)
    {
        tBob += Time.deltaTime * velocity.magnitude;
        Vector3 bobOffset = HeadBob(tBob);
        playerCamera.transform.localPosition = bobOffset;
    }
    else
    {
        //reset the head position when not moving
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.zero, Time.deltaTime * 10f);
    }
}

private Vector3 HeadBob(float time)
{
    Vector3 pos = Vector3.zero;
    pos.y = Mathf.Sin(time * bobFrequency) * bobAmplitude;
    pos.x = Mathf.Cos(time * bobFrequency / 2) * bobAmplitude;
    return pos;
}


private void HandleFOV()
{
    float velocityClamped = Mathf.Clamp(velocity.magnitude, 0.5f, sprintSpeed * 2);
    float targetFOV = baseFOV + fovChange * velocityClamped;
    playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 8.0f);
}








//************************************************************************
//********************************MOVEMENT********************************
//*************************************************************************

    private void HandleMovement()
    {
        
        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpVelocity;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = sprintSpeed;
            }
            else
            {
                speed = walkSpeed;
            }

            Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 direction = (head.forward * inputDir.y + head.right * inputDir.x).normalized;
            isMoving = inputDir.magnitude > 0.1f;
            velocity.x = direction.x * speed;
            velocity.z = direction.z * speed;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }






    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleHeadBob();
        HandleFOV();

    }



    
}
