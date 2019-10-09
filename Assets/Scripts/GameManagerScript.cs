using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [HideInInspector]
    public MouseController mouseController;
    public GameObject AICar;
    bool paused = false;

    int spawnLimit = 5;
    float spawnTimer;
    bool isSpawning;

    void Start()
    {
        mouseController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MouseController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;

        spawnTimer = Time.time + 20f;
        isSpawning = false;
        StopCoroutine(AICarSpawnCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            paused = !paused;

            mouseController.paused = paused;

            if (paused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }

        if (Time.time > spawnTimer && !isSpawning)
        {
            StartCoroutine(AICarSpawnCoroutine());
            isSpawning = true;
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
            int newWaypoint = Random.Range(1, 3);

            var AICars = FindObjectsOfType<AICarMovementScript>();
            if (AICars.Length < spawnLimit)
            {
                if (newWaypoint == 1)
                {
                    if (newMoveSpeed > playerCarTransform.GetComponent<CarMovementScript>().carVel)
                    {
                        newAICar = Instantiate(AICar, new Vector3(Mathf.Clamp(playerCarTransform.position.x + 300f, -10000f, 390f), 2f, -200f), Quaternion.Euler(Vector3.zero));
                        newAIScript = newAICar.GetComponent<AICarMovementScript>();
                        newAIScript.moveSpeed = newMoveSpeed;
                        newAIScript.waypointNum = newWaypoint;
                    }
                    else
                    {
                        newAICar = Instantiate(AICar, new Vector3(Mathf.Clamp(playerCarTransform.position.x - 300f, -10000f, 390f), 2f, -200f), Quaternion.Euler(Vector3.zero));
                        newAIScript = newAICar.GetComponent<AICarMovementScript>();
                        newAIScript.moveSpeed = newMoveSpeed;
                        newAIScript.waypointNum = newWaypoint;
                    }
                }
                else if (newWaypoint == 2)
                {
                    newAICar = Instantiate(AICar, new Vector3(Mathf.Clamp(playerCarTransform.position.x - 300f, -10000f, 390f), 2f, -200f), Quaternion.Euler(Vector3.zero));
                    newAIScript = newAICar.GetComponent<AICarMovementScript>();
                    newAIScript.moveSpeed = newMoveSpeed;
                    newAIScript.waypointNum = newWaypoint;
                }
            }

            Debug.Log("New AI car Instantiated.");
            yield return new WaitForSeconds(15f);

            yield return null;
        }
    }

    public void Crash()
    {
        Debug.Log("Crashed!");
        SceneManager.LoadScene(3);
    }
}
