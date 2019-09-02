using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CarMovementScript : MonoBehaviour
{
    // Wheel Colliders
    private Rigidbody carRB;
    public WheelCollider frontLeftW, frontRightW;
    public WheelCollider rearLeftW, rearRightW;
    public Transform frontLeftT, frontRightT;
    public Transform rearLeftT, rearRightT;

    // Acceleration
    CarAccelerationPID AccelPID = new CarAccelerationPID();
    public float pid_Kp1 = 1f;
    public float pid_Ki1 = 1f;
    public float pid_Kd1 = 1f;

    float verticalInput
    {
        get
        {
            return Input.GetAxis("Vertical");
        }
    }
    public float maxMotorForce;
    public float minMotorForce;

    public float motorForce;
    float brakeForce = 50000f;
    public float carVel
    {
        get
        {
            return carRB.velocity.magnitude;
        }
    }
    public float targetVel;

    public bool PIDActive;

    // Steering
    CarSteeringPID SteerPID = new CarSteeringPID();
    public GameObject mouseRigObject;

    float steerRotation;

    public float pid_Kp2 = 1f;
    public float pid_Ki2 = 1f;
    public float pid_Kd2 = 1f;

    float steeringTarget;
    float steeringAngle;
    float maxSteerAngle = 30f;

    public float inputDelay;
    List<float> mouseAimYRotations = new List<float>();

    // Text & UI
    public Text speedText;
    public Text verticalAxisText;

        // START BLOCK //
    void Start()
    {
        CheckVariables();

        carRB = GetComponent<Rigidbody>();
        InitializeAccelPID();
        InitializeSteerPID();
        PIDActive = false;
    }

    void CheckVariables()
    {
        if (maxMotorForce <= 0 || minMotorForce <= 0)
            Debug.LogError("Min and max motor forces must be greater than zero.");

        if (mouseRigObject == null)
            Debug.LogError("mouseRigObject not specified. Make sure that the MouseRig prefab is in the scene.");
    }

    private void InitializeAccelPID()
    {
        AccelPID.Kp = pid_Kp1;
        AccelPID.Ki = pid_Ki1;
        AccelPID.Kd = pid_Kd1;
        AccelPID.outputMax = maxMotorForce;
        AccelPID.outputMin = minMotorForce;
    }
    private void InitializeSteerPID()
    {
        SteerPID.Kp = pid_Kp2;
        SteerPID.Ki = pid_Ki2;
        SteerPID.Kd = pid_Kd2;
        SteerPID.outputMax = maxSteerAngle;
        SteerPID.outputMin = -maxSteerAngle;
    }
        // END START BLOCK //

        // UPDATE BLOCK //
    void Update()
    {
        if (speedText != null)
            speedText.text = Mathf.Round(carVel * 3.6f).ToString() + "km/h";

        if (verticalAxisText != null)
            verticalAxisText.text = "VertAxis: " + Input.GetAxis("Vertical").ToString();

        Accelerate();
    }

    void Accelerate()
    {
        // Acceleration & Reverse
        if (Input.GetAxis("Vertical") == 0)
        {
            PIDActive = true;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            PIDActive = false;
            targetVel = carVel;
        }

        // Acceleration PID
        if (!PIDActive)
        {
            frontLeftW.motorTorque = verticalInput * maxMotorForce;
            frontRightW.motorTorque = verticalInput * maxMotorForce;
        }
        if (PIDActive)
        {
            motorForce = AccelPID.Cycle(carVel, targetVel, Time.fixedDeltaTime);
            frontLeftW.motorTorque = motorForce;
            frontRightW.motorTorque = motorForce;
        }

        // Braking
        if (Input.GetKey(KeyCode.Space))
        {
            frontLeftW.brakeTorque = brakeForce;
            frontRightW.brakeTorque = brakeForce;
        }
        else
        {
            frontLeftW.brakeTorque = 0f;
            frontRightW.brakeTorque = 0f;
        }
    }
        // END UPDATE BLOCK //

        // FIXEDUPDATE BLOCK //
    // Note that the FixedUpdate loop runs at 50 frames per second by default.
    private void FixedUpdate()
    {
        InputDelay();
        Steer();
        //UpdateWheelPoses();
    }

    void InputDelay()
    {

        mouseAimYRotations.Insert(0, mouseRigObject.GetComponent<MouseController>().mouseAimTransform.rotation.eulerAngles.y);

        // Delete any rotations that have persisted for more than 3 seconds.
        if (mouseAimYRotations.Count > Mathf.RoundToInt(3f / Time.fixedDeltaTime))
        {
            mouseAimYRotations.RemoveAt(mouseAimYRotations.Count - 1);
        }

        steerRotation = mouseAimYRotations[Mathf.RoundToInt(inputDelay / Time.fixedDeltaTime)];
    }

    void Steer()
    {
        // If player right-clicks, the steeringAngle will not recieve any new data, which allows
        // the player to move the camera around without steering the car.
        if (!Input.GetKey(KeyCode.Mouse1))
        {
            steeringAngle = SteerPID.Cycle(transform.rotation.eulerAngles.y, 
                                           steerRotation, 
                                           Time.fixedDeltaTime);
        }
        else
        {
            steeringAngle = 0;
        }
        frontLeftW.steerAngle = steeringAngle;
        frontRightW.steerAngle = steeringAngle;
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

        //quat.eulerAngles = new Vector3(quat.eulerAngles.x, Wtransform.rotation.eulerAngles.y, Wtransform.rotation.eulerAngles.z);

        Wtransform.position = pos;
        Wtransform.localRotation = quat;
    }
        // END FIXEDUPDATE BLOCK //
}

/// <summary>
/// PID controller used for controlling car acceleration.
/// </summary>
public class CarAccelerationPID
{
    public float Kp;
    public float Ki;
    public float Kd;

    public float outputMax;
    public float outputMin;

    public float preError;
    public float integral;
    public float derivative;
    public float output;

    public float Cycle(float currentSpeed, float targetSpeed, float Dt)
    {
        var error = targetSpeed - currentSpeed;
        integral += error + Dt;
        derivative = (error - preError) / Dt;
        output = Mathf.Clamp(error * Kp + integral * Ki + derivative * Kd, outputMin, outputMax);

        preError = error;
        return output;
    }
}

/// <summary>
/// PID controller used for controlling steering. 'Cycle' output should be used to determine steering angle.
/// </summary>
public class CarSteeringPID
{
    public float Kp;
    public float Ki;
    public float Kd;

    public float outputMax;
    public float outputMin;

    public float preError;
    public float integral;
    public float derivative;
    public float output;

    public float Cycle(float currentDirection, float targetDirection, float Dt)
    {
        var error = Mathf.DeltaAngle(currentDirection, targetDirection);
        integral += error + Dt;
        derivative = (error - preError) / Dt;
        output = Mathf.Clamp(error * Kp + integral * Ki + derivative * Kd, outputMin, outputMax);

        preError = error;
        return output;
    }
}
