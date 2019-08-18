using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CarMovementScript : MonoBehaviour
{

    float horizontalInput;
    float verticalInput;
    float steeringAngle;

    public WheelCollider frontLeftW, frontRightW;
    public WheelCollider rearLeftW, rearRightW;
    public Transform frontLeftT, frontRightT;
    public Transform rearLeftT, rearRightT;
    Rigidbody carRB;

    float maxSteerAngle = 30f;
    public float inputDelay;
    public float horizInputSens = 0.2f;
    public float horizInputGrav = 1f;
    float inputSensModerator;
    float steeringTarget;

    bool gettingInput;
    float motorForce = 1000f;
    float brakeForce = 50000f;
    [SerializeField]
    float carVel;

    public Text speedText;
    public Text verticalAxisText;

    void Start()
    {
        carRB = GetComponent<Rigidbody>();

        StartCoroutine(InputDelay());
    }

    void Update()
    {
        GetInput();

        carVel = carRB.velocity.magnitude;

        if (speedText != null)
            speedText.text = Mathf.Round(carVel * 3.6f).ToString() + "km/h";

        if (verticalAxisText != null)
            verticalAxisText.text = "VertAxis: " + Input.GetAxis("Vertical").ToString();
    }

    private void FixedUpdate()
    {
        Steer();
        Accelerate();
        //UpdateWheelPoses();
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
                horizontalInput = Mathf.Clamp(horizontalInput + horizInputSens * inputSensModerator * Time.deltaTime,
                                              -1f,
                                               1f);
            }
            else
            {
                horizontalInput = Mathf.Clamp(horizontalInput + horizInputGrav * inputSensModerator * Time.deltaTime,
                            -1f,
                            1f);
            }
        }
        else if (steeringTarget < 0)
        {
            if (horizontalInput <= 0)
            {
                horizontalInput = Mathf.Clamp(horizontalInput - horizInputSens * inputSensModerator * Time.deltaTime,
                            -1f,
                            1f);
            }
            else
            {
                horizontalInput = Mathf.Clamp(horizontalInput - horizInputGrav * inputSensModerator * Time.deltaTime,
                            -1f,
                            1f);
            }
        }
        else
        {
            if (horizontalInput < 0)
            {
                horizontalInput = Mathf.Clamp(horizontalInput + horizInputGrav * inputSensModerator * Time.deltaTime,
                            -1f,
                            0f);
            }
            else if (horizontalInput > 0)
            {
                horizontalInput = Mathf.Clamp(horizontalInput - horizInputGrav * inputSensModerator * Time.deltaTime,
                            0f,
                            1f);
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

        //quat.eulerAngles = new Vector3(quat.eulerAngles.x, Wtransform.rotation.eulerAngles.y, Wtransform.rotation.eulerAngles.z);

        Wtransform.position = pos;
        Wtransform.localRotation = quat;
    }
}
