using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [HideInInspector]
    public MouseController mouseController;
    public GameObject AICar;
    public GameObject menuCanvas;


    public bool paused;

    int spawnLimit = 16;
    float spawnTimer;
    int lastLaneNum;
    public bool spawnAI;
    bool isSpawning;

    void Start()
    {
        mouseController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MouseController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        paused = false;
        if (menuCanvas)
        {
            menuCanvas.SetActive(false);
        }

        spawnTimer = Time.time + 20f;
        isSpawning = false;
        StopCoroutine(AICarSpawnCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PauseCheck();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R) && Application.isEditor)
        {
            SceneManager.LoadScene("Level");
        }

        if (spawnAI)
        {
            if (Time.time > spawnTimer && !isSpawning)
            {
                StartCoroutine(AICarSpawnCoroutine());
                isSpawning = true;
            }
        }
    }

    public void PauseCheck()
    {
        paused = !paused;

        mouseController.paused = paused;

        if (paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            menuCanvas.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
            menuCanvas.SetActive(false);
        }
    }

    IEnumerator AICarSpawnCoroutine()
    {
        while (true)
        {
            Transform playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;
            GameObject newAICar;
            AICarMovementScript newAIScript;

            float newMoveSpeed = Random.Range(25f, 30.5f);
            float waypointRandom = Random.Range(1f, 10f);
            int newWaypoint;
            if (waypointRandom > 6f)
            {
                newWaypoint = 2;
            }
            else
            {
                newWaypoint = 1;
            }

            var AICars = FindObjectsOfType<AICarMovementScript>();
            if (AICars.Length < spawnLimit)
            {
                if (newWaypoint == 1)
                {
                    if (newMoveSpeed > playerCarTransform.GetComponent<CarMovementScript>().carVel)
                    {
                        newAICar = Instantiate(AICar, new Vector3(Mathf.Clamp(playerCarTransform.position.x + 300f, -10000f, 390f), 2f, -120f), Quaternion.Euler(Vector3.zero));
                        newAIScript = newAICar.GetComponent<AICarMovementScript>();
                        newAIScript.moveSpeed = newMoveSpeed;
                        newAIScript.waypointNum = newWaypoint;

                        if (lastLaneNum == 1)
                        {
                            newAIScript.laneNum = 2;
                        }
                        else if (lastLaneNum == 2)
                        {
                            newAIScript.laneNum = 1;
                        }
                        else
                        {
                            newAIScript.laneNum = Random.Range(1, 3);
                        }
                        lastLaneNum = newAIScript.laneNum;
                    }
                    else
                    {
                        newAICar = Instantiate(AICar, new Vector3(Mathf.Clamp(playerCarTransform.position.x - 300f, -10000f, 390f), 2f, -200f), Quaternion.Euler(Vector3.zero));
                        newAIScript = newAICar.GetComponent<AICarMovementScript>();
                        newAIScript.moveSpeed = newMoveSpeed;
                        newAIScript.waypointNum = newWaypoint;

                        if (lastLaneNum == 1)
                        {
                            newAIScript.laneNum = 2;
                        }
                        else if (lastLaneNum == 2)
                        {
                            newAIScript.laneNum = 1;
                        }
                        else
                        {
                            newAIScript.laneNum = Random.Range(1, 3);
                        }
                        lastLaneNum = newAIScript.laneNum;
                    }
                }
                else if (newWaypoint == 2)
                {
                    newAICar = Instantiate(AICar, new Vector3(Mathf.Clamp(playerCarTransform.position.x - 300f, -10000f, 390f), 2f, -200f), Quaternion.Euler(Vector3.zero));
                    newAIScript = newAICar.GetComponent<AICarMovementScript>();
                    newAIScript.moveSpeed = newMoveSpeed;
                    newAIScript.waypointNum = newWaypoint;


                    if (lastLaneNum == 3)
                    {
                        newAIScript.laneNum = 4;
                    }
                    else if (lastLaneNum == 4)
                    {
                        newAIScript.laneNum = 3;
                    }
                    else
                    {
                        newAIScript.laneNum = Random.Range(3, 5);
                    }
                    lastLaneNum = newAIScript.laneNum;
                }
            }
            Debug.Log("Num of AI cars: " + AICars.Length);
            Debug.Log("New AI car Instantiated.");
            yield return new WaitForSeconds(5f);

            yield return null;
        }
    }

    public void Crash()
    {
        Debug.Log("Crashed!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndState");
    }
}
