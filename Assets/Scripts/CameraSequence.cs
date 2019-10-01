using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSequence : MonoBehaviour {

    public GameObject[] cameraNodes;
    public GameObject[] lookNodes;

    private int nodeIndex = 0;

    public float moveSpeed;
    public float rotSpeed;

    //Rotation vars
    private float adjRotSpeed;
    private Quaternion targetRotation;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Movement();
        Rotation();
	}


    private void Movement() {

        //Increment or reset nodeIndex if close to current node
        if (Vector3.Distance(transform.position, cameraNodes[nodeIndex].transform.position) < 3.0f) {
            nodeIndex++;

            if (nodeIndex >= cameraNodes.Length)
                nodeIndex = 0;
        } 

        transform.position = Vector3.Lerp(transform.position, cameraNodes[nodeIndex].transform.position, moveSpeed * Time.deltaTime);
    }

    //Lerp rotate to look at next camera node object in camera path
    private void Rotation() {

        //rotate to look at enemy
        if (lookNodes[nodeIndex]) {
            targetRotation = Quaternion.LookRotation(lookNodes[nodeIndex].transform.position - transform.position);
            adjRotSpeed = Mathf.Min(rotSpeed * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
        }
        
    }
}
