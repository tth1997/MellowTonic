using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SplashFade : MonoBehaviour
{
    public float wait_time;

    float remainingTime;

    void Start()
    {
        StartCoroutine(Wait_for_intro());
    }

    private void Update()
    {

    }

    IEnumerator Wait_for_intro()
    {
        yield return new WaitForSeconds(wait_time);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);

        //SceneManager.LoadScene(2);
    }
}

