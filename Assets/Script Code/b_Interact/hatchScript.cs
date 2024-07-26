using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class hatchScript : MonoBehaviour, Interactible //HATCH
{
    [Header ("Hatch Objects")]
    [SerializeField] private GameObject slash1;
    [SerializeField] private GameObject slash2;
    [SerializeField] private GameObject slash3;
    [SerializeField] private GameObject hatchWinScreen;

    [Header ("YesNo Objects")]
    [SerializeField] private yesNoBox yesNoBoxScript;
    [SerializeField] private GameObject yesNoBox;
    [SerializeField] private CanvasGroup yesNoPanel;  // CanvasGroup of the Yes/No panel

    [Header ("Misc. Objects")]
    [SerializeField] private GameObject dayChange;
    [SerializeField] private DaysChange dayChangeAgain;
    [SerializeField] private playerHealth player;
    [SerializeField] private PlayerMovement pMove;
    [SerializeField] private string sceneToLoad = "HeavenScene";

    public void Awake() {
        slash1.SetActive(false);
        slash2.SetActive(false);
        slash3.SetActive(false);
    }
    
    public void Interact(){
        //Debug.Log("working");
         if (yesNoPanel.alpha == 0) {
            yesNoBox.SetActive(true);
            yesNoBoxScript.PromptYesNoBox(pMove.transform.position, "hatch");
         }
    }

    public void InteractYes() {
        //Debug.Log("working");
        if (GlobalData.Instance.hatchCounter < 3) {
        GlobalData.Instance.hatchCounter += 1;

        if (GlobalData.Instance.equippedItemEffect == "hatch") { //if using pipe
            //delete pipe, reset data thing, show it got used
            GlobalData.Instance.equippedItemEffect = "none";
            //yesNoBoxScript.PromptJustText("pipeUse");
        } else player.takeDamage(50);

        hatchUpdate();
        }
        else { //you win!!
            // youWin();
            SceneManager.LoadScene(sceneToLoad);
        }     //you win screen for debug, replaced with heaven ascenscion 
        hatchUpdate();
    }

    void hatchUpdate() {
        yesNoBoxScript.which = "hatch";
        dayChange.SetActive(true);
        dayChangeAgain.subsequentStart();
        switch(GlobalData.Instance.hatchCounter) {
            case 1:
                slash1.SetActive(true);
                break;
            case 2:
                slash2.SetActive(true);
                break;
            case 3:
                slash3.SetActive(true);
                break;
            default:
                break;
        }
       GlobalData.Instance.currentlyInteracting = false;
    }

    private void youWin(){
         StartCoroutine(SoundManager.Instance.PlayAudioClip("win", false));
              Debug.Log("soundPlayed: Win");
            hatchWinScreen.SetActive(true);
            GlobalData.Instance.speed = 20f;
            GlobalData.Instance.jumpForce = 30f;
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Check if the entering collider has the tag "Bird" or is the player's collider
     //   Debug.Log("triggered");
        if (other.CompareTag("PlayerMain"))
        {
         //   Debug.Log("player triggered");
            GlobalData.Instance.inHatch = true;
        }
    }

    // This method is called when another collider exits the trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other) {
        // Check if the exiting collider has the tag "Bird" or is the player's collider
        if (other.CompareTag("PlayerMain"))
        {
            GlobalData.Instance.inHatch = false;
        }
    }

}
