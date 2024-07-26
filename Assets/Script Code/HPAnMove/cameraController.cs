using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("player following data")]
    public float speed = 3f;  // Speed of camera movement
    public PlayerMovement pMove;  // Reference to the player movement script
    private Vector3 velocity = Vector3.zero;  // Velocity for SmoothDamp

     [Header("camera borders")]
    public float minX;  // Minimum x position for the camera
    public float maxX;  // Maximum x position for the camera
    public float minY;  // Minimum y position for the camera
    public float maxY;  // Maximum y position for the camera
    public float yOffset = 5.0f;  // Offset value for the y position

     [Header("zoom data")]
   // public float normalZoom = 5f;  // Normal zoom level of the camera
   // public float zoomedOutSize = 8f;  // Zoomed out size of the camera
    public float zoomSpeed = 2f;  // Speed at which the camera zooms

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Define the target position based on the player's position with an offset
        Vector3 targetPosition = new Vector3(pMove.transform.position.x, pMove.transform.position.y + yOffset, transform.position.z);

        // Clamp the target position to stay within the min and max bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, speed * Time.deltaTime);
    }

    public IEnumerator ZoomCamera(float targetSize, float speed)
    {   
        float currentVelocity = 0f;

        while (!Mathf.Approximately(cam.orthographicSize, targetSize))
        {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref currentVelocity, speed);
            yield return null;
        }
    }

    public IEnumerator offsetUpdate(int targetOffset, float speed) {
        //cause its only used in heaven area currently
        maxX = 750f;
        maxY = 300f; //shouldn't go too high

        float startOffset = yOffset;
        float elapsedTime = 0f;

        while (elapsedTime < speed)
        {
            yOffset = Mathf.Lerp(startOffset, targetOffset, elapsedTime / speed );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set after the lerp completes
        yOffset = targetOffset;
    }


}
