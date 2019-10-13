using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoPlay : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;

    Color fadeColor;

    bool isPlayingVideo = false;

    void Start()
    {
        videoPlayer.Prepare();
        //StartCoroutine(PlayVideo());

        fadeColor = Color.white;
        rawImage.color = fadeColor;

        StopCoroutine(FadeCoroutine());
        StartCoroutine(FadeCoroutine());
    }

    private void Update()
    {
        if (videoPlayer.isPrepared && !isPlayingVideo)
        {
            rawImage.texture = videoPlayer.texture;
            videoPlayer.Play();
            audioSource.Play();
            isPlayingVideo = true;
        }
    }

    IEnumerator FadeCoroutine()
    {
        while (true)
        {
            fadeColor.r -= Time.fixedDeltaTime / 20;
            fadeColor.g -= Time.fixedDeltaTime / 20;
            fadeColor.b -= Time.fixedDeltaTime / 20;

            rawImage.color = fadeColor;

            if (fadeColor.r <= 0)
            {
                StopCoroutine(FadeCoroutine());
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
