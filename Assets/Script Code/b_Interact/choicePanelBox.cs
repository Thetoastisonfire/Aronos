using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; 

public class choicePanelBox: MonoBehaviour
{
     [Header ("Camera")]
    private Camera _mainCamera;

     [Header ("Choice Panel Objects")]
    [SerializeField] private CanvasGroup choicePanel;  // CanvasGroup of the choice panel
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private GameObject talkButton;
    [SerializeField] private GameObject noButton;
    [SerializeField] private GameObject meatButton;

     [Header ("Misc. Objects")]
    [SerializeField] private npcGuyInteract npcScript;    // Reference to the PlayerMovement script
    [SerializeField] private MenuButton inventoryDisable;
    [SerializeField] private PlayerMovement pMove;
    public Animator npcAnimator;


     [Header ("Variables")]
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade effect
    private Vector3 initialPosition; // Initial position where the Yes/No box was triggered
    [SerializeField] private float maxDistance = 3f;  // Maximum allowed distance before automatically selecting "No"
    private bool isFading = false;   // Flag to check if fading is in progress
    //private bool meatLatch = true;

    void Start()
    {
        Debug.Log("starts good");
        _mainCamera = Camera.main;
        choicePanel.alpha = 0f;  // Ensure the panel is initially invisible
    //    meatButton.SetActive(false);//off until main dialogue
    }

    void Update(){
        if (pMove.transform.position.x > initialPosition.x + maxDistance || 
            pMove.transform.position.x < initialPosition.x - maxDistance) OnNoButton(); //should automatically say no on leaving area

    }

    public void PromptChoiceBox(Vector3 triggerPosition) {

      /*  if (meatLatch && (GlobalData.Instance.npcCounter >= 2 || npcScript.warningGiven)) {
            meatLatch = false;
            meatButton.SetActive(true);
        } */

        Debug.Log("box prompted");
        if (isFading) return; // Prevent multiple calls while fading

        initialPosition = triggerPosition; // Set the initial position

        GlobalData.Instance.doingSomething = true; //disable inventory button
        choiceBox.SetActive(true);                // Ensure the panel is active
        meatButton.SetActive(true);
        StartCoroutine(FadeInChoiceBox());
    }


#region buttonStuff

     public void OnTalkButton()
    {
    //    Debug.Log("yes button clicked");
        if (isFading) return; // Prevent interaction while fading
        npcScript.InteractYes();
        Debug.Log("talk button");

        StartCoroutine(FadeOutChoiceBox());
    }

     public void OnNoButton()
    {
        if (isFading) return; // Prevent interaction while fading

        GlobalData.Instance.currentlyInteracting = false;
        if (choicePanel.alpha != 0) StartCoroutine(FadeOutChoiceBox());
        Debug.Log("talk button");
    }


    public void OnMeatButton() {
        if (isFading) return; // Prevent interaction while fading
        npcScript.InteractMeat();
        StartCoroutine(FadeOutChoiceBox());
        Debug.Log("talk button");


     //  npcAnimator.SetTrigger("opening");
        // Set up a coroutine to loop the second animation after the first completes
    //    StartCoroutine(LoopSecondAnimation());
        /*
        needs to:
            - only turn on after the main dialogue has been completed
            - lead to different voice lines, just needs a 'of course child' and a 'you could not stomach it' or smth
            - gives diff responses depending on current hunger level

        */
    }

    public IEnumerator LoopSecondAnimation()
    {
        // Wait for the duration of the first animation
        yield return new WaitForSeconds(npcAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Set the second animation to loop
        npcAnimator.SetBool("openLoop", true);

        // Trigger the second animation
        npcAnimator.SetTrigger("openLoop");
    }

#endregion

#region fadeStuff
     //fades in alpha value
   IEnumerator FadeInChoiceBox()
    {
        isFading = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            choicePanel.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        isFading = false;
    }

     IEnumerator FadeOutChoiceBox() {
        isFading = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            choicePanel.alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        choiceBox.SetActive(false); // Deactivate the panel after fading out
        isFading = false;
    }

#endregion

}
