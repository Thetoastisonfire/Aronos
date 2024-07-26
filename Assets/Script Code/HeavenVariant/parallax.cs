using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    [SerializeField] private PlayerMovement pMove; // Reference to the player's movement script
    private Vector3 previousPlayerPosition;
    [SerializeField] private float parallaxFactorX = 5f;
    //[SerializeField] private float parallaxFactorY = 20f;
    private Vector3 deltaMovement;
    private Vector3 newPosition;
    
    void Start()
    {
        if (pMove == null)
        {
            pMove = FindObjectOfType<PlayerMovement>(); // Find the player movement script if not assigned
        }
        
        previousPlayerPosition = pMove.transform.position; // Initialize the previous player position
    }

    void Update()
    {
        deltaMovement = pMove.transform.position - previousPlayerPosition; // Calculate the movement delta
        newPosition = transform.position; // Get the current position

        newPosition.x += deltaMovement.x / parallaxFactorX; // Apply the parallax effect on the x-axis
       // newPosition.y += deltaMovement.y / parallaxFactorY; // Apply the parallax effect on the y-axis

        transform.position = newPosition;

        previousPlayerPosition = pMove.transform.position; // Update the previous player position
    }
}
