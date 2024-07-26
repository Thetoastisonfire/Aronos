using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; 

public class yesNoBoxHeaven : MonoBehaviour
{
#region variables and stuff
     [Header ("Camera")]
    private Camera _mainCamera;

     [Header ("YesNoBox Objects")]
    [SerializeField] private CanvasGroup yesNoPanel;  // CanvasGroup of the Yes/No panel
    [SerializeField] private GameObject yn;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;

     [Header ("Misc. Objects")]
    [SerializeField] private PlayerMovementHeaven pMove;    // Reference to the PlayerMovementHeaven script
    [SerializeField] private CreamyCrateTime crate;
    [SerializeField] private MenuButton inventoryDisable;
    [SerializeField] private npcGuyInteract npc;

     [Header ("Variables")]
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade effect
    private Vector3 initialPosition; // Initial position where the Yes/No box was triggered
    [SerializeField] private float maxDistance = 3f;  // Maximum allowed distance before automatically selecting "No"
    [SerializeField] private float textSpeed = 0.5f;
    private bool isFading = false;   // Flag to check if fading is in progress

     [Header ("Strings")]
    public string which = "null";
    [SerializeField] private string hideText = "Hide in the crate?";
    [SerializeField] private string crateFailText = "...What are you doing..?";
  //  [SerializeField] private string tiredText = "You're too tired to do anything else..";
    private string textToType;
    [SerializeField] private string[] lines;

#endregion

    void Start()
    {
        _mainCamera = Camera.main;
        yesNoPanel.alpha = 0f;  // Ensure the panel is initially invisible
    }

     public void PromptYesNoBox(Vector3 triggerPosition, string str)
    {
        if (isFading) return; // Prevent multiple calls while fading

        initialPosition = triggerPosition; // Set the initial position
        which = str;                       // Set which object is being interacted with
    
        GlobalData.Instance.doingSomething = true; //disable inventory button
        yn.SetActive(true);                // Ensure the panel is active
        StartCoroutine(FadeInYesNoBox());
        promptText();
    }


    public void promptText() {
        //prompt reset code
        yesButton.SetActive(true);
        noButton.SetActive(true);
        textComponent.text = string.Empty;


        StartCoroutine(SoundManager.Instance.PlayAudioClip("dialogueWhoosh", false)); //not dialogue so false
         Debug.Log("soundPlayed");

        StartCoroutine(TypeLine()); //line to type; also does case-specific stuff
    }

        IEnumerator TypeLine(){
            textToType = "";
            
            switch (which) {

                

                case "crate":
                    textToType = hideText;
                    break;
                
                case "crateFail": //if there's no reason to hide
                    yesButton.SetActive(false);
                    noButton.SetActive(false);
                    textToType = crateFailText; //crate
                    foreach (char c in textToType.ToCharArray()) {
                                textComponent.text += c;
                                yield return new WaitForSeconds(textSpeed);
                        }
                    GlobalData.Instance.currentlyInteracting = false;
                    yield return new WaitForSeconds(textSpeed*10);
                    if (yesNoPanel.alpha != 0) StartCoroutine(FadeOutYesNoBox());
                    break;

                default:
                    yield break; // Exit if no valid option is provided
            }


            if (which != "crateFail")
            foreach (char c in textToType.ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

        } //end of type line

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


     public void OnYesButton()
    {
    //    Debug.Log("yes button clicked");
        if (isFading) return; // Prevent interaction while fading
        GlobalData.Instance.tiredCounter += 1; //if yes, get tired

        // Different yes responses handled here
        switch (which) {
            case "crate": 
                crate.InteractYes();
                GlobalData.Instance.tiredCounter -= 1; //crate no make tire :3
                break;
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
        if (which != "crate") GlobalData.Instance.doingSomething = false; //reactivate inventory button
        GlobalData.Instance.doingSomething = false;
        isFading = false;
        npc.healthWarning();
    }



}
