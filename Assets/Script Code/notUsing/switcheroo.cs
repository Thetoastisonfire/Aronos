using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class switcheroo : MonoBehaviour
{ /*
    // Start is called before the first frame update

    public zoomIn zoom;

    private bool zoomingOut = false;

    public int zoomOutScaleUp = 4;
    public GameObject MenuSun;

    public GameObject Canvas;


    private Camera _mainCamera;
    public SpriteRenderer spriteRenderer;
    public Color targetBackgroundColor;

    public string sceneToLoad = "IntroScene"; // Name of the scene to load

    void Awake() {
        _mainCamera = Camera.main;
        _mainCamera.backgroundColor = targetBackgroundColor;
    }


    void Start()
    {
        transform.localScale = zoom.initialScale * zoom.zoomAmount; //should scale to target scale
    }

    // Update is called once per frame
    void Update()
    {
        if (zoomingOut && zoom.zoomTimer < zoom.zoomDuration) //zoom out
        {
            // Increment the timer
            zoom.zoomTimer += Time.deltaTime;

            // Interpolate between initial scale and target scale
            transform.localScale = Vector3.Lerp(zoom.targetScale, zoom.initialScale / zoomOutScaleUp, zoom.zoomTimer / zoom.zoomDuration);
        }

         else if (zoomingOut && zoom.zoomTimer >= zoom.zoomDuration)
        {
            // add new scene logic here
            MenuSun.SetActive(false);
            zoomingOut = false;
        //    SetOpacity(0f);
            _mainCamera.backgroundColor = Color.Lerp(targetBackgroundColor, Color.black, 1);
            SceneManager.LoadScene(sceneToLoad); //move on from menu

        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {

        if (!context.started) return;
        
        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        // Check if the mouse button was clicked and the sprite HAS been zoomed yet
        if (context.started && zoom.zoomed)
        {
            // Reset timer and set zoomed to true to prevent further zooming
            zoom.zoomTimer = 0f;
            zoomingOut = true;
      //      zoom.SetOpacity(0f);
            Canvas.SetActive(false);
            Debug.Log("Click detected 2!");
            
        }
    }
*/
}
