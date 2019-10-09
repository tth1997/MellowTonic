using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AICarMovementScript : MonoBehaviour
{
    int AILimit = 10;


    // Wheel Colliders
    private Rigidbody AIcarRB;
    public WheelCollider frontLeftW, frontRightW;
    public WheelCollider rearLeftW, rearRightW;

    // Acceleration PID
    AIAccelerationPID AIAccelPID = new AIAccelerationPID();
    float pid_Kp1 = 1000f;
    float pid_Ki1 = 0.15f;
    float pid_Kd1 = 10f;
    float pid_maxMotorForce = 1000;
    float pid_minMotorForce = -10;

    // Steering PID
    AISteeringPID AISteerPID = new AISteeringPID();
    float pid_Kp2 = 1f;
    float pid_Ki2 = 0f;
    float pid_Kd2 = 0.2f;
    float pid_maxSteerAngle = 30f;

    // Acceleration
    public float motorForce;
    public float currentBrakeForce;
    float brakeForce = 50000f;
    float brakeDistance = 10f;
    public float AIVel
    {
        get
        {
            return AIcarRB.velocity.magnitude;
        }
    }

    // Steering
    [HideInInspector]
    public GameObject mouseRigObject;

    float steeringTarget;
    float pid_steeringDamp = 1f;
    float steeringAngle;

    // NavMesh
    public GameObject carAgentObject;
    NavMeshAgent carAgent;
    Transform carAgentTransform;

    [HideInInspector]
    public Transform waypointCurrent;
    [HideInInspector]
    Transform playerCarTransform;

    [HideInInspector]
    public int waypointNum;
    [HideInInspector]
    public int laneNum;
    [HideInInspector]
    public float moveSpeed;
    float agentMaxDistance = 10f;

    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;
        AIcarRB = GetComponent<Rigidbody>();

        Transform closestLaneTransform = FindClosestLane(GameObject.FindGameObjectsWithTag("Lane"));
        transform.position = new Vector3(transform.position.x, transform.position.y, closestLaneTransform.position.z); 

        carAgentTransform = Instantiate(carAgentObject, transform.position, Quaternion.identity).transform;
        carAgent = carAgentTransform.GetComponent<NavMeshAgent>();

        waypointCurrent = GameObject.FindGameObjectWithTag("Waypoint" + waypointNum.ToString()).transform;
        if (waypointNum == 1)
        {
            laneNum = Random.Range(1, 3);
            transform.eulerAngles = Vector3.down * 90;
        }
        else
        {
            laneNum = Random.Range(3, 5);
            transform.eulerAngles = Vector3.up * 90;
        }

        if (laneNum == 1)
            carAgent.areaMask = 1 << NavMesh.GetAreaFromName("Lane A1");

        if (laneNum == 2)
            carAgent.areaMask = 1 << NavMesh.GetAreaFromName("Lane A2");

        if (laneNum == 3)
            carAgent.areaMask = 1 << NavMesh.GetAreaFromName("Lane B1");

        if (laneNum == 4)
            carAgent.areaMask = 1 << NavMesh.GetAreaFromName("Lane B2");


        carAgent.speed = 50f;
        carAgent.acceleration = 50f;
        carAgent.enabled = true;
        
        NavMeshHit navHit;
        if (carAgent.FindClosestEdge(out navHit))
        {
            transform.position = navHit.position;
        }
        
        InitializeAccelPID();
        InitializeSteerPID();
    }

    private Transform FindClosestLane(GameObject[] lanes)
    {
        Transform closestLane = null;
        float closestDistSqr = Mathf.Infinity;

        for (int i = 0; i < lanes.Length; i += 12)
        {
            float distSqr = (lanes[i].transform.position - transform.position).sqrMagnitude;

            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closestLane = lanes[i].transform;
            }
        }

        return closestLane;
    }

    private void InitializeAccelPID()
    {
        AIAccelPID.Kp = pid_Kp1;
        AIAccelPID.Ki = pid_Ki1;
        AIAccelPID.Kd = pid_Kd1;
        AIAccelPID.outputMax = pid_maxMotorForce;
        AIAccelPID.outputMin = pid_minMotorForce;
    }
    private void InitializeSteerPID()
    {
        AISteerPID.Kp = pid_Kp2;
        AISteerPID.Ki = pid_Ki2;
        AISteerPID.Kd = pid_Kd2;
        AISteerPID.outputMax = pid_maxSteerAngle;
        AISteerPID.outputMin = -pid_maxSteerAngle;
        AISteerPID.steeringDamp = pid_steeringDamp;
    }

    void Update()
    {
        DestroyCheck();

        Acceleration();
        NavAgentUpdate();
    }

    void DestroyCheck()
    {
        if (transform.position.sqrMagnitude - playerCarTransform.position.sqrMagnitude > Mathf.Pow(2, 400))
        {
            Destroy(carAgentTransform.gameObject);
            Destroy(gameObject);
        }

        if (transform.position.sqrMagnitude - waypointCurrent.position.sqrMagnitude < Mathf.Pow(10, 2))
        {
            Destroy(carAgentTransform.gameObject);
            Destroy(gameObject);
        }
    }

    void Acceleration()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, brakeDistance))
        {
            motorForce = -AIAccelPID.Cycle(AIVel, moveSpeed, Time.fixedDeltaTime);
            frontLeftW.motorTorque = motorForce;
            frontRightW.motorTorque = motorForce;

            if (AIVel > 14)
            {
                frontLeftW.brakeTorque = brakeForce;
                frontRightW.brakeTorque = brakeForce;
                currentBrakeForce = brakeForce;
            }
            else
            {
                frontLeftW.brakeTorque = 0;
                frontRightW.brakeTorque = 0;
                currentBrakeForce = 0;
            }
        }
        else
        {
            motorForce = AIAccelPID.Cycle(AIVel, moveSpeed, Time.fixedDeltaTime);
            frontLeftW.motorTorque = motorForce;
            frontRightW.motorTorque = motorForce;

            frontLeftW.brakeTorque = 0;
            frontRightW.brakeTorque = 0;
            currentBrakeForce = 0;
        }
    }

    public void NavAgentUpdate()
    {
        if (waypointCurrent != null && carAgent.isOnNavMesh)
        {
            carAgent.SetDestination(waypointCurrent.position);
        }
        else
        {
            if (waypointCurrent == null)
                Debug.LogError("Waypoint not found!");
            if (!carAgent.isOnNavMesh)
                Debug.LogError("Agent is not close enough to NavMesh!");
        }

        carAgentTransform.position = new Vector3(Mathf.Clamp(carAgentTransform.position.x, transform.position.x - agentMaxDistance, transform.position.x + agentMaxDistance),
                                                 Mathf.Clamp(carAgentTransform.position.y, transform.position.y - agentMaxDistance, transform.position.y + agentMaxDistance),
                                                 Mathf.Clamp(carAgentTransform.position.z, transform.position.z - agentMaxDistance, transform.position.z + agentMaxDistance));
    }

    private void FixedUpdate()
    {
        Steer();
    }

    void Steer()
    {
        Vector3 relPos = carAgentTransform.position - transform.position;
        Quaternion targetAngle = Quaternion.LookRotation(relPos, Vector3.up);

        steeringAngle = AISteerPID.Cycle(transform.rotation.eulerAngles.y,
                                         targetAngle.eulerAngles.y,
                                         Time.fixedDeltaTime);


        if (Vector3.Angle(transform.forward, AIcarRB.velocity) > 90)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * brakeDistance);

        Gizmos.color = Color.red;
        if (carAgentTransform != null)
            Gizmos.DrawWireSphere(carAgentTransform.position, 2f);
    }
}

public class AIAccelerationPID
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

public class AISteeringPID
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
