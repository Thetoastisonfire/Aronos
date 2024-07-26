using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class jumpPillarScript : MonoBehaviour, Interactible
{
   
#region variables
     [Header ("GameObjects")]
    [SerializeField] private inventoryScreenScript invScreenScript;
    [SerializeField] private GameObject jumpPillar;
    [SerializeField] private PlayerMovement pMove;

     [Header ("Variables")]

    [SerializeField] private float pillarPushForce;
    [SerializeField] private int pillarDirection; //1 2 3 4 is up down left right
    private bool jumpRadius = false;
     [Header ("Anim and Audio")]
    [SerializeField] private Animator anim;

#endregion

    public void Awake() {
        anim = jumpPillar.GetComponent<Animator>();
        anim.SetBool("idle", true);
    }

    public void Interact() {
        if (jumpRadius) { 
            pillarUpdate(); //jump radius true on trigger enter
            anim.SetBool("jumping", true); //trying to call pillarUpdate() on an animation event
        }
    }


    public void pillarUpdate() {
        Debug.Log("pillar update");

        // Get the top position of the jump pillar
        Vector3 jumpPillarTop = jumpPillar.transform.position + Vector3.up * (jumpPillar.transform.localScale.y / 2);
        Debug.Log("Jump pillar top: " + jumpPillarTop);

        // Move the player to the top of the jump pillar
        pMove.transform.position = jumpPillarTop;

        Vector2 direction = Vector2.zero;

        // Set the direction based on the pillarDirection
        switch(pillarDirection) {
            case 2: // down
                direction = Vector2.down;
                break;
            case 3: // left
                direction = Vector2.left;
                break;
            case 4: // right
                direction = Vector2.right;
                break;
            case 1: // up
            default:
                direction = Vector2.up;
                break;
        }
        Debug.Log("Direction: " + direction);

        // Apply the movement to the player
        Rigidbody2D rb = pMove.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.velocity = Vector2.zero;  // Reset velocity
            rb.velocity = direction * pillarPushForce;  // Apply new velocity
            Debug.Log("Player velocity: " + rb.velocity);
        } else {
            Debug.LogError("Rigidbody2D not found on player");
        }
        StartCoroutine(ApplyMovementForDuration(direction, 3f));

        anim.SetBool("jumping", false);
    }

    private IEnumerator ApplyMovementForDuration(Vector2 direction, float duration) {
        // Get the player's Rigidbody2D component
        Rigidbody2D rb = pMove.GetComponent<Rigidbody2D>();
        if (rb != null) {
            // Set the initial velocity
            rb.velocity = direction * pillarPushForce;
            Debug.Log("Player initial velocity: " + rb.velocity);

            // Wait for the specified duration
            yield return new WaitForSeconds(duration);

            // Stop the movement by setting the velocity to zero
            rb.velocity = Vector2.zero;
            Debug.Log("Player velocity after stopping: " + rb.velocity);
        } else {
            Debug.LogError("Rigidbody2D not found on player");
        }
    }


#region handle enter and exit 
    void OnTriggerEnter2D(Collider2D other) {
        // Check if the entering collider has the tag "Bird" or is the player's collider
     //   Debug.Log("triggered");
        if (other.CompareTag("PlayerMain"))
        {
         //   Debug.Log("player triggered");
            jumpRadius = true;
            anim.SetBool("idle", false);
        }
    }

    // This method is called when another collider exits the trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other) {
        // Check if the exiting collider has the tag "Bird" or is the player's collider
        if (other.CompareTag("PlayerMain"))
        {
            jumpRadius = false;
            anim.SetBool("idle", true);
        }
    }

#endregion

}
