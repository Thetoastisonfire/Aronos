using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inUndergroundArea : MonoBehaviour
{
    public GameObject door; //gotta turn off the tp door
    public GameObject playerMain;
    // Start is called before the first frame update
    
#region handle enter and exit 

    // This method is called when another collider exits the trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other) {
        // Check if the exiting collider has the tag "Bird" or is the player's collider
        if (other.CompareTag("PlayerMain"))
        {  
            door.SetActive(false);
            SpriteRenderer sp = playerMain.GetComponent<SpriteRenderer>();
            Color c = sp.color; 
            c.a = 1f; //cause tp door coroutine dies make player alpha 1
            sp.color = c; // reassign the modified color back to the SpriteRenderer

            this.gameObject.SetActive(false); //turn off underground area
        }
    }

#endregion
}
