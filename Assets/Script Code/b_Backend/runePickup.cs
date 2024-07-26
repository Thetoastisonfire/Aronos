using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class runePickup : MonoBehaviour, Interactible //RUNE
{   
     [Header ("Inventory")]
    [SerializeField] private inventoryScreenScript invScreenScript;
    private bool runeRadius = false;
    [SerializeField] private GameObject currentRune;
    [SerializeField] private int whichRune = 0;
    [SerializeField] private InventoryStat invStat;
    private bool pickedUpAlready = false;

     [Header ("Anim and Audio")]
    private Animator anim;

    public void Awake() {
        anim = GetComponent<Animator>();
    }


    public void Interact() {
        StartCoroutine(SoundManager.Instance.PlayAudioClip("zoomInBoom", false));

        if (!pickedUpAlready) {
            pickedUpAlready = true; 

            if (runeRadius) runeUpdate();
            anim.SetBool("runeTime", true);
        }
    }

    private void runeUpdate() {
        //Depending on which rune, will add to inventory
        invStat.updateRune();
        switch (whichRune) {
            case 0:
                invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[10], 1, "Light Rune", 
                        "Allows one to activate the inherent strength of fading light. Double tap to use.", 13);
                break;
            case 1:
                invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[11], 1, "Death Rune", 
                        "Allows one to activate the inherent strength of decay. Double tap to use.", 13);
                break;
            default:
                break;
        
        }

    }

    private void deleteRune() { //when already picked up
        anim.enabled = false;
        currentRune.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Check if the entering collider has the tag "Bird" or is the player's collider
     //   Debug.Log("triggered");
        if (other.CompareTag("PlayerMain"))
        {
         //   Debug.Log("player triggered");
            runeRadius = true;
        }
    }

    // This method is called when another collider exits the trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other) {
        // Check if the exiting collider has the tag "Bird" or is the player's collider
        if (other.CompareTag("PlayerMain"))
        {
            runeRadius = false;
        }
    }

}
