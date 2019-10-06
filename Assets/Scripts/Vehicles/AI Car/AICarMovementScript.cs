using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AICarMovementScript : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent carAgent;

    [HideInInspector]
    public Transform waypointCurrent;

    int waypointNum = 1; // Waypoint 1 corresponds to A lanes, 2 to B lanes

    // FUTURE REFERENCE:
    // Spawn an AI car every 30 - 60 seconds
    // The AI's "Lane Number" is randomised between 1 and 4. 1-2 = A lanes, 3-4 = B lanes.
    // Lane Number will determine initial orientation of the vehicle as well as which lane it will use to navigate
    // AI car speed is also randomised between 90 and 110kph.
    // If the AI car speed (upon instantiation) is LOWER than the player's speed, it will spawn AHEAD of the player by ~300m.
    // Likewise, if the AI car speed is HIGHER than the player's, it will spawn BEHIND the player.
    // If an AI car is more than 500m away from the player, despawn it.

    void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();

        waypointCurrent = GameObject.FindGameObjectWithTag("Waypoint" + waypointNum.ToString()).transform;        
    }

    void Update()
    {
        if (waypointCurrent != null)
        {
            carAgent.SetDestination(waypointCurrent.position);
        }
        else if (waypointCurrent == null)
        {
            Debug.Log("Waypoint A not found!");
        }
    }
}
