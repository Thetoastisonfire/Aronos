using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DaysChange : MonoBehaviour
{   

#region variables
     [Header ("UI Assets")]
    [SerializeField] private Image fadeImage;             // Reference to the UI Image for the fade effect
    public TextMeshProUGUI messageText; // Reference to the UI Text for the message
    [SerializeField] private TextMeshProUGUI dayCountText; // Reference to the UI Text for another message

     [Header ("Misc Objects")]
    [SerializeField] private DialogScript dialogueScript;
    [SerializeField] private BirdNest birdNest;
    [SerializeField] private UIManager gameOver;
    [SerializeField] private yesNoBox yesNoBoxScript;
    [SerializeField] private GameObject yesNoBox;
    [SerializeField] private playerHealth pHealth;

     [Header ("String Data")]
    [SerializeField] private float fadeSpeed = 10f;      // Speed of the fade effect
    [SerializeField] private string timeMessage = "...Time Passes..."; // Message to display
    [SerializeField] private string sleepMessage = "HONNNKKK schmeememememe";
    [SerializeField] private string dayMessage = "Day ";

     [Header ("Inventory")]
    [SerializeField] private inventoryScreenScript invScreenScript;  //its in canvas -> inventory screen -> inventory panel -> inventory list area

    private float alpha = 0.0f;         // Current alpha value of the fade effect
    private int fadeDir = 1;            // Direction of the fade effect (1 for fade to black, -1 for fade to transparent)
    private bool isFading = false; 
    private bool latch = false;         // Latch to trigger subsequentStart
    private float originalBackgroundMusicVolume = 0.0f;
    private float originalAmbienceVolume = 0.0f;


    public void Awake() {
        gameOver = FindObjectOfType<UIManager>();
    }

#endregion

#region start and update

    public void subsequentStart() {
        if (birdNest.inNest) {
            messageText.text = sleepMessage;
        }
        else messageText.text = timeMessage;

        messageText.enabled = false; // Initially hide the message text
        dayCountText.enabled = false; // Initially hide the message text
        fadeImage.raycastTarget = false;

        StartCoroutine(FadeSequence());
    }

    private void Update() {
        if (isFading) {
            GlobalData.Instance.currentlyInteracting = true; //to freeze player at start of any fade
            GlobalData.Instance.doingSomething = true;

            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            Color newColor = fadeImage.color;
            newColor.a = alpha;
            fadeImage.color = newColor;

            if (fadeDir == 1 && alpha >= 1.0f) // Fade to black complete
            {
                isFading = false;
                StartCoroutine(ShowMessage());
            } else if (fadeDir == -1 && alpha > 0.0f){
                    if (birdNest.inNest) { //sleep interaction stuff
                         if (GlobalData.Instance.longSleep == 0) { //hungy die
                            gameOver.GameOver("hunger");
                        } else if (GlobalData.Instance.longSleep == 1) { //frend die
                            gameOver.GameOver("friend");
                        } else {// not die, show day
                        }
                    }
            }
            else if (fadeDir == -1 && alpha <= 0.0f) // Fade to transparent complete
            {

                GlobalData.Instance.currentlyInteracting = false; //to freeze player
                isFading = false;
                messageText.enabled = false; // Hide the message text
                if (latch)
                {


    //interactions after fades                
                    if (yesNoBoxScript.which == "junkInteract") { //only show this on end of interaction
                        yesNoBox.SetActive(true);
                        yesNoBoxScript.junkStart();
                    } else if (birdNest.inNest) { //sleep interaction stuff
                         if (GlobalData.Instance.longSleep == 0) { //hungy die
                        } else if (GlobalData.Instance.longSleep == 1) { //frend die
                        } else {// not die, show day
                            dayCountText.text = dayMessage + GlobalData.Instance.dayCount;
                            Debug.Log("day count text start");
                            StartCoroutine(dayCountFader());
                        }

                    }
                    latch = false; // Reset the latch
                }
            }
        }
    }

#endregion

#region fade stuff
    private IEnumerator FadeSequence()
    {
        // Start fade to black
        StartFadeToBlack();
        yield return new WaitForSeconds(1.0f);

        // Show message and keep screen black for a while
       // yield return new WaitForSeconds(3.0f);

        // Start fade to transparent
        StartFadeToTransparent();
        yield return new WaitForSeconds(3.0f);
    }

    private IEnumerator ShowMessage()
    {
        messageText.enabled = true;

        if (dialogueScript.dialogueTag == "npcGuyMeatYes") yesNoBoxScript.which = "meat"; 
                //just making sure so it doesn't play random sound effects for meat cutscene

        //different sounds during blackout 
        if (yesNoBoxScript.which == "junkInteract") { //only show this on end of interaction
            StartCoroutine(SoundManager.Instance.PlayAudioClip("junk3", false)); //not dialogue so false
        } else if (yesNoBoxScript.which == "hatch") { //only show this on end of interaction
            StartCoroutine(SoundManager.Instance.PlayAudioClip("hatchDig", false)); //not dialogue so false
        } else if (birdNest.inNest) StartCoroutine(SoundManager.Instance.PlayAudioClip("zoomInBoom", false)); //not dialogue so false


        yield return new WaitForSeconds(3.0f); // Display the message for 3 seconds
        StartFadeToTransparent();
    }

    public void StartFadeToBlack()
    {
        alpha = 0.0f;       // Ensure alpha starts from 0
        fadeDir = 1;        // Set fade direction to fade to black (alpha = 1)
        isFading = true;    // Activate fade effect
        latch = true;       // Set the latch
    }

    public void StartFadeToTransparent()
    {
        alpha = 1.0f;       // Ensure alpha starts from 1
        fadeDir = -1;       // Set fade direction to fade to transparent (alpha = 0)
        isFading = true;    // Activate fade effect
    }

#endregion

#region cutscenes
//text fade stuff
    private IEnumerator dayCountFader() {
        Debug.Log("dayCountFader");
        dayCountText.enabled = true;
        dayCountText.color = new Color(dayCountText.color.r, dayCountText.color.g, dayCountText.color.b, 0); //should set alpha to 0

        yield return StartCoroutine(SetOpacity(1f)); //setting opacity of day count tmpro
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(SetOpacity(0f)); //setting opacity of day count tmpro
        dayCountText.enabled = false;
    }

    public void meatCutsceneStart() {
       //     GlobalData.Instance.currentlyInteracting = true; //to freeze player
            messageText.text = "  ";

            originalBackgroundMusicVolume = SoundManager.Instance.backgroundMusicSource.volume;
            originalAmbienceVolume = SoundManager.Instance.backgroundAmbientSource.volume;
            SoundManager.Instance.backgroundMusicSource.volume = originalBackgroundMusicVolume * 0f; //reduce to 0%
            SoundManager.Instance.backgroundAmbientSource.volume = originalAmbienceVolume * 0f;

            StartCoroutine(meatCutscene());
    }
    private IEnumerator meatCutscene() {
        // Start fade to black
        StartFadeToBlack();
        yield return new WaitForSeconds(3.0f);

        
        //change stats
        //pHealth.takeDamage(-GlobalData.Instance.maxHunger); //hunger back to full
        invScreenScript.overwriteItem(GlobalData.Instance.spriteBank[3 /* meat */ - 1], 1, "Misshapen Meat", 
                        "Gain hunger", 34);
        pHealth.takeEmotionalDamage(GlobalData.Instance.maxFriendship/2); //friendship is halved

        // Start fade to transparent
        StartFadeToTransparent();
        SoundManager.Instance.backgroundMusicSource.volume = originalBackgroundMusicVolume; //back to 100%
        SoundManager.Instance.backgroundAmbientSource.volume = originalAmbienceVolume;
        dialogueScript.nextLine();
        yield return new WaitForSeconds(3.0f);
    }

     private IEnumerator SetOpacity(float targetOpacity) // Adds duration parameter for smooth fade
    {
        Debug.Log("setting opacity");
        float duration = 1f;
        yield return StartCoroutine(FadeTo(dayCountText, targetOpacity, duration));
    }

    private IEnumerator FadeTo(TextMeshProUGUI textRenderer, float targetOpacity, float duration)
    {
        Debug.Log("fading now");
        Color color = textRenderer.color;
        float startOpacity = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startOpacity, targetOpacity, elapsed / duration);
            textRenderer.color = color;
            yield return null;
        }

        // Ensure the final opacity is set correctly
        color.a = targetOpacity;
        textRenderer.color = color;
    }
#endregion

}
