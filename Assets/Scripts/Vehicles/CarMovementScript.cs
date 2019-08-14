using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementScript : MonoBehaviour
{
    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void Steer()
    {
        steeringAngle = maxSteerAngle * horizontalInput;
        frontLeftW.steerAngle = steeringAngle;
        frontRightW.steerAngle = steeringAngle;
    }

    void Accelerate()
    {
        frontLeftW.motorTorque = verticalInput * motorForce;
        frontRightW.motorTorque = verticalInput * motorForce;
    }

    void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftW, frontLeftT);
        UpdateWheelPose(frontRightW, frontRightT);
        UpdateWheelPose(rearLeftW, rearLeftT);
        UpdateWheelPose(rearRightW, rearRightT);
    }

    void UpdateWheelPose(WheelCollider collider, Transform Wtransform)
    {
        Vector3 pos = Wtransform.position;
        Quaternion quat = Wtransform.rotation;

        collider.GetWorldPose(out pos, out quat);

        Wtransform.position = pos;
        Wtransform.rotation = quat;
    }

    float horizontalInput;
    float verticalInput;
    float steeringAngle;

    public WheelCollider frontLeftW, frontRightW;
    public WheelCollider rearLeftW, rearRightW;
    public Transform frontLeftT, frontRightT;
    public Transform rearLeftT, rearRightT;
    public float maxSteerAngle = 30f;
    public float motorForce = 50f;

    void Start()
    {

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }
}
