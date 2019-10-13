using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    Image fadeImg;
    Color fadeColor;

    private void Start()
    {
        fadeImg = GameObject.Find("FadeImg").GetComponent<Image>();
        fadeColor = Color.black;
        fadeImg.color = fadeColor;

        StopCoroutine(FadeCoroutine());
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        while (true)
        {
            fadeColor.a -= Time.fixedDeltaTime / 6;
            fadeImg.color = fadeColor;

            if (fadeColor.a <= 0)
            {
                StopCoroutine(FadeCoroutine());
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}