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

    // Steering PID
    CarSteeringPID SteerPID = new CarSteeringPID();
    public float pid_Kp2 = 1f;
    public float pid_Ki2 = 1f;
    public float pid_Kd2 = 1f;
    float maxSteerRate = 120f; // How fast the wheels can steer (degrees per second)
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
    public float currentAccelSwayMagnitude;
    float accelSwayMagnitudeRate = 2.7f;
    float accelSwayMagnitudeLimit = 500f;

    [HideInInspector]
    public float currentSteerSwayMagnitude;                                                          // RESET THIS TO 0 AT REST STOP
    float steerSwayMagnitudeRate = 0.013f;
    float steerSwayMagnitudeLimit = 4f;

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
    public float blackoutBaseDuration = 0.1f;                                                          // RESET THIS TO 0.5f AT REST STOP
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
        blackoutTimer += Time.time;

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
        InputDelay();
        SteeringSway();
        AcceleratorSway();
        CamBlur();
        CamBlackout();

        Steer();
        UpdateSteeringWheel();

        CheckRoll();
        CollisionCheck();
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
