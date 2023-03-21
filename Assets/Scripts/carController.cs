using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class carController : MonoBehaviour
{


    public WheelCollider front_left;
    public WheelCollider front_right;
    public WheelCollider back_left;
    public WheelCollider back_right;
    public Transform FL;
    public Transform FR;
    public Transform BL;
    public Transform BR;

    public float maxTurnAngle = 15f;
    public float currentTurnAngle = 0f;
    public float Torque = 0;

    // Update is called once per frame
    void Update()
    {
 
        //Rotation des roues
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        front_left.steerAngle = currentTurnAngle;
        front_right.steerAngle = currentTurnAngle;
        front_left.motorTorque = 0;
        front_right.motorTorque = 0;
        back_left.motorTorque = 0;
        back_right.motorTorque = 0;

        //update wheel meshes
        UpdateWheel(front_left, FL);
        UpdateWheel(front_right, FR);
        UpdateWheel(back_left, BL);
        UpdateWheel(back_right, BR);
    }

    void UpdateWheel(WheelCollider col, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        transform.position = position;
        transform.rotation = rotation;
    }
}
