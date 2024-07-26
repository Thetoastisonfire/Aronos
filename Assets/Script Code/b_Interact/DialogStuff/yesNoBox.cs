using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; 

public class yesNoBox : MonoBehaviour
{
    #region variables and imports
     [Header ("Camera")]
    private Camera _mainCamera;

     [Header ("YesNoBox Objects")]
    [SerializeField] private CanvasGroup yesNoPanel;  // CanvasGroup of the Yes/No panel
    [SerializeField] private GameObject yn;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;

     [Header ("Misc. Objects")]
    [SerializeField] private PlayerMovement pMove;    // Reference to the PlayerMovement script
    [SerializeField] private junkInteract junk;       // Reference to the junkInteract script
    [SerializeField] private hatchScript hatch;
    [SerializeField] private BirdNest birdNest;
    [SerializeField] private CreamyCrateTime crate;
    [SerializeField] private MenuButton inventoryDisable;
    [SerializeField] private npcGuyInteract npc;
    [SerializeField] private UIManager uiMan;
    [SerializeField] private inventoryScreenScript invScreen;

     [Header ("Variables")]
    [SerializeField] private static float fadeDuration = 1f; // Duration of the fade effect
    private Vector3 initialPosition; // Initial position where the Yes/No box was triggered
    [SerializeField] private static float maxDistance = 3f;  // Maximum allowed distance before automatically selecting "No"
    [SerializeField] private static float textSpeed = 0.05f;
    [SerializeField] private static float waitTime = 1f;

    private bool isFading = false;   // Flag to check if fading is in progress

     [Header ("Strings")]
    public string which = "null";
    [SerializeField] private static string junkText = "Search the pile of junk?";
    [SerializeField] private static string hatchText = "The hatch remains out of reach. Dig a furrow into the wall?";
    [SerializeField] private static string nestText = "Rest in the strange bird nest? The assortment of sticks give pleasantly under your feet.";
    [SerializeField] private static string hideText = "Hide in the crate?";
    [SerializeField] private static string crateFailText = "...What are you doing..?";
    [SerializeField] private static string doorGoneText = "The doorway does not respond.";
    [SerializeField] private static string tiredText = "You're too tired to do anything else..";
    private string textToType;
    [SerializeField] private string[] lines;

    #endregion

    #region init and prompts

    void Start()
    {
        _mainCamera = Camera.main;
        yesNoPanel.alpha = 0f;  // Ensure the panel is initially invisible
    }

    public void PromptYesNoBox(Vector3 triggerPosition, string str) {

        if (isFading) {
            StopAllCoroutines();
            FadeOutImmediate(); // Prevent multiple calls while fading
        }

        initialPosition = triggerPosition; // Set the initial position
        which = str;                       // Set which object is being interacted with
        if (which == "junk" || which == "hatch") GlobalData.Instance.currentlyInteracting = false;

        GlobalData.Instance.doingSomething = true; //disable inventory button
        yn.SetActive(true);                // Ensure the panel is active
        StartCoroutine(FadeInYesNoBox());

        //prompt reset code
        yesButton.SetActive(true);
        noButton.SetActive(true);
        
        promptText();
    }

    public void PromptJustText(string str) {
        if (isFading) return; // Prevent multiple calls while fading

        which = str;                       // Set which object is being interacted with
        yn.SetActive(true);                // Ensure the panel is active
        StartCoroutine(FadeInYesNoBox());
        promptText();
    }

    #endregion

    #region text and text logic

    public void promptText() {
        //text reset
        textComponent.text = string.Empty;


        StartCoroutine(SoundManager.Instance.PlayAudioClip("dialogueWhoosh", false)); //not dialogue so false
         Debug.Log("soundPlayed");

        textToTypeLogic(); //line to type; also does case-specific stuff
    }

    private void textToTypeLogic() {
        textToType = ""; //text that goes on to the player
        bool needsChoiceBoxes = true; //first switch is for things that need choice boxes

     #region text that needs choice boxes
        switch (which) {

            case "junk": //can't if tired
                if (GlobalData.Instance.tiredCounter > GlobalData.Instance.minimumTired) {
                    StartCoroutine(typingWithoutChoiceBoxes(tiredText));
                } 
                else textToType = junkText;
                break;

            case "hatch": //is more readable when tired code is duplicated
                //change hatch text based on whether its ready or not
                if (GlobalData.Instance.hatchCounter >= 3) {
                    hatchText = "..Ascend?";
                }
                else if (GlobalData.Instance.tiredCounter > GlobalData.Instance.minimumTired) {
                    StartCoroutine(typingWithoutChoiceBoxes(tiredText));
                } 
                else textToType = hatchText;
                break;

            case "nest":
                textToType = nestText;
                break;

            case "crate":
                textToType = hideText;
                break;
            /* //add confirmation boxes to inventory actions?
            case "areYouSure":
                textToType = "Are you sure you want to ";
                switch (actionIDSystem.Instance.currentActionID.firstDigit){
                    case 1:
                        textToType += "equip ";
                        break;
                    case 2:
                        textToType += "use ";
                        break;
                    case 3:
                        textToType += "discard ";
                        break;
                }
                string itemName = "";
                textToType += actionIDSystem.Instance.justItemText(actionIDSystem.Instance.currentActionID.thirdDigits, itemName) 
                                + "?";
                break; */


            default:
                //keeps going to next bit
                needsChoiceBoxes = false;
                break;
        }
        if (needsChoiceBoxes) {
            StartCoroutine(typingInGeneral(textToType));
            return;
        }
     #endregion

     #region text that doesn't have choice boxes
        //for cases without choice boxes
        actionIDSystem.Instance.yieldBreak = false;
        switch (which) {
            
            case "junkInteract": //dialog for interacting with crate
                GlobalData.Instance.currentlyInteracting = true;
                textToType = junkDialogue(junk.currentItem);  //junk
                break;
            case "crateFail": //if there's no reason to hide
                textToType = crateFailText; //crate
                break;
            case "doorGone":
                textToType = doorGoneText; //door portal
                break;
            case "pipeUse":
                textToType = "Used Pipe."; //door portal
                break;
            case "notInRange":
                textToType = "Not in range.";
                break;



            //inventory things
            case "itemStuff":
                //figure out what to do based on action ID anditem ID system
                int currentItemID = GlobalData.Instance.ActionID;

                textToType = actionIDSystem.Instance.ReturnString(currentItemID);
                break;

            default:
                return; // Exit if no valid option is provided
        }
     #endregion

        //types everything after building the string
        StartCoroutine(typingWithoutChoiceBoxes(textToType));

    } //end of type line
    
