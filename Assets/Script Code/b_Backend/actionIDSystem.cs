using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public class actionIDSystem : MonoBehaviour
{
    public static actionIDSystem Instance { get; private set; }

    [Header("action id values")]
    public bool yieldBreak = false;
    public bool unable = false; // if was unable to perform inventory action
    (int firstDigit, int secondDigit, int thirdDigits) currentActionID = (-1, -1, -1);
    public int currentActionIDForReal = -1;
    public int currentlySelectedItemListID = -1;
    public bool shouldDeleteCurrentItem = false;

    [Header("Misc Objects")]
    [SerializeField] private yesNoBox ynBox;
    [SerializeField] private playerHealth pHealth;
    [SerializeField] private inventoryScreenScript invScreenScript;
    [SerializeField] private hatchScript hatch;
    [SerializeField] private BirdNest birdNest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string ReturnString(int actionID) {

        #region calculating action id system things
            
            //for other methods
            currentActionIDForReal = actionID;
            Debug.Log("action id is "+ actionID + ", idforReal is " + currentActionIDForReal);

            // Extract digits action ID, e.g. 12011 
            string actionIDStr = actionID.ToString();
            char[] actionIDChars = actionIDStr.ToCharArray();

            currentActionID.firstDigit = int.Parse(actionIDChars[0].ToString()); // Get the first digit; see actionID guide in globalscript

            currentActionID.secondDigit = int.Parse(actionIDChars[1].ToString()); // Get the second digit
            // Find the index of '0'
            int zeroIndex = actionIDStr.IndexOf('0');
            if (zeroIndex == -1)
            {
                Debug.LogError("Action ID does not contain a '0'.");
                currentActionID.thirdDigits = -1;
            }
            
            // Get the digits after '0'
            string thirdDigitsStr = actionIDStr.Substring(zeroIndex + 1);
            if (int.TryParse(thirdDigitsStr, out int thirdDigits))
            {
                currentActionID.thirdDigits = thirdDigits; //get the last two digits
            }
            else
            {
                Debug.LogError("Failed to parse digits after '0'.");
            }  // Get the last two digits
        #endregion

        string textToType = "";
        shouldDeleteCurrentItem = false;

        // First part of the tree
        switch (currentActionID.firstDigit)
        {
            case -1: // -1: null
                Debug.Log("null id");
                yieldBreak = true;
                break;

            case 0: // 0: not doing anything
                Debug.Log("no action called");
                yieldBreak = true;
                break;

            case 1: // 1: equipping something
                Debug.Log("equipping something");
                textToType = HandleEquipActions(currentActionID.secondDigit, out yieldBreak, out unable);
                break;

            case 2: // 2: using something
                Debug.Log("using something");
                textToType = HandleUseActions(currentActionID.secondDigit, out yieldBreak, out unable);
                break;

            case 3: // 3: discarding something
                Debug.Log("discarding something");
                textToType = HandleDiscardActions(currentActionID.secondDigit, out yieldBreak, out unable);
                break;

            default:
                Debug.Log("something went wrong with id system: first digit");
                yieldBreak = true;
                break;
        }

        // If not yield break yet then keep going
        if (!yieldBreak)
        {
            //textToType = " " + justItemText(currentActionID.thirdDigits, textToType) + ".";
            textToType = HandleItemType(currentActionID.thirdDigits, textToType);
        }

        // End of item stuff
        return textToType;
    }

    private string HandleEquipActions(int secondDigit, out bool yieldBreak, out bool unable)
    {
        yieldBreak = false;
        unable = false;
        string textToType = "";

        switch (secondDigit)
        {
            case 0: // 0: unable to perform action 
                textToType = "unable to equip item.";
                unable = true;
                yieldBreak = true;
                break;

            case 1: // 1: able to perform action 
                textToType = "Equipped";

                //changing id from equippable to unequippable
                equipIDLogic(true);
                Debug.Log("equipped??");

                //invScreenScript.currentlySelectedItem.itemObject.Equipped();
                
                break;

            case 2: // 2: able to perform reverse of action, e.g. unequipping 
                textToType = "Unequipped";

                equipIDLogic(false);
                Debug.Log("UNequipped??");

                //invScreenScript.currentlySelectedItem.itemObject.Unequipped();

                break;

            default:
                Debug.Log("something went wrong with id system: second digit");
                yieldBreak = true;
                break;
        }

        return textToType;
    }
        //changing ITEM id from equippable to unequippable, and vice versa, NOT ACTION ID
        private void equipIDLogic(bool equipping) {
            //I want this to, based on if the player is equipping or unequipping an item, change the ITEM ID from 13011 to 23011
            
            if (GlobalData.Instance.ItemID == -1) GlobalData.Instance.ItemID = 13011; //default will be the rune ig lol

            string itemidString = GlobalData.Instance.ItemID.ToString(); //this is getting the action id I need the actual id like the item one
            char[] modifiedItemID = itemidString.ToCharArray();

            Debug.Log("string builder is " + modifiedItemID + ", and item to string is " + itemidString + "id is " + GlobalData.Instance.ItemID);

                for (int i = 0; i < modifiedItemID.Length; i++) {
                    if (modifiedItemID[i] == '0') break;
                    else if (modifiedItemID[i] == '1' && equipping) {
                        modifiedItemID[i] = '2'; //equipping
                        break;
                    } 
                    else if (modifiedItemID[i] == '2' && !equipping) {
                        modifiedItemID[i] = '1'; //unequipping
                        break;
                    } 
                }

                int newID;
                if (int.TryParse(new string(modifiedItemID), out newID)) {
                    invScreenScript.changeIDinUIItemIndex(invScreenScript.currentlySelectedItem, newID);
                    Debug.Log("item id changed to " + newID );
                } else {
                    Debug.LogError("Failed to parse modified item ID.");
                }
        
        }

    private string HandleUseActions(int secondDigit, out bool yieldBreak, out bool unable)
    {
        yieldBreak = false;
        unable = false;
        string textToType = "";

        switch (secondDigit)
        {
            case 0: // 0: unable to perform action 
            case 2: // 2: fallback to 0
                textToType = "unable to use item.";
                unable = true;
                yieldBreak = true;
                break;

            case 1: // 1: able to perform action 
                textToType = "Used";
                break;

            default:
                Debug.Log("something went wrong with id system: second digit");
                yieldBreak = true;
                break;
        }

        return textToType;
    }

    private string HandleDiscardActions(int secondDigit, out bool yieldBreak, out bool unable)
    {
        yieldBreak = false;
        unable = false;
        string textToType = "";

        switch (secondDigit)
        {
            case 0: // 0: unable to perform action 
            case 2: // 2: fallback to 0
                textToType = "unable to discard item.";
                unable = true;
                yieldBreak = true;
                break;

            case 1: // 1: able to perform action 
                textToType = "Discarded";
                break;

            default:
                Debug.Log("something went wrong with id system: second digit");
                yieldBreak = true;
                break;
        }

        return textToType;
    }

    //also handles what the items do at various points
    private string HandleItemType(int thirdDigits, string textToType)
    {
        switch (thirdDigits)
        {
            case 0:
                Debug.Log("unknown item");
                break;
            case 1:
                Debug.Log("pipe");
                textToType += " pipe.";

                //does the actual action
                /*
                    1: equipping something
                    2: using something
                    3: discarding something
                */
                /*
                    1: able to perform action
                    2: able to perform reverse of action, e.g. unequipping
                */
                usePipe();

                break;
            case 2:
                Debug.Log("knife");
                textToType += " knife.";

                discardKnife();
                break;
            case 3:
                Debug.Log("meat lump");
                textToType += " meat lump.";

                useMeat();
                break;
            case 4:
                Debug.Log("puppet");
                textToType += " puppet.";

                discardPuppet();
                break;
            case 5:
                Debug.Log("tire");
                textToType += " tire.";

                discardTire();
                break;
            case 6:
                Debug.Log("hat");
                textToType += " hat.";

                useHat();
                break;
            case 7:
                Debug.Log("trinket");
                textToType += " trinket.";

                useTrinket();
                break;
            case 8:
                Debug.Log("egg");
                textToType += " egg.";

                useEgg();
                break;
            case 9:
                Debug.Log("mask");
                textToType += " mask.";

                useMask();
                break;
            case 10:
                Debug.Log("timepiece");
                textToType += " timepiece.";

                useTimepiece();
                break;
            case 11:
            case 12:
            case 13:
                Debug.Log("rune");
                textToType += " rune.";

                useRune(thirdDigits);
                break;
            default:
                Debug.Log("Invalid last two digits.");
                break;
        }

        return textToType;
    }

    #region just show text for what item it is
    private string justItemText(int thirdDigits, string textToType)
    {
        switch (thirdDigits)
        {
            case 0:
                break;
            case 1:
                textToType += "pipe";
                break;
            case 2:
                textToType += "knife";
                break;
            case 3:
                textToType += "meat lump";
                break;
            case 4:
                textToType += "puppet";
                break;
            case 5:
                textToType += "tire";
                break;
            case 6:
                textToType += "hat";
                break;
            case 7:
                textToType += "trinket";
                break;
            case 8:
                textToType += "egg";
                break;
            case 9:
                textToType += "mask";
                break;
            case 10:
                textToType += "timepiece";
                break;
            case 11:
            case 12:
            case 13:
            case 14:
                textToType += "rune";
                break;
            default:
                Debug.Log("Invalid last two digits.");
                break;
        }

        return textToType;
    }
    #endregion

    #region helper methods: action and item correct bool, not in range ienumerator
    private bool ActionAndItemCorrect() {
        return (currentActionID.firstDigit != -1 && currentActionID.thirdDigits != -1);
    }

    private IEnumerator notInRange(){
        yield return new WaitForSeconds(2f);
        ynBox.PromptJustText("notInRange");
    }

    #endregion

    private void usePipe() {
        if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    GlobalData.Instance.equippedItemEffect = (currentActionID.secondDigit == 1 ? "hatch" : "none");
                    break;
                case 2:
                    if (GlobalData.Instance.inHatch) {
                        //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                        GlobalData.Instance.equippedItemEffect = (currentActionID.secondDigit == 1 ? "hatch" : "none");
                        hatch.InteractYes();
                        invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    } else {
                        StartCoroutine(notInRange()); //not in range of hatch
                    }
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //end of if statement
    }

    private void discardKnife() {
        if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    Debug.LogError("unable to equip");
                    break;
                case 2:
                    Debug.LogError("unable to use");
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //end of if statement
    }

    private void useMeat() {
         if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    Debug.LogError("unable to equip");
                    break;
                case 2:
                    pHealth.takeDamage(-50);
                    invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //end of if statement
    }

    private void discardPuppet() {
        if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    Debug.LogError("unable to equip");
                    break;
                case 2:
                    Debug.LogError("unable to use");
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //endof if
    }

    private void discardTire() {
        if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    Debug.LogError("unable to equip");
                    break;
                case 2:
                    Debug.LogError("unable to use");
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //endof if
    }

    private void useHat() {
        if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    GlobalData.Instance.equippedItemEffect = (currentActionID.secondDigit == 1 ? "hat" : "none");
                    break;
                case 2:
                    Debug.LogError("unable to use");
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //end of if statement
    }

    private void useTrinket() {
         if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    GlobalData.Instance.equippedItemEffect = (currentActionID.secondDigit == 1 ? "trinket" : "none");
                    break;
                case 2:
                    Debug.LogError("unable to use");
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //end of if statement
    }

    private void useEgg() {
         if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    GlobalData.Instance.equippedItemEffect = (currentActionID.secondDigit == 1 ? "egg" : "none");
                    break;
                case 2:
                    Debug.LogError("unable to use");
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //end of if statement
    }

    private void useMask() {
         if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    GlobalData.Instance.equippedItemEffect = (currentActionID.secondDigit == 1 ? "mask" : "none");
                    break;
                case 2:
                    Debug.LogError("unable to use");
                    break;
                case 3: 
                    Debug.LogError("unable to discard");
                    break;
            }
        } //end of if statement
    }

    private void useTimepiece() {
         if (ActionAndItemCorrect()) { //if action id exists
            switch (currentActionID.firstDigit) {
                case 1: //equipping pipe lets you do a hatch scratch for free, unequip removes effect
                    Debug.LogError("unable to equip");
                    break;
                case 2:
                    if (birdNest.inNest) {
                        //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                        GlobalData.Instance.equippedItemEffect = (currentActionID.secondDigit == 1 ? "timepiece" : "none");
                        birdNest.InteractYes();
                        invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    } else {
                        StartCoroutine(notInRange()); //not in range of hatch
                    }
                    break;
                case 3: 
                    //ynBox.PromptJustText("areYouSure"); //addconfirmation to actions?
                    if (currentlySelectedItemListID != -1) invScreenScript.RemoveItemFromUI(currentlySelectedItemListID);
                    else Debug.LogError("item id in action id system is -1");
                    break;
            }
        } //end of if statement
    }

    private void useRune(int runeNumber) {
        string whichRune = "";
        switch (runeNumber) {
            case 11:
                whichRune = "broken voice rune";
                break;
            case 12:
                whichRune = "death rune";
                break;
            case 13:
                whichRune = "eye rune";
                break;
            case 14:
                whichRune = "fixed voice rune";
                break;
            default:
                Debug.LogError("somehow rune stuff broke");
                break;
        }

        Debug.Log(whichRune);
    }
}
