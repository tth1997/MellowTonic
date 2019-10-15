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

    // Acceleration PID
    CarAccelerationPID AccelPID = new CarAccelerationPID();
    public float pid_Kp1 = 1f;
    public float pid_Ki1 = 1f;
    public float pid_Kd1 = 1f;
    public float pid_maxMotorForce;
    public float pid_minMotorForce;

    public bool slowed;

    // Steering PID
    CarSteeringPID SteerPID = new CarSteeringPID();
    public float pid_Kp2 = 1f;
    public float pid_Ki2 = 1f;
    public float pid_Kd2 = 1f;
    float pid_maxSteerAngle = 30f;

    // Acceleration
    float verticalInput
    {
        get
        {
            return blackedOut == false // When blacked out
                ? Input.GetAxis("Vertical")
                : 0f;

        }
    }
    [HideInInspector]
    public float motorForce;
    float brakeForce = 50000f;

    public float carVel
    {
        get
        {
            return carRB.velocity.magnitude;
        }
    }
    float oldCarVel;
    [HideInInspector]
    public float targetVel;
    float maxTargetVel;
    [HideInInspector]
    public bool PIDActive;

    // Steering
    [HideInInspector]
    public GameObject mouseRigObject;

    float steerRotation;

    float steeringTarget;
    float pid_steeringDamp = 1f;
    float steeringAngle;

    bool freeLook;

    // Fatigue Effects for Steering
    [HideInInspector]
    public float swayTimer;                             // RESET THIS TO Time.time + CarMovementScript.swayTimerDefault AT REST STOP
    public float swayTimerDefault = 60; 

    [HideInInspector]
    public float currentAccelSwayMagnitude;             // RESET THIS TO 0 AT REST STOP
    float accelSwayMagnitudeRate = 4.16f;
    float accelSwayMagnitudeLimit = 500f;

    [HideInInspector]
    public float currentSteerSwayMagnitude;             // RESET THIS TO 0 AT REST STOP
    float steerSwayMagnitudeRate = 0.033f;
    float steerSwayMagnitudeLimit = 4f;

    public float steerRate = 80f; // How fast the wheels can steer (degrees per second).    RESET THIS TO CarMovementScript.maxSteerRate AT REST STOP
    float steerRateLoss = 0.33f;
    float minSteerRate = 40f;
    public float maxSteerRate = 80f;

    // Fatigue Effects for Camera
    private PostProcessVolume postProcVolume;

    private DepthOfField depthOfField;
    [HideInInspector]
    public float currentFocusDistance = 0.5f;           // RESET THIS TO 0.5f AT REST STOP
    float focusDistanceRate = 0.00075f;
    public float focusDistanceMax = 0.25f;
    float focusDistanceMin = 0.16f;

    float focalLengthDefault = 5f;
    float apertureDefault = 1f;

    private Vignette vignette;
    public float vignetteIntensity;                 // RESET THIS TO 0.5f AT REST STOP
    float vignetteRate = 0.0033f;
    float maxVignetteIntensity = 0.6f;

    [HideInInspector]
    public Image blackoutImg;
    Color fadeColor;
    [HideInInspector]
    public float blackoutTimer;                         // RESET THIS TO Time.time + 180 AT REST STOP
    public float blackoutTimerDefault = 180f;
    float blackoutRate = 20f; // The amount of time between blackouts.
    public float blackoutBaseDuration = 0.1f;           // RESET THIS TO CarMovementScript.blackoutBaseDurationDefault AT REST STOP
    public float blackoutDurationDefault = 0.1f;
    bool isBlackingOut = false;
    bool blackedOut = false;
    bool fadingIn;
    bool fadingOut;

    // Text & UI
    public Text speedTxt;

    // End-State & Game Management
    [HideInInspector]
    public GameManagerScript gameManagerScript;
    float crashAccel = 7.5f;
    [SerializeField]
    bool hasCrashed = false;

    public bool allowDebugging;


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
        carRB.centerOfMass = new Vector3(0, -0.5f, 0);

        mouseRigObject = GameObject.Find("MouseRig");
        if (mouseRigObject == null)
            Debug.LogError("mouseRigObject not specified. Make sure that the MouseRig prefab is in the scene, and is named as such.");

        postProcVolume = GetComponentInChildren<PostProcessVolume>();
        postProcVolume.profile.TryGetSettings(out depthOfField);
        depthOfField.aperture.value = apertureDefault;
        depthOfField.focalLength.value = focalLengthDefault;

        postProcVolume.profile.TryGetSettings(out vignette);
        vignette.intensity.value = 0;

        blackoutTimer = Time.time + blackoutTimerDefault;
        swayTimer = Time.time + swayTimerDefault;

        blackoutImg = GameObject.Find("BlackoutImg").GetComponent<Image>();
        blackoutImg.color = Color.clear;
        fadeColor = Color.clear;

        speedTxt = GameObject.Find("SpeedTxt").GetComponent<Text>();

        gameManagerScript = GameObject.Find("EventSystem").GetComponent<GameManagerScript>();

        PIDActive = false;
        freeLook = false;
    }

    private void InitializeAccelPID()
    {
        AccelPID.Kp = pid_Kp1;
        AccelPID.Ki = pid_Ki1;
        AccelPID.Kd = pid_Kd1;
        AccelPID.outputMax = pid_maxMotorForce;
        AccelPID.outputMin = pid_minMotorForce;
    }
    private void InitializeSteerPID()
    {
        SteerPID.Kp = pid_Kp2;
        SteerPID.Ki = pid_Ki2;
        SteerPID.Kd = pid_Kd2;
        SteerPID.outputMax = pid_maxSteerAngle;
        SteerPID.outputMin = -pid_maxSteerAngle;
        SteerPID.steeringDamp = pid_steeringDamp;
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
        if (slowed)
        {
            maxTargetVel = 11.11f;
        }
        else
        {
            maxTargetVel = 33.33f;
        }

        // Acceleration & Reverse

        if (verticalInput != 0 && carVel < maxTargetVel)
        {
            PIDActive = false;
            targetVel = Mathf.Clamp(carVel,
                                    0, 
                                    maxTargetVel);
        }
        else
        {
            PIDActive = true;
        }

        // Acceleration PID
        if (!PIDActive)
        {
            frontLeftW.motorTorque = verticalInput * pid_maxMotorForce;
            frontRightW.motorTorque = verticalInput * pid_maxMotorForce;
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
            targetVel = carVel;
        }
        else if (verticalInput == 0 && carVel < 0.25f)
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
        CamBlur();

        if (swayTimer > Time.time)
        {
            SteeringRate();
            SteeringSway();
            AcceleratorSway();
        }

        CamBlackout();

        Steer();
        UpdateSteeringWheel();

        CheckRoll();
        CollisionCheck();

        if (allowDebugging)
            Debugging();
    }

    void SteeringRate()
    {
        steerRate = Mathf.Clamp(steerRate - steerRateLoss * Time.fixedDeltaTime,
                                minSteerRate,
                                maxSteerRate);
    }
    void SteeringSway()
    {
        SteerPID.steeringSwayMagnitude = currentSteerSwayMagnitude;

        currentSteerSwayMagnitude = Mathf.Clamp(currentSteerSwayMagnitude + steerSwayMagnitudeRate * Time.fixedDeltaTime, 
                                                0, 
                                                steerSwayMagnitudeLimit);
    }
    void AcceleratorSway()
    {
        AccelPID.acceleratorSwayMagnitude = currentAccelSwayMagnitude;

        currentAccelSwayMagnitude = Mathf.Clamp(currentAccelSwayMagnitude + accelSwayMagnitudeRate * Time.fixedDeltaTime,
                                                0,
                                                accelSwayMagnitudeLimit);
    }
    void CamBlur()
    {
        // Blur
        currentFocusDistance = Mathf.Clamp(currentFocusDistance - (focusDistanceRate * Time.fixedDeltaTime), 
                                           focusDistanceMin,
                                           focusDistanceMax);

        depthOfField.focusDistance.value = currentFocusDistance;
        
        // Vignette
        vignetteIntensity = Mathf.Clamp(vignetteIntensity - (vignetteRate * Time.fixedDeltaTime),
                                        0,
                                        maxVignetteIntensity);

        vignette.intensity.value = vignetteIntensity;

        
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
                SteerPID.steeringDamp = Mathf.Clamp(SteerPID.steeringDamp - fadeRate, 0.5f, 1f);
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
                fadeColor.a = Mathf.Clamp(fadeColor.a - (fadeRate * 6), 0, 1);
                blackoutImg.color = fadeColor;
                SteerPID.steeringDamp = Mathf.Clamp(SteerPID.steeringDamp + fadeRate * 4, 0.5f, 1f);
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

            if (Time.time < blackoutTimer)
            {
                isBlackingOut = false;

                StopCoroutine(BlackoutFade());
            }
        }
    }

    void Steer()
    {
        // If player right-clicks, the steeringAngle will not recieve any new data, which allows
        // the player to move the camera around without steering the car.
        
        if (!freeLook)
        {
            steerRotation = mouseRigObject.GetComponent<MouseController>().mouseAimTransform.eulerAngles.y;

            steeringAngle = Mathf.Clamp(
                            SteerPID.Cycle(transform.rotation.eulerAngles.y,
                                           steerRotation,
                                           Time.fixedDeltaTime),
                            steeringAngle - (steerRate * Time.fixedDeltaTime),
                            steeringAngle + (steerRate * Time.fixedDeltaTime));




            if (Vector3.Angle(transform.forward, carRB.velocity) > 90)
            {
                frontLeftW.steerAngle = -steeringAngle;
                frontRightW.steerAngle = -steeringAngle;
            }
            else
            {
                frontLeftW.steerAngle = steeringAngle;
                frontRightW.steerAngle = steeringAngle;
            }
        }
    }

    void UpdateSteeringWheel()
    {
        if (!freeLook)
        {
            steeringWheel.eulerAngles = new Vector3(steeringWheel.eulerAngles.x, steeringWheel.eulerAngles.y, (transform.eulerAngles.y - steerRotation));
        }
    }

    void CheckRoll()
    {
        if (transform.eulerAngles.z < 180 && transform.eulerAngles.z > 60 && !hasCrashed)
        {
            gameManagerScript.Crash();
            hasCrashed = true;
        }
        else if (transform.eulerAngles.z > 180 & transform.eulerAngles.z < 300 && !hasCrashed)
        {
            gameManagerScript.Crash();
            hasCrashed = true;
        }
    }
    void CollisionCheck()
    {
        if (Mathf.Abs(oldCarVel - carVel) > crashAccel && !hasCrashed)
        {
            gameManagerScript.Crash();
            hasCrashed = true;
        }

        oldCarVel = carVel;
    }


    void Debugging()
    {
        if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out RaycastHit rayHit))
        {
            Debug.Log(rayHit.collider.name);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SlowSpeedTrigger")
        {
            slowed = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SlowSpeedTrigger")
        {
            slowed = false;
        }
    }
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

    float acceleratorSway;
    float acceleratorSwayPeriod = 10f;
    public float acceleratorSwayMagnitude;

    public float preError;
    public float integral;
    public float derivative;
    public float output;

    public float Cycle(float currentSpeed, float targetSpeed, float Dt)
    {
        var error = targetSpeed - currentSpeed;
        integral = Mathf.Clamp(integral + error + Dt, outputMin, outputMax);
        derivative = (error - preError) / Dt;

        acceleratorSway += (Mathf.PI / acceleratorSwayPeriod) * Time.fixedDeltaTime;
        if (acceleratorSway >= 2 * Mathf.PI)
            acceleratorSway = 0f;

        output = Mathf.Clamp(error * Kp + integral * Ki + derivative * Kd, outputMin, outputMax);

        preError = error;
        return output + (Mathf.Sin(acceleratorSway) * acceleratorSwayMagnitude);
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
