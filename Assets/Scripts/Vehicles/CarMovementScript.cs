using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CarMovementScript : MonoBehaviour
{
    // Steering Wheel
    public Transform steeringWheel;
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
            return blackedOut == false // When blacked out
                ? Input.GetAxis("Vertical")
                : 0f;

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
    //float steeringDamp = 1f;
    public float steeringAngle;
    float maxSteerRate = 120f; // How fast the wheels can steer (degrees per second)
    float maxSteerAngle = 30f;

    bool freeLook;

    // Fatigue Effects for Steering
    [HideInInspector]
    public float currentSwayMagnitude;                                                          // RESET THIS TO 0 AT REST STOP
    float swayMagnitudeRate = 0.013f;
    float swayMagnitudeLimit = 4f;

    [HideInInspector]
    public float inputDelayTime;                                                                // RESET THIS TO 0 AT REST STOP
    int inputDelayFrames;
    float inputDelayLimit = 0.5f;
    float inputDelayRate = 0.0016f;
    List<float> mouseAimYRotations = new List<float>();

    // Fatigue Effects for Camera
    private PostProcessVolume postProcVolume;

    private DepthOfField depthOfField;
    [HideInInspector]
    public float currentFocusDistance = 0.5f;                                                   // RESET THIS TO 0.5f AT REST STOP
    float focusDistanceRate = 0.0013f;
    float focusDistanceMax = 0.5f;
    float focusDistanceMin = 0.14f;

    float focalLengthDefault = 5f;
    float apertureDefault = 0.5f;

    public Image blackoutImg;
    Color fadeColor;
    //[HideInInspector]
    public float blackoutTimer;                                                                 // RESET THIS TO Time.time + 180 AT REST STOP
    float blackoutRate = 20f; // The amount of time between blackouts.
    float blackoutBaseDuration = 0.1f;                                                          // RESET THIS TO 0.5f AT REST STOP
    bool isBlackingOut = false;
    bool blackedOut = false;
    bool fadingIn;
    bool fadingOut;

    // Text & UI
    public Text speedTxt;


        // START BLOCK //
    void Start()
    {
        InitializeVariables();

        InitializeAccelPID();
        InitializeSteerPID();
    }

    void InitializeVariables()
    {
        carRB = GetComponent<Rigidbody>();
        mouseRigObject = GameObject.Find("MouseRig");
        if (mouseRigObject == null)
            Debug.LogError("mouseRigObject not specified. Make sure that the MouseRig prefab is in the scene.");

        postProcVolume = GetComponentInChildren<PostProcessVolume>();
        postProcVolume.profile.TryGetSettings(out depthOfField);
        depthOfField.aperture.value = apertureDefault;
        depthOfField.focalLength.value = focalLengthDefault;

        blackoutImg = GameObject.Find("BlackoutImg").GetComponent<Image>();
        blackoutImg.color = Color.clear;
        fadeColor = Color.clear;

        speedTxt = GameObject.Find("SpeedTxt").GetComponent<Text>();

        PIDActive = false;
        blackoutTimer += Time.time;
        freeLook = false;
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
        SteerPID.steeringDamp = 1;
    }
        // END START BLOCK //

        // UPDATE BLOCK //
    void Update()
    {
        if (speedTxt != null)
            speedTxt.text = Mathf.Round(carVel * 3.6f).ToString() + "km/h";

        Accelerate();
        InputCheck();
    }

    void Accelerate()
    {
        // Acceleration & Reverse
        if (verticalInput == 0)
        {
            PIDActive = true;
        }
        if (verticalInput != 0)
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
        if (!blackedOut && Input.GetKey(KeyCode.Space))
        {
            frontLeftW.brakeTorque = brakeForce;
            frontRightW.brakeTorque = brakeForce;
            rearLeftW.motorTorque = minMotorForce;
            rearRightW.motorTorque = minMotorForce;
            targetVel = carVel;
        }
        else
        {
            frontLeftW.brakeTorque = 0f;
            frontRightW.brakeTorque = 0f;
        }
    }

    void InputCheck()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            freeLook = true;
        }
        else
        {
            freeLook = false;
        }
    }
        // END UPDATE BLOCK //

        // FIXEDUPDATE BLOCK //
    // Note that the FixedUpdate loop runs at 50 frames per second by default.
    private void FixedUpdate()
    {
        // Fatigue Effects
        InputDelay();
        SteeringSway();
        CamBlur();
        CamBlackout();

        Steer();
        UpdateSteeringWheel();
    }

    void InputDelay()
    {
        mouseAimYRotations.Insert(0, mouseRigObject.GetComponent<MouseController>().mouseAimTransform.eulerAngles.y);

        // Delete any rotations that have persisted for more than 3 seconds.
        if (mouseAimYRotations.Count > Mathf.RoundToInt(3f / Time.fixedDeltaTime))
        {
            mouseAimYRotations.RemoveAt(mouseAimYRotations.Count - 1);
        }

        steerRotation = mouseAimYRotations[Mathf.RoundToInt(inputDelayTime / Time.fixedDeltaTime)];

        inputDelayFrames = Mathf.RoundToInt(inputDelayTime / Time.fixedDeltaTime);

        inputDelayTime += Mathf.Clamp(inputDelayRate * Time.fixedDeltaTime, 
                                      0, 
                                      inputDelayLimit);
    }
    void SteeringSway()
    {
        SteerPID.steeringSwayMagnitude = currentSwayMagnitude;

        currentSwayMagnitude = Mathf.Clamp(currentSwayMagnitude + swayMagnitudeRate * Time.fixedDeltaTime, 
                                           0, 
                                           swayMagnitudeLimit);
    }
    void CamBlur()
    {
        currentFocusDistance = Mathf.Clamp(currentFocusDistance - (focusDistanceRate * Time.fixedDeltaTime), 
                                           focusDistanceMin,
                                           focusDistanceMax);

        depthOfField.focusDistance.value = currentFocusDistance;
    }
    void CamBlackout()
    {
        if (Time.time > blackoutTimer && !isBlackingOut)
        {
            isBlackingOut = true;
            fadingIn = true;
            fadingOut = false;
            StartCoroutine(BlackoutFade());
            Debug.Log("Blackout Coroutine Started.");
        }
        else
        {
            StopCoroutine(BlackoutFade());
        }
    }
    IEnumerator BlackoutFade()
    {
        while (true)
        {
            float fadeRate = Time.deltaTime / 2;

            if (fadeColor.a < 1 && fadingIn)
            {
                fadeColor.a = Mathf.Clamp(fadeColor.a + fadeRate, 0, 1);
                blackoutImg.color = fadeColor;
                SteerPID.steeringDamp = Mathf.Clamp(SteerPID.steeringDamp - fadeRate, 0, 1);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            if (fadeColor.a == 1 && fadingIn)
            {
                float blackoutDuration = Random.Range(blackoutBaseDuration, blackoutBaseDuration * 2);
                blackedOut = true;
                yield return new WaitForSeconds(blackoutDuration);
                blackedOut = false;
                fadingIn = false;
                fadingOut = true;
            }

            if (fadeColor.a > 0 && fadingOut)
            {
                fadeColor.a = Mathf.Clamp(fadeColor.a - (fadeRate * 4), 0, 1);
                blackoutImg.color = fadeColor;
                SteerPID.steeringDamp = Mathf.Clamp(SteerPID.steeringDamp + fadeRate * 4, 0, 1);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            if (fadeColor.a == 0f && fadingOut)
            {
                yield return new WaitForSeconds(blackoutRate); // This will delay the next blackout for blackoutRate seconds
                blackoutBaseDuration += 0.1f;
                fadingIn = true;
                fadingOut = false;
                Debug.Log("Blackout Coroutine Cycle complete.");
            }
        }
    }

    void Steer()
    {
        // If player right-clicks, the steeringAngle will not recieve any new data, which allows
        // the player to move the camera around without steering the car.
        
        if (!freeLook)
        {
            steeringAngle = Mathf.Clamp(
                            SteerPID.Cycle(transform.rotation.eulerAngles.y,
                                           steerRotation,
                                           Time.fixedDeltaTime),
                            steeringAngle - (maxSteerRate * Time.fixedDeltaTime),
                            steeringAngle + (maxSteerRate * Time.fixedDeltaTime));

            frontLeftW.steerAngle = steeringAngle;
            frontRightW.steerAngle = steeringAngle;
        }
    }

    void UpdateSteeringWheel()
    {
        if (!freeLook)
        {
            steeringWheel.eulerAngles = new Vector3(steeringWheel.eulerAngles.x, steeringWheel.eulerAngles.y, (transform.eulerAngles.y - steerRotation));
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
        integral = Mathf.Clamp(integral + error + Dt, outputMin, outputMax);
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
    public float steeringDamp;

    float steeringSway;
    float steeringSwayPeriod = 5f;
    public float steeringSwayMagnitude;

    public float preError;
    public float integral;
    public float derivative;
    public float output;

    public float Cycle(float currentDirection, float targetDirection, float Dt)
    {
        var error = Mathf.DeltaAngle(currentDirection, targetDirection);

        integral = Mathf.Clamp(integral + error + Dt, outputMin, outputMax);

        derivative = (error - preError) / Dt;
        
        steeringSway += (Mathf.PI / steeringSwayPeriod) * Time.fixedDeltaTime;
        if (steeringSway >= 2 * Mathf.PI)
            steeringSway = 0f;
      
        output = Mathf.Clamp((error * Kp + integral * Ki + derivative * Kd) * steeringDamp, outputMin, outputMax);

        preError = error;
        return output + (Mathf.Sin(steeringSway) * steeringSwayMagnitude);
    }
}
