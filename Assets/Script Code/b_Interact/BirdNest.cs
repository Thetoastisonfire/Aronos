using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BirdNest : MonoBehaviour, Interactible
{
     [Header ("YesNo Objects")]
    [SerializeField] private yesNoBox yesNoBoxScript;
    [SerializeField] private GameObject yesNoBox;
    [SerializeField] private CanvasGroup yesNoPanel;  // CanvasGroup of the Yes/No panel

     [Header ("Sleep Bool")]
    public bool inNest = false;
    
     [Header ("Misc Objects")]
    [SerializeField] private playerHealth player;
    [SerializeField] private PlayerMovement pMove;
    [SerializeField] private GameObject dayChange; 
    [SerializeField] private DaysChange dayChangeAgain;


    public void Interact(){
        //Debug.Log("working");
         if (yesNoPanel.alpha == 0) {
            yesNoBox.SetActive(true);
            yesNoBoxScript.PromptYesNoBox(pMove.transform.position, "nest");
         }
    }

    public void InteractYes() {
        //maybe add sound effects for sleeping here or smth idk
        nestUpdate();
    }

     void OnTriggerEnter2D(Collider2D other) {
        // Check if the entering collider has the tag "Bird" or is the player's collider
     //   Debug.Log("triggered");
        if (other.CompareTag("PlayerMain"))
        {
         //   Debug.Log("player triggered");
            inNest = true;
        }
    }

    // This method is called when another collider exits the trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other) {
        // Check if the exiting collider has the tag "Bird" or is the player's collider
        if (other.CompareTag("PlayerMain"))
        {
            inNest = false;
        }
    }

    void nestUpdate(){ //will change the day
        player.takeDamage(20); 

        //debug
     //   player.takeDamage(100); 
       // player.takeEmotionalDamage(100); 

        player.checkHealth(); //just sleeping!!!

        //next day if not dead; 0 is hunger death, 1 is friend death, 2 is alive
        GlobalData.Instance.dayCount += 1;
        GlobalData.Instance.tiredCounter = 0;
      //  GlobalData.Instance.minimumTired += 1; //this increases the amount of actions but also it currently lets you rush the hatch no problem
        GlobalData.Instance.currentlyInteracting = false;
        GlobalData.Instance.alreadyMentionedJunk = false; //see global data for what this does
        dayChange.SetActive(true); //should autocheck which dayschange message to use in dayschange at CheckIfSleep()
        dayChangeAgain.subsequentStart();
    }
}
