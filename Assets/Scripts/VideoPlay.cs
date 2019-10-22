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

    bool isPlayingVideo = false;

    void Start()
    {
        videoPlayer.Prepare();
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
}
