using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SplashFade : MonoBehaviour
{
    [HideInInspector]
    public Animator startTextAnimator;

    bool sceneLoaded = false;

    AsyncOperation asyncLoad;

    void Start()
    {
        startTextAnimator = GameObject.Find("StartText").GetComponent<Animator>();

        StartCoroutine(PreloadLevel());
    }

    private void Update()
    {
        if (sceneLoaded && Input.GetKeyDown(KeyCode.Space))
        {
            asyncLoad.allowSceneActivation = true;
        }
    }

    IEnumerator PreloadLevel()
    {
        yield return new WaitForSeconds(0.1f);

        asyncLoad = SceneManager.LoadSceneAsync("Level");

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        startTextAnimator.SetTrigger("TriggerStartText");

        sceneLoaded = true;
        //SceneManager.LoadScene(2);
    }
}

