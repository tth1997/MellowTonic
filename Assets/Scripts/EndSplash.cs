using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EndSplash : MonoBehaviour
{
    [HideInInspector]
    public Animator endTextAnimator;
    
    [HideInInspector]
    public Animator rawImageAnimator;

    bool sceneLoaded = false;

    AsyncOperation asyncLoad;

    void Start()
    {
        endTextAnimator = GameObject.Find("EndText").GetComponent<Animator>();

        rawImageAnimator = GameObject.Find("RawImage").GetComponent<Animator>();

        StartCoroutine(PreloadLevel());
    }

    private void Update()
    {
        if (sceneLoaded && Input.GetKeyDown(KeyCode.Space))
        {
            rawImageAnimator.SetTrigger("TriggerEndStateFade");

            StartCoroutine(WaitForFade());
        }
    }

    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(3f);

        asyncLoad.allowSceneActivation = true;
    }

    IEnumerator PreloadLevel()
    {
        yield return new WaitForSeconds(5f);

        asyncLoad = SceneManager.LoadSceneAsync("MainMenu");

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        endTextAnimator.SetTrigger("TriggerEndText");

        sceneLoaded = true;
        //SceneManager.LoadScene(2);
    }
}

