using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using UnityEngine;
using System.Globalization;
using Valve.VR;

public class rotateSerial : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM10", 115200);

    public enum handID
    {
        left,
        right
    };
    public handID HandID;

    public Transform rootBone;

    public SteamVR_Action_Single controllerTrigger = null;

    public float rotX = 0;
    public float rotY = 0;
    public float rotZ = 0;

    public bool recenter = false;
    public bool bEnableOffset = false;
    public bool enableRotation = false;

    private bool itemGrabbed = false;
    private Quaternion itemOldOrientation;

    private int sensorAmount = 10;
    private int[] fingerArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        if(controllerTrigger == null) {
            sp.Open();
            sp.ReadTimeout = 14;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sp.IsOpen && controllerTrigger == null)
        {
            try
            {
                serialEvent(sp);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // If recentering orientation
        if (Input.GetKey("c"))
        {
            recenter = true;
        }

        updateJoints();
    }

    // Read serial port
    // FIXME: At the beginning the serial will time out
    void serialEvent(SerialPort port)
    {
        port.Write("f;");
        string myString = port.ReadLine();

        if (Input.GetKey("n"))
        {
            port.Write("cmin;");
            Debug.Log(port.ReadLine());
        }
        if (Input.GetKey("m"))
        {
            port.Write("cmax;");
            Debug.Log(port.ReadLine());
        }

        if (myString != null)
        {

            // Split the string from the glove
            string[] array = myString.Split(',');

            // Parse the finger data
            for (int i = 0; i < sensorAmount; i++)
            {
                fingerArray[i] = int.Parse(array[i], CultureInfo.InvariantCulture.NumberFormat);
            }
        }

    }

    private void updateJoints()
    {
        // Override the values with trigger
        if (controllerTrigger != null)
        {
            for (int i = 0; i < sensorAmount; i++)
            {
                if(HandID == handID.right)
                {
                    fingerArray[i] = (int)(controllerTrigger.GetAxis(SteamVR_Input_Sources.RightHand) * 100);
                }
                else
                {
                    fingerArray[i] = (int)(controllerTrigger.GetAxis(SteamVR_Input_Sources.LeftHand) * 100);
                }
                
            }
        }

        // Rotate the bones
        List<Transform> boneTransformArray = new List<Transform>();

        string HANDNESS = "l";

        string[] FINGERID =
        {
                "thumb",
                "index",
                "middle",
                "ring",
                "pinky"
            };


        for (int i = 0; i < 5; i++)
        {
            if (i != 0)
            {
                boneTransformArray.Add(rootBone.Find($"wrist_l/finger_{FINGERID[i]}_meta_{HANDNESS}/finger_{FINGERID[i]}_0_{HANDNESS}").transform);
                boneTransformArray.Add(rootBone.Find($"wrist_l/finger_{FINGERID[i]}_meta_{HANDNESS}/finger_{FINGERID[i]}_0_{HANDNESS}/finger_{FINGERID[i]}_1_{HANDNESS}").transform);
                boneTransformArray.Add(rootBone.Find($"wrist_l/finger_{FINGERID[i]}_meta_{HANDNESS}/finger_{FINGERID[i]}_0_{HANDNESS}/finger_{FINGERID[i]}_1_{HANDNESS}/finger_{FINGERID[i]}_2_{HANDNESS}").transform);

            }
            else
            {
                // Thumb doesn't have finger_meta
                boneTransformArray.Add(rootBone.Find($"wrist_l/finger_{FINGERID[i]}_0_{HANDNESS}").transform);
                boneTransformArray.Add(rootBone.Find($"wrist_l/finger_{FINGERID[i]}_0_{HANDNESS}/finger_{FINGERID[i]}_1_{HANDNESS}").transform);
                boneTransformArray.Add(rootBone.Find($"wrist_l/finger_{FINGERID[i]}_0_{HANDNESS}/finger_{FINGERID[i]}_1_{HANDNESS}/finger_{FINGERID[i]}_2_{HANDNESS}").transform);
            }

        }

        for (var i = 0; i < 5; i++)
        {

            // The first joint in the thumb needs to be rotated 90 degrees more, I have no idea why
            if (i == 0)
            {
                boneTransformArray[i * 3].localEulerAngles = new Vector3(0, -fingerArray[i * 2] / 2 - 50, -fingerArray[i * 2] / 2 - 144);
            }
            else
            {
                boneTransformArray[i * 3].localEulerAngles = new Vector3(0, 0, -fingerArray[i * 2]);
            }

            boneTransformArray[i * 3 + 1].localEulerAngles = new Vector3(0, 0, -fingerArray[i * 2 + 1]);
            boneTransformArray[i * 3 + 2].localEulerAngles = new Vector3(0, 0, -fingerArray[i * 2 + 1]);
        }

        // Get all of the triggers
        var indexFingerTrigger = GameObject.FindGameObjectsWithTag("FingerTrigger");
        var bIndexColliding = indexFingerTrigger[0].GetComponent<showCollision>().colliding;

        if (bIndexColliding)
        {
            Debug.Log("COLLIDING!!");
            //port.Write("h;");
            indexFingerTrigger[0].GetComponent<showCollision>().colliding = false;
        }

        // Get all of the pickupObjects
        var pickupObjects = GameObject.FindGameObjectsWithTag("PickupObject");
        var pickupObject = pickupObjects[0].GetComponent<pickUp>();

        for (var i = 0; i < pickupObjects.Length-1; i++)
        {
            if (pickupObjects[i].GetComponent<pickUp>().colliding)
            {
                pickupObject = pickupObjects[i].GetComponent<pickUp>();
                break;
            }

        }

        var bHandIsInGrabPose = true;
        for (var i = 0; i < 8; i++)
        {
            if (fingerArray[i] < 70)
            {
                bHandIsInGrabPose = false;
            }
        }

        var grabTrigger = bHandIsInGrabPose;
        if (grabTrigger)
        {
            //Debug.Log("GRAB");
        }

        if (pickupObject.colliding && !itemGrabbed && grabTrigger)
        {
            itemGrabbed = true;
            itemOldOrientation = pickupObject.transform.localRotation;
        }

        if (itemGrabbed && !grabTrigger)
        {
            itemGrabbed = false;
        }

        if (itemGrabbed)
        {
            pickupObject.transform.position = transform.position;
            pickupObject.transform.rotation = transform.rotation;
        }
    }
}