using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustColliderToFeet : MonoBehaviour
{
    public BoxCollider2D boxCollider;  // Reference to the BoxCollider2D
    public Vector2 feetLocalPosition;  // Local position of the feet in the sprite's space
    public float offsetY = 0f;         // Additional offset if needed\
    public PlayerMovement playerMovement;


    void Start() {
        playerMovement = GameObject.FindGameObjectWithTag("PlayerMain").GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if (boxCollider != null && playerMovement.moveInput != 0)
        {
            // Calculate the new offset for the collider
            Vector2 newOffset = boxCollider.offset;
            newOffset.y = feetLocalPosition.y + offsetY;
            boxCollider.offset = newOffset;
        }

        else if (boxCollider != null && playerMovement.moveInput == 0)
        {
            // Calculate the new offset for the collider
            Vector2 newOffset = boxCollider.offset;
            newOffset.y = feetLocalPosition.y - offsetY;
            boxCollider.offset = newOffset;
        }
    }
}