    #endregion

    #region typing
     //needs choice boxes
    public IEnumerator typingInGeneral(string textToType) {
        foreach (char c in textToType.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
     //no choice boxes
    public IEnumerator typingWithoutChoiceBoxes(string textToType) {
        turnOffChoiceBoxes();
        if (which == "itemStuff" && !actionIDSystem.Instance.unable) {
             Debug.Log("back pressed");
            // actionIDSystem.ActionIDInput(GlobalData.Instance.ActionID);
                GlobalData.Instance.doingSomething = false;
                invScreen.SetOpacity(0f);
        } else actionIDSystem.Instance.unable = false; 

        foreach (char c in textToType.ToCharArray()) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        GlobalData.Instance.currentlyInteracting = false;
        yield return new WaitForSeconds(waitTime);
        if (yesNoPanel.alpha != 0) StartCoroutine(FadeOutYesNoBox());
    }

    public void turnOffChoiceBoxes() {
        yesButton.SetActive(false);
        noButton.SetActive(false);
    }

    #endregion

    #region update and fade in
    void Update(){
        if (pMove.transform.position.x > initialPosition.x + maxDistance || 
            pMove.transform.position.x < initialPosition.x - maxDistance) OnNoButton(); //should automatically say no on leaving area
    }

   //fades in alpha value
   IEnumerator FadeInYesNoBox()
    {   
        isFading = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yesNoPanel.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        isFading = false;
    }
    
    #endregion

    #region yes no and fade out

     public void OnYesButton()
    {
    //    Debug.Log("yes button clicked");
        if (isFading) return; // Prevent interaction while fading
        //GlobalData.Instance.tiredCounter += 1; //if yes, get tired

        // Different yes responses handled here
        switch (which) {
            case "junk":  
                GlobalData.Instance.tiredCounter += 1; //if yes, get tired
                junk.InteractYes();
                break;
            case "hatch": 
                GlobalData.Instance.tiredCounter += 2; //if yes, get tired
                hatch.InteractYes();
                break;
            case "nest": 
                birdNest.InteractYes();
                GlobalData.Instance.tiredCounter = 0; //nest make no more tire :3
                break;
            case "crate": 
                crate.InteractYes();
                GlobalData.Instance.tiredCounter -= 1; //crate no make tire :3
                break;
            /* //add confirmations for inventory actions?
            case "areYouSure":
                Debug.Log("performed inventory item action.");
                actionIDSystem.Instance.performAction();
                break;*/
            default:
                break;
        }
        StartCoroutine(FadeOutYesNoBox());
    }

     public void OnNoButton()
    {
        if (isFading) return; // Prevent interaction while fading

        GlobalData.Instance.currentlyInteracting = false;
        GlobalData.Instance.doingSomething = false;
        if (yesNoPanel.alpha != 0) StartCoroutine(FadeOutYesNoBox());
    }

     IEnumerator FadeOutYesNoBox()
    {
        isFading = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yesNoPanel.alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        yn.SetActive(false); // Deactivate the panel after fading out
        GlobalData.Instance.doingSomething = false; //reactivate inventory button
        isFading = false;
        npc.healthWarning();
    }

    public void FadeOutImmediate() {
        yesNoPanel.alpha = 0f;
        yn.SetActive(false); // Deactivate the panel after fading out
        GlobalData.Instance.doingSomething = false; //reactivate inventory button
        isFading = false;
    }

    #endregion

    #region junk interaction stuff

     public string junkDialogue(string item) {
    //Debug.Log("sets up junk");
        // Create a list to store the lines
        List<string> lineList = new List<string>();

        // Add your predefined dialogue part and append the passed string
        if (item == "nothing") lineList.Add("You find " + item);
        else if (item == "old tire" || item == "old cloth puppet") lineList.Add("You find an " + item);
        else lineList.Add("You find a " + item);

        // Convert the list to an array
        lines = lineList.ToArray();
        return lines[0];
    }

    public void junkStart(){
        PromptYesNoBox(pMove.transform.position, "junkInteract");
    }

    #endregion
}
