using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using UnityEngine;
using System.Globalization;

public class rotateSerial : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM10", 115200);

    public Transform rootBone;

    public float rotX = 0;
    public float rotY = 0;
    public float rotZ = 0;

    public bool recenter = false;
    public bool bEnableOffset = false;
    public bool enableRotation = false;

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 14;
    }

    // Update is called once per frame
    void Update()
    {
        if(sp.IsOpen) {
            try{
                serialEvent(sp);
            }catch(System.Exception) {
                throw;
            }
        }

        // If recentering orientation
        if(Input.GetKey("c")) {
            recenter = true;
        }
    }

    // Read serial port
    // FIXME: At the beginning the serial will time out
    void serialEvent(SerialPort port) {
        port.Write("f;");
        string myString = port.ReadLine();

        if(myString != null) {

            // Split the string from the glove
            string[] array = myString.Split(',');

            // Parse the finger data
            int finger1 = int.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
            int finger2 = int.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
            int finger3 = int.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);

            /*
            float w = float.Parse(array[3], CultureInfo.InvariantCulture.NumberFormat);
            float y = float.Parse(array[4], CultureInfo.InvariantCulture.NumberFormat);
            float x = float.Parse(array[5], CultureInfo.InvariantCulture.NumberFormat);
            float z = float.Parse(array[6], CultureInfo.InvariantCulture.NumberFormat);
            

            //Quaternion rotation = new Quaternion(x, y, z, w);
            Quaternion rotation = new Quaternion(-w, -y, -z, x);

            // If recentering, save the current orientation to rotX, rotY and rotZ
            if(recenter) {
                rotZ = -rotation.eulerAngles.x;
                rotY = -rotation.eulerAngles.y;
                rotX = -rotation.eulerAngles.z;

                recenter = false;

                // Print some debug info
                Debug.Log("Recentered orientation!");

                Debug.Log("Current:");
                Debug.Log(rotation.eulerAngles);
                Debug.Log("New:");
                Debug.Log(rotX);
                Debug.Log(rotY);
                Debug.Log(rotZ);
            }

            // Set an offset for the hand with the recenter function
            if(rotX != 0 && rotY != 0 && rotZ != 0 && bEnableOffset == true) {
                Quaternion offsetAngle = Quaternion.Euler(rotX, rotY, rotZ);
                rotation = rotation * offsetAngle;
            }
            
            if(enableRotation) {
                // Change the objects orientation
                transform.rotation = rotation;
            }
            */

            // Rotate the bones

            // INDEX
            rootBone.Find("wrist_l/finger_index_meta_l/finger_index_0_l").transform.localEulerAngles = new Vector3(0, 0, -finger1);
            rootBone.Find("wrist_l/finger_index_meta_l/finger_index_0_l/finger_index_1_l").transform.localEulerAngles = new Vector3(0, 0, -finger1);

            // MIDDLE
            rootBone.Find("wrist_l/finger_middle_meta_l/finger_middle_0_l").transform.localEulerAngles = new Vector3(0, 0, -finger3);
            rootBone.Find("wrist_l/finger_middle_meta_l/finger_middle_0_l/finger_middle_1_l").transform.localEulerAngles = new Vector3(0, 0, -finger2);

        }
    }
}