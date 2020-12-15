using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{

    public float turnSensitivity = 2.0f;
    public float sensitivity = 0.01f;
    public float maxSpeed = 10.0f;

    public SteamVR_Action_Vector2 joystick = null;
    public SteamVR_Action_Vector2 trackpad = null;

    public Transform MovementTransform = null;

    private float speedX = 0.0f;
    private float speedY = 0.0f;
    private float speedZ = 0.0f;

    private CharacterController cc = null;

    private void Awake() {
        cc = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start() {

        if(MovementTransform == null)
        {
            MovementTransform = GameObject.Find("Camera").transform;
        }
    }

    // Update is called once per frame
    void Update() {
        CalculateMovement();
    }
    
    private void CalculateMovement() {
        // Store current position
        Vector3 oldPosition = MovementTransform.position;
       //Quaternion oldRotation = MovementTransform.rotation;

        // Rotation
        var tEuler = new Vector3(0.0f, MovementTransform.eulerAngles.y, 0.0f);

        Vector3 orientationEuler = new Vector3(0, tEuler.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero; 

        Quaternion rotatePlayer = Quaternion.Euler(Vector3.zero);

        // Joystick turn left <-> right
        if (joystick.axis.x <= -0.1 || joystick.axis.x >= 0.1)
        {
            rotatePlayer = Quaternion.Euler(new Vector3(0, joystick.axis.x * Time.deltaTime * turnSensitivity, 0));
        }

        // Joystick up <-> down
        if(joystick.axis.y <= -0.1 || joystick.axis.y >= 0.1)
        {
            speedZ += joystick.axis.y * sensitivity;
            speedZ = Mathf.Clamp(speedZ, -maxSpeed, maxSpeed);

            movement += orientation * (new Vector3(0, speedZ, 0)) * Time.deltaTime;
        }
        else
        {
            speedZ = 0;
        }
        
        // Trackpad left <-> right
        if ((trackpad.axis.x <= -0.1 || trackpad.axis.x >= 0.1) || (trackpad.axis.y <= -0.1 || trackpad.axis.y >= 0.1)) {
            speedX += trackpad.axis.x * sensitivity;
            speedX = Mathf.Clamp(speedX, -maxSpeed, maxSpeed);

            speedY += trackpad.axis.y * sensitivity;
            speedY = Mathf.Clamp(speedY, -maxSpeed, maxSpeed);

            movement += orientation * (new Vector3(speedX, 0, speedY)) * Time.deltaTime;
            Debug.Log("Moving");
        }
        else
        {
            speedX = 0;
            speedY = 0;
        }

        //float l_gravity = (float)(9.81) * Time.deltaTime;
        //if (cc.isGrounded) l_gravity = 0;
        //movement = new Vector3(movement.x, movement.y - l_gravity, movement.z);

        cc.Move(movement);
        cc.transform.rotation *= rotatePlayer;
    }
}
