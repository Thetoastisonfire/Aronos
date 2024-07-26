using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControllerHeaven : MonoBehaviour
{
    public float speed = 3f;  // Speed of camera movement
    public PlayerMovementHeaven pMove;  // Reference to the player movement script
    private Vector3 velocity = Vector3.zero;  // Velocity for SmoothDamp

    public float minX;  // Minimum x position for the camera
    public float maxX;  // Maximum x position for the camera
    public float minY;  // Minimum y position for the camera
    public float maxY;  // Maximum y position for the camera
    public float yOffset = 5.0f;  // Offset value for the y position

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
}
