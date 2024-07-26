using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class zoomIn : MonoBehaviour
{


#region variables

     [Header ("Scaling vectors")]
    public Vector3 initialScale;
    public Vector3 targetScale;

     [Header ("Zoom Variables")]
    public int zoomAmount = 9;
    public float zoomDuration = 2f; // Duration of the zoom in seconds
    
     [Header ("Zoom Booleans")]
    public bool zoomed = false;
    public bool timeToZoomOut = false;
    private bool backgroundMusicLatch = true;

     [Header ("scene to load")]
    [SerializeField] private string sceneToLoad = "IntroScene"; // name of next scene

    [Header ("Misc Objects")]
    [SerializeField] private GameObject MenuSun;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private Color targetBackgroundColor;
    [SerializeField] private GameObject MenuSun2;

    private Camera _mainCamera;

#endregion

#region startup
    void Awake() {
        _mainCamera = Camera.main;
    }

    void Start()
    {
        initialScale = MenuSun.transform.localScale;
        targetScale = initialScale * zoomAmount; // Increase scale by 9?  
    }

#endregion

#region main zoom code
    IEnumerator ZoomInCoroutine() {
        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration) {
            MenuSun.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // After lerping is complete
        MenuSun2.SetActive(true);
        zoomDone();
    }

    public IEnumerator ZoomOutCoroutine() {
        Debug.Log("zoom out is called");
        Canvas.SetActive(false); // Assuming Canvas is your GameObject to deactivate during zoom out
         
        
        initialScale = MenuSun2.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < zoomDuration) {
            MenuSun2.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After zoom out is complete
        prepForNextScene();
    }


    void Update() {
        //play background music once
        if (backgroundMusicLatch && SoundManager.Instance.initializationComplete) {
            StartCoroutine(SoundManager.Instance.PlayBackgroundMusic("shadowsEmbrace"));  
            backgroundMusicLatch = false; 
        }

        //click and tap logic
        if (!zoomed) checkTouch();

        if(timeToZoomOut){
            timeToZoomOut = false;
            StartCoroutine(ZoomOutCoroutine());
        }

    }//end of update

    private void checkTouch() {
        if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)){
                // Check if the mouse button was clicked and the sprite has not been zoomed yet
                // Reset timer and set zoomed to true to prevent further zooming
                StartCoroutine(SoundManager.Instance.PlayAudioClip("zoomInBoom", false));
                zoomed = true;
                StartCoroutine(ZoomInCoroutine());
        }
    }

    private void zoomDone() { // Reset timer and set zoomed to true to prevent further zooming
            Canvas.SetActive(true);
            _mainCamera.backgroundColor = targetBackgroundColor;
            MenuSun.SetActive(false);
            Debug.Log("Click detected 2!");
    }

    private void prepForNextScene() {
        MenuSun2.SetActive(false);
        _mainCamera.backgroundColor = Color.Lerp(targetBackgroundColor, Color.black, 1);
        SceneManager.LoadScene(sceneToLoad); //move on from menu
    }

#endregion

}
