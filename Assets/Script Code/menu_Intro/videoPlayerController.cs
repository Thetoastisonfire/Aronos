using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip firstVideoClip; // Drag and drop your first video clip here
    public VideoClip secondVideoClip; // Drag and drop your second video clip here

    private Camera _mainCamera;
    private bool fadingToBlack = false;
    private bool startPlayingSecondVideo = false;
    private Color originalBackgroundColor;
    private Color targetBackgroundColor = Color.black;
    private float fadeDuration = 2f;
    private float fadeTimer = 0f;

    void Start()
    {
        _mainCamera = Camera.main;
        originalBackgroundColor = _mainCamera.backgroundColor;

        // Assign the first video clip to the video player
        videoPlayer.clip = firstVideoClip;

        // Play the first video
        PlayVideo();
    }

    void Update()
    {
        if (fadingToBlack)
        {
            // Increment the fade timer
            fadeTimer += Time.deltaTime;

            // Interpolate between original and target background color
            _mainCamera.backgroundColor = Color.Lerp(originalBackgroundColor, targetBackgroundColor, fadeTimer / fadeDuration);

            if (fadeTimer >= fadeDuration && !startPlayingSecondVideo)
            {
                // Start playing the second video
                PlaySecondVideo();
                startPlayingSecondVideo = true;
            }
        }
    }

    void PlayVideo()
    {
        // Start playing the video
        videoPlayer.Play();

        // Start playing the audio
        StartCoroutine(SoundManager.Instance.PlayBackgroundMusic("atmosphereForEye"));

        // Start fading to black after a short delay
        StartCoroutine(StartFading());
    }

    IEnumerator StartFading()
    {
        // Wait for the first video to almost end
        yield return new WaitForSeconds((float)videoPlayer.clip.length - 0.5f);

        // Start fading to black
        fadingToBlack = true;
        fadeTimer = 0f;
    }

    void PlaySecondVideo()
    {
        // DONT STOP AUDIO!!!
        // audioSource.Stop();

        // Load and play the second video
        videoPlayer.Stop();
        videoPlayer.clip = secondVideoClip;
        videoPlayer.isLooping = true; // Loop the second video
        videoPlayer.Play();

        // Reset fade properties
        fadeTimer = 0f;
        fadingToBlack = false;
        _mainCamera.backgroundColor = originalBackgroundColor;
    }
}
