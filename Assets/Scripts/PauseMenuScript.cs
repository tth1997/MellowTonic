using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    [HideInInspector]
    public GameManagerScript gameManagerScript;

    private void Start()
    {
        gameManagerScript = GameObject.Find("EventSystem").GetComponent<GameManagerScript>();
    }

    public void ResumeGame()
    {
        gameManagerScript.PauseCheck();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
