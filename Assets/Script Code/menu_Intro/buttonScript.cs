using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private zoomIn zoomInScript;

    // This method will be called by the button click
    public void OnButtonClick()
    {
        StartCoroutine(SoundManager.Instance.PlayAudioClip("zoomInBoom", false));
       // StartCoroutine(zoomInScript.ZoomOutCoroutine());
       zoomInScript.timeToZoomOut = true;
    }
}
