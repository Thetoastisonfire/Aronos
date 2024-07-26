using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class junkInteract : MonoBehaviour, Interactible //JUNK
{
     [Header ("YesNo Objects")]
    [SerializeField] private yesNoBox yesNoBoxScript;
    [SerializeField] private GameObject yesNoBox;
    [SerializeField] private CanvasGroup yesNoPanel;  // CanvasGroup of the Yes/No panel

     [Header ("Misc Objects")]
    [SerializeField] private playerHealth player;
    [SerializeField] private DialogScript dialogueScript;
    [SerializeField] private PlayerMovement pMove;
    [SerializeField] private GameObject dayChange;
    [SerializeField] private DaysChange dayChangeAgain;

     [Header ("Inventory")]
    [SerializeField] private inventoryScreenScript invScreenScript; //its in canvas -> inventory screen -> inventory panel -> inventory list area

     [Header ("Junk Data")]
    public string currentItem;
    //remove items from random pool
    private bool[] junkRemove = {false, false, false, false, false, false, false, false, false, false, false}; //10..?

     [Header ("Junk Bool")]
    public bool inJunk = false;

    public void Interact(){
        //Debug.Log("working");
        // if (yesNoPanel.alpha == 0) {
            yesNoBox.SetActive(true);
            yesNoBoxScript.PromptYesNoBox(pMove.transform.position, "junk");
     //    }
    }

    public void InteractYes() {
        yesNoBoxScript.which = "junkInteract";
        GlobalData.Instance.junkCounter += 1;
        player.takeDamage(20);
     //   dayChange.SetActive(true);
        junkUpdate();
    }

    void junkUpdate() {
        // Generate a random number between 1 and 10
        int randomNumber = Random.Range(1, 10);
   //     Debug.Log("Generated Number: " + randomNumber);
        if (GlobalData.Instance.junkCounter <= 5) {
            while(true){
                if (junkRemove[randomNumber]){
                    // If the random number is found in the array, increment it
                    randomNumber++;
                    // Check if the incremented number is still within the range
                    if (randomNumber > 10) randomNumber = 1; // If the incremented number exceeds 10, reset it to 1
                } else break; //break when unused number found
            }
            //items that exist; put effects here
            GlobalData.Instance.currentJunkItem = randomNumber;
            switch (randomNumber) {
                /* ItemID Guide--
                ------------------------------
                First Part of Tree
                ---------------
                -1: null
                0: not able to interact
                1: able to equip
                2: able to unequip
                3: able to use
                4: able to discard

                e.g. rune 1 not equipped = 13

                e.g. hat equipped = 24
             */
                case 1:
                    currentItem = "rusty pipe";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Rusty Pipe", 
                        "A jagged piece of rusted metal. One end appears to have been flattened roughly, flecked with specks of dirt.", 14001);
                    // FREE HATCH SCRATCH
                    break;
                case 2:
                    currentItem = "dull knife";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Dull Knife", 
                        "LOSE FRIENDSHIP", 4002);
                    // LOSE FRIENDSHIP
                    break;
                case 3:
                    currentItem = "misshapen meat lump";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Misshapen Meat", 
                        "Gain hunger", 34003);
                    // GAIN HUNGER
                    break;
                case 4:
                    currentItem = "old cloth puppet";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Old Cloth Puppet", 
                        "Gain friendship, tattered lil poppet wheeeee", 4004);
                    // GAIN FRIENDSHIP; its his old toy :3
                    break;
                case 5:
                    currentItem = "old tire";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Old Tire", 
                        "Junk, does nothing", 4005);
                    // DOES NOTHING; tire appears in corner
                    break;
                case 6:
                    currentItem = "tattered hat";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Tattered Hat", 
                        "Does nothing, but looks cool", 14006);
                    // DOES NOTHING; appears in corner; can wear hat
                    break;
                case 7:
                    currentItem = "broken geometric sphere trinket";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Geometric Trinket", 
                        "A broken trinket from the old gods. Secretly decreases hunger drain.", 14007);
                    // SECRETLY PERMANENTLY DECREASES HUNGER DRAIN; ‘a symbol of the heaven shapes, embossed with silver. It shimmers faintly.’
                    break;
                case 8:
                    currentItem = "cracked egg";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Cracked Egg", 
                    "Remnants of an egg. Whatever hatched from this is long gone. Some blue tattered" +
                    " cloth seems to remain on the sharp shell, left behind and forgotten. Secretly increase frienship gain.", 14008);
                    // SECRETLY PERMANENTLY INCREASES FRIENDSHIP GAIN; ‘whatever hatched from this is long gone. Some blue tattered cloth seems to remain on the sharp shell, left behind and forgotten.’
                    break;
                case 9:
                    currentItem = "cracked mask";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Cracked Mask", "It stares at you. Increase random attack chance.", 13009);
                    // INCREASES CHANCE FOR RANDOM ATTACKS; ’it stares at you.’
                    break;
                case 10:
                    currentItem = "weathered timepiece";
                    invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[randomNumber-1], 1, "Weathered Timepiece", "Is it timeless, or stagnant? Stats locked for the day.", 34010);
                    // BOTH FRIENDSHIP AND HUNGER ARE LOCKED FOR THE DAY; ’is it timeless, or stagnant?’
                    break;
                default:
                    currentItem = "nothing";
                    break;
            }
            junkRemove[randomNumber] = true; //take off random number from list

        } else currentItem = "nothing"; //if junk was accessed 5 times
        
        dayChange.SetActive(true);
        dayChangeAgain.subsequentStart();
        //dialogueScript.subsequentStart();
    }


    void OnTriggerEnter2D(Collider2D other) {
        // Check if the entering collider has the tag "Bird" or is the player's collider
     //   Debug.Log("triggered");
        if (other.CompareTag("PlayerMain"))
        {
         //   Debug.Log("player triggered");
            inJunk = true;
        }
    }

    // This method is called when another collider exits the trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other) {
        // Check if the exiting collider has the tag "Bird" or is the player's collider
        if (other.CompareTag("PlayerMain"))
        {
            inJunk = false;
        }
    }

}
