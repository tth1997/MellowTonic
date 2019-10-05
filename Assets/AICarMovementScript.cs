using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AICarMovementScript : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent carAgent;

    [HideInInspector]
    public Transform waypointTransform;

    void Awake()
    {
        waypointTransform = GameObject.FindGameObjectWithTag("Waypoint A").transform;
        carAgent = GetComponent<NavMeshAgent>();

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit))
        {
            carAgent.Warp(rayHit.point);
        }
    }

    void Update()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(waypointTransform.position, Vector3.down, out rayHit))
        {
            carAgent.destination = rayHit.point;
        }
    }
    
}
