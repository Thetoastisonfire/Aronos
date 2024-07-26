using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour //its called menu button but its sort of just all the buttons now
{
    [Header ("Button Data")]
    [SerializeField] private Button button;
    public string action;

     [Header ("Misc Objects")]
    [SerializeField] private UIManager uiMan;
    [SerializeField] private yesNoBox ynBox;
    [SerializeField] private GameObject miscObject;
    [SerializeField] private GameObject miscObject2;
    [SerializeField] private SoundManager soundManager;

    [Header ("item data")]
    [SerializeField] private UIinventoryDescription itemDesc;
    [SerializeField] private inventoryScreenScript invScreen; //its in canvas -> inventory screen -> inventory panel -> inventory list area
    private bool calculating = false;


    private bool isCooldown = false;

    void OnEnable()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        // Remove any existing listeners to prevent duplicates
        button.onClick.RemoveListener(OnButtonClick);
        
        // Add the listener
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }


    public void OnButtonClick() {
        Debug.Log($"Button Clicked! Action: {action}");
        if (isCooldown)
        {
            Debug.Log("Button is on cooldown.");
            return;
        }

        if (invScreen.currentlySelectedItem != null) {
                            invScreen.currentlySelectedItem.Deselect(); //de-selects whatever item was selected
                            itemDesc.ResetDesc(); //resets item description stuff
        }

        switch (action)
        {
            //boot to main menu
            case "MainMenu":
                SceneManager.LoadScene("MenuScene");
                break;

            //turn on/off inventory
            case "Inventory":
                if (!GlobalData.Instance.doingSomething) {

                    GlobalData.Instance.doingSomething = true;
                    if (invScreen.canvasGroup.alpha == 0) { //!uiMan.invScreen.activeSelf) { //if invscreen NOT active
                        if (invScreen.currentlySelectedItem != null) {
                            invScreen.currentlySelectedItem.Deselect(); //de-selects whatever item was selected
                            itemDesc.ResetDesc(); //resets item description stuff
                        }

                        invScreen.SetOpacity(1f); //turn on inventory
                        invScreen.turnOffEquipPanel(); //in case its still on
                    }
                    else invScreen.SetOpacity(0f); //so they don't get stuck in inventory no matter what!
                }
                break;
            
    #region stat panel buttons
            //stat panel buttons
            case "HungerDescription":
                invScreen.turnOffEquipPanel(); //in case its still on
                itemDesc.SetDesc(GlobalData.Instance.spriteBank[2], "Current Hunger", "The more effort one exerts, the weaker they become. Death occurs when it falls to 0.");
                break;
            case "FriendDescription":
                invScreen.turnOffEquipPanel(); //in case its still on
                itemDesc.SetDesc(GlobalData.Instance.spriteBank[15], "Current Affinity", "The higher their regard, the more the hooded ones bestow. To reach 0 would be suicide.");
                break;
            case "RuneDescription":
                invScreen.turnOffEquipPanel(); //in case its still on
                itemDesc.SetDesc(GlobalData.Instance.spriteBank[12], "Runes Found", "Runes found throughout Above and Below. Some may be of significant help.");
                break;
            case "StatusDescription":
                invScreen.turnOffEquipPanel(); //in case its still on
                itemDesc.SetDesc(GlobalData.Instance.spriteBank[4], "Current Status", "To be exiled is to be forgotten. The hatch can be dug freely.");
                break;
    #endregion


    #region settings buttons
            //boot to basement
            case "Restart":
                SceneManager.LoadScene("BasementScene");
                break;

            //turn off inventory
            case "Back":
                GlobalData.Instance.doingSomething = false;
                invScreen.SetOpacity(0f);
                break;

            //settings
            case "Settings":
                 Debug.Log("settings");
                break;

            //audio is settings..?
            case "Audio":
                soundManager.SetGlobalVolume(1f);
                miscObject.SetActive(true);
                 Debug.Log("audio");
                break;
    #endregion

    #region item setting thingie buttons
       
            case "Equip":
                if (calculating) {
                    Debug.Log("calculating EQUIP");
                    break;
                }
                ActionIDCalcLogic(1);
                ynBox.PromptJustText("itemStuff");
                break;
            case "Unequip":
                if (calculating) {
                    Debug.Log("calculating UNEQUIP");
                    break;
                }
                ActionIDCalcLogic(2);
                ynBox.PromptJustText("itemStuff");
                break;
            case "Use":
                if (calculating) {
                    Debug.Log("calculating");
                    break;
                }
                ActionIDCalcLogic(3);
                ynBox.PromptJustText("itemStuff");
                break;
            case "Discard":
                if (calculating) {
                    Debug.Log("calculating");
                    break;
                }
                ActionIDCalcLogic(4);
                ynBox.PromptJustText("itemStuff");//switch(GlobalData.Instance.ActionID) 
                break;

            case "notEnabled": //for inventory buttons
                StartCoroutine(SoundManager.Instance.PlayAudioClip("zoomInBoom", false));
                break;
/*
            case "Equip":
                Debug.Log("equipping");
                //show textbox 'equipped blank'
                //show its equipped somehow
                //keep showing item description
                //if already equipped, unequip and change text to match that also
                //sound effects for equip n unequip
                break;
            case "Use":
                //if usable, use
                //show textbox for used blank
                //sound effects
                Debug.Log("Using");
                break;
            case "Discard":
                //if discardable, discard
                //show textbox for discarded blank
                //if junk item, put somewhere in room?
                //important items will stay in nest?
                Debug.Log("Discarding");
                break;
*/
    #endregion

    #region sign buttons
            //for signs
            case "Sign2":
                if (!GlobalData.Instance.doingSomething) {
                    action = "Sign1";
                    miscObject2.SetActive(false);
                    miscObject.SetActive(false);
                }
                break;

            case "Sign1":
                if (!GlobalData.Instance.doingSomething) {
                    miscObject2.SetActive(true);
                    action = "Sign2";
                }
                break;
    #endregion

            //unknown button
            default:
                 Debug.LogWarning("Unknown action: " + action);
                break;
        }

        //so you can't spam buttons
        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine() {
        isCooldown = true;
        yield return new WaitForSeconds(0.2f); // 1 second cooldown
        isCooldown = false;
    }

   
    public void ActionIDCalcLogic(int whatAction) { //calculate what the action id should be based off of the item

        calculating = true;

        if (GlobalData.Instance.ItemID == -1) {
            Debug.LogError("ItemId instance not correct");
            GlobalData.Instance.ActionID = -1; //error return
            calculating = false;
            return;
        }
        int ItemID = 0;//GlobalData.Instance.ItemID;
        int lastDigits = -1;  // Get the last two digits

        //action being taken
        switch(whatAction) {
            case 1: //"equip"
            case 2: //"unequip"
                ItemID += 1000; //first digit
                break;
            case 3: //"use"
                ItemID += 2000; //first digit
                break;
            case 4: //"discard"
                ItemID += 3000; //first digit
                break;
            default:
                Debug.LogError("Unknown action");
                GlobalData.Instance.ActionID = -1; // Error return
                calculating = false;
                return;
        }

        //whether the action is able to be performed
        string itemString = GlobalData.Instance.ItemID.ToString();
        Debug.Log( "item id is " + GlobalData.Instance.ItemID);
        for (int i = 0; i <= itemString.Length; i++) {
            if (i >= itemString.Length) {
                Debug.Log("i was " + i + ", length was " + itemString.Length);
                calculating = false;
                return; //in case of overflow?
            }
            switch (itemString[i]) {
                case '1': //able to equip
                    if (whatAction == 1) ItemID += 100; //equipping id
                    break;
                case '2': //able to unequip
                    if (whatAction == 2) ItemID += 200; //unequipping id
                    break;
                case '3': //able to use
                    if (whatAction == 3) ItemID += 100; //using id
                    break;
                case '4': //able to discard
                    if (whatAction == 4) ItemID += 100; //discarding id
                    break;
                case '0': //end of possible actions
                    bool success = int.TryParse(itemString.Substring(i), out lastDigits);
                    if (!success) Debug.Log("Conversion failed in menuButton.ActionIDCalcLogic.");
                    i += itemString.Length;
                    Debug.Log("hit zero in action parsing");
                    break;
                default:
                    break;
            }
        }
        //-------------making the action id from the item id and the desired actions------

        string itemIDStr = ItemID.ToString();
        string lastDigitsStr = lastDigits.ToString();

        // Combine the strings
        string combinedStr = itemIDStr + lastDigitsStr;

        // Ensure there is exactly one '0' between them
        if (!combinedStr.Contains("0"))
        {
            combinedStr = itemIDStr + "0" + lastDigitsStr;
        }
        else
        {
            // If there are multiple '0's, reduce them to one
            combinedStr = combinedStr.Replace("00", "0");
            // Ensure there is only one '0' between the segments
            int zeroIndex = combinedStr.IndexOf("0");
            combinedStr = combinedStr.Substring(0, zeroIndex) + "0" + combinedStr.Substring(zeroIndex).Replace("0", "");
        }

        GlobalData.Instance.ActionID = int.Parse(combinedStr); //should look like xx0xx
        actionIDSystem.Instance.currentActionIDForReal = int.Parse(combinedStr);

        Debug.Log("action id calc logic action id " + GlobalData.Instance.ActionID);


        calculating = false;
    }

}