using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScale : MonoBehaviour
{
    // Reference to the sprite renderer component
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Get the sprite renderer component attached to the GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
            return;
        }

        // Calculate the screen aspect ratio
        float screenAspectRatio = (float)Screen.width / Screen.height;

        // Calculate the sprite aspect ratio
        float spriteAspectRatio = spriteRenderer.sprite.bounds.size.x / spriteRenderer.sprite.bounds.size.y;

        // Scale the sprite to fit the screen
        if (screenAspectRatio > spriteAspectRatio)
        {
            // Screen is wider than the sprite
            float newScaleX = spriteRenderer.transform.localScale.x * (screenAspectRatio / spriteAspectRatio);
            spriteRenderer.transform.localScale = new Vector3(newScaleX, newScaleX, 1);
        }
        else
        {
            // Screen is taller than the sprite or has the same aspect ratio
            float newScaleY = spriteRenderer.transform.localScale.y * (spriteAspectRatio / screenAspectRatio);
            spriteRenderer.transform.localScale = new Vector3(newScaleY, newScaleY, 1);
        }
    }
}

