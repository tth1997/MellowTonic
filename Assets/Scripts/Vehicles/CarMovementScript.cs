using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CarMovementScript : MonoBehaviour
{

    float horizontalInput;
    float verticalInput;
    float steeringAngle;
    float maxSteerAngle = 30f;

    private Rigidbody carRB;
    public WheelCollider frontLeftW, frontRightW;
    public WheelCollider rearLeftW, rearRightW;
    public Transform frontLeftT, frontRightT;
    public Transform rearLeftT, rearRightT;

    CarAccelerationPID AccelPID = new CarAccelerationPID();
    public float pid_Kp1 = 1f;
    public float pid_Ki1 = 1f;
    public float pid_Kd1 = 1f;
    public float maxMotorForce = 1000f;
    public float minMotorForce = -100f;
    public bool PIDActive;

    CarSteeringPID SteerPID = new CarSteeringPID();
    public Transform mouseAimTransform;
    public float pid_Kp2 = 1f;
    public float pid_Ki2 = 1f;
    public float pid_Kd2 = 1f;

    public float inputDelay;
    public float horizInputSens = 0.2f;
    public float horizInputGrav = 1f;
    float inputSensModerator;
    float steeringTarget;
    
    public float motorForce;
    float brakeForce = 50000f;
    public float carVel;
    public float targetVel;

    public Text speedText;
    public Text verticalAxisText;

    void Start()
    {
        carRB = GetComponent<Rigidbody>();
        InitializeAccelPID();
        InitializeSteerPID();
        PIDActive = false;
        StartCoroutine(InputDelay());
    }

    void Update()
    {
        GetInput();

        if (speedText != null)
            speedText.text = Mathf.Round(carVel * 3.6f).ToString() + "km/h";

        if (verticalAxisText != null)
            verticalAxisText.text = "VertAxis: " + Input.GetAxis("Vertical").ToString();

        Accelerate();
    }

    private void FixedUpdate()
    {
        Steer();
        //UpdateWheelPoses();
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

    IEnumerator InputDelay()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.D))
            {
                yield return new WaitForSeconds(inputDelay);
                steeringTarget = 1f;
                Debug.Log("Input delayed by " + inputDelay);

            }
            else if (Input.GetKey(KeyCode.A))
            {
                yield return new WaitForSeconds(inputDelay);
                steeringTarget = -1f;
                Debug.Log("Input delayed by " + inputDelay);
            }
            else
            {
                steeringTarget = 0f;
            }
            yield return null;
        }
    }
    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");


        inputSensModerator = Mathf.Clamp(5 / carVel, 0, 1);

        if (steeringTarget > 0)
        {
            if (horizontalInput >= 0)
            {
                horizontalInput = steeringOut(steeringTarget);
            }
            else
            {
                horizontalInput = steeringIn(steeringTarget);
            }
        }
        else if (steeringTarget < 0)
        {
            if (horizontalInput <= 0)
            {
                horizontalInput = steeringOut(steeringTarget);
            }
            else
            {
                horizontalInput = steeringIn(steeringTarget);
            }
        }
        else
        {
            if (horizontalInput < 0)
            {
                horizontalInput = steeringIn(1);
            }
            else if (horizontalInput > 0)
            {
                horizontalInput = steeringIn(-1);
            }
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

    float steeringOut(float target)
    {
        return Mathf.Clamp(horizontalInput + (target * horizInputSens) * inputSensModerator * Time.deltaTime, -1f, 1f);
    }
    float steeringIn(float target)
    {
        return Mathf.Clamp(horizontalInput + (target * horizInputGrav) * inputSensModerator * Time.deltaTime, -1f, 1f);
    }

    void Steer()
    {
        // Old A/D steering

        //steeringAngle = maxSteerAngle * horizontalInput;
        steeringAngle = SteerPID.Cycle(transform.rotation.eulerAngles.y, mouseAimTransform.rotation.eulerAngles.y, Time.fixedDeltaTime);
        frontLeftW.steerAngle = steeringAngle;
        frontRightW.steerAngle = steeringAngle;
    }

    void Accelerate()
    {
        carVel = carRB.velocity.magnitude;

        if (Input.GetAxis("Vertical") == 0)
        {
            PIDActive = true;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            PIDActive = false;
            targetVel = carVel;
        }

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
}

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
        Debug.Log(error * Kp);
        return output;
    }
}
