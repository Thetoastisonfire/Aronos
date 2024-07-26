using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class SignScript : MonoBehaviour, Interactible // Assuming IInteractible is the correct interface
{
    #region Variables

    [Header("Sign Objects")]
    [SerializeField] private GameObject signScreen1;
    [SerializeField] private GameObject signScreen2;
    [SerializeField] private TMP_Text textComponent;
    private CanvasGroup signScreen1CanvasGroup;
    private CanvasGroup signScreen2CanvasGroup;
    private Coroutine typeLineCoroutine;

    [Header("Variables")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float textSpeed = 0.5f;
    private bool isFading = false;
    [SerializeField] private int whichSign = 1; //1 is the 1st sign, 2 is the second

    [Header("Strings")]
    public string which = "null";
    [SerializeField] private string signText1 = "(The marks form words in your mind)";
                                                /*"The air here feels timeless. What's left" +
                                                " of my hands lay raw and bleeding, yet I" +
                                                " bear witness to my own incessant, relentless" +
                                                " digging to the upper floor. Each scratch on" +
                                                " the walls seemed to vanish into the darkness,";*/
    [SerializeField] private string signText2 = "The doors scream with me as I work. It only needs three now, I know it." +
                                                "I need to get out. I need to break them. I need the hatch. " +
                                                "I need to dig. It took my arms but I can't stop. I won't. " +
                                                "I won't let them see. I need to get out. I need to get out. Please";
                                                /*"swallowed by doorways that consumed the dirt" +
                                                " and stone. The stairway I carved towards the" +
                                                " hole they cast us into collapsed the day my" +
                                                " arms ceased to move. But then, I found it..";*/
    [SerializeField] private string signText3 = "A hatch, barely holding as their light seeps" +
                                                " through the cracks. I reached for it but," +
                                                " alas, my strength was gone. Now, I can only" +
                                                " hope the hooded figure will show mercy as he" +
                                                " did for the others, one last time. Perhaps" +
                                                " it will be whoever is next..?";

    private string[] lines;
    private int index = 0;

    #endregion

    #region Initialization and Interaction

    void Start()
    {
        textComponent.text = "";

        signScreen1CanvasGroup = signScreen1.GetComponent<CanvasGroup>();
        if (signScreen1CanvasGroup == null)
        {
            signScreen1CanvasGroup = signScreen1.AddComponent<CanvasGroup>();
        }

        signScreen2CanvasGroup = signScreen2.GetComponent<CanvasGroup>();
        if (signScreen2CanvasGroup == null)
        {
            signScreen2CanvasGroup = signScreen2.AddComponent<CanvasGroup>();
        }

        signScreen1CanvasGroup.alpha = 0;
        signScreen2CanvasGroup.alpha = 0;

    }

    public void Interact() {

        Debug.Log("interact thing");

        if (which == "null") //if theres no sign currently showing, only show the image
        {
            StartCoroutine(SoundManager.Instance.PlayAudioClip("placeholder", false));
            lines = new string[] { "" };
            PromptSign("null");
        }
        else if (whichSign == 1) //1st sign's text
        { 
            lines = new string[] { signText1, signText2 };
            PromptSign("sign 1");
        }
        else if (whichSign == 2) //2nd sign's text
        {
            lines = new string[] { signText3 };
            PromptSign("sign 2");
        }
    }

    public void PromptSign(string str) {
         Debug.Log("sign prompted");
        if (isFading) return;

        GlobalData.Instance.currentlyInteracting = true;
        GlobalData.Instance.doingSomething = true;

        if (str == "null") StartCoroutine(FadeInSign(signScreen1CanvasGroup, signScreen1)); //if only image variation is needed
        else StartCoroutine(FadeInSign(signScreen2CanvasGroup, signScreen2)); //if sign text is needed

        PromptText();

    }

    public void PromptText(){
        if (textComponent == null)
        {
            Debug.LogError("textComponent is not assigned.");
            return;
        }

        textComponent.text = string.Empty;

        StartCoroutine(SoundManager.Instance.PlayAudioClip("dialogueWhoosh", false));

        index = 0;
        typeLineCoroutine = StartCoroutine(TypeLine());
    }

    #endregion

    #region Line Manipulation

    public void Update() {
        if (signScreen1CanvasGroup != null && signScreen1CanvasGroup.alpha != 0 || 
            signScreen2CanvasGroup != null && signScreen2CanvasGroup.alpha != 0)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0) || 
                (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began))
            {
                StartCoroutine(SoundManager.Instance.PlayAudioClip("smallWhoosh", false));

                if (lines != null && index >= 0 && index < lines.Length)
                {
                    if (textComponent.text == lines[index])
                    {
                        NextLine();
                    }
                    else
                    {
                       if (typeLineCoroutine != null) //only stopping typeline so player doesn't get softlocked from fade out coroutine stopping
                        {
                            StopCoroutine(typeLineCoroutine);
                            typeLineCoroutine = null;
                        }
                        textComponent.text = lines[index];
                    }
                }
            }
        }
        
    }

    public void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            SoundManager.Instance.StopDialogueClip();
            typeLineCoroutine = StartCoroutine(TypeLine());
        }
        else //no more lines
        {
            if (which != "null") OnNoButton();
            else {
                which = "not null";
                Interact();
            }
        }
    }

    IEnumerator TypeLine()
    {

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            Debug.Log("typing " + c);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    #endregion

    #region Fade Code

    IEnumerator FadeInSign(CanvasGroup canvasGroup, GameObject signScreen)
    {
         Debug.Log("sign fading in");
        isFading = true;
        float elapsedTime = 0f;

        signScreen.SetActive(true);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
         Debug.Log("sign faded in");

        isFading = false;
    }

    public void OnNoButton()
    {
        if (isFading) return;

        GlobalData.Instance.currentlyInteracting = false;
        GlobalData.Instance.doingSomething = false;
        which = "null";

        if (signScreen1.activeSelf) StartCoroutine(FadeOutSign(signScreen1CanvasGroup, signScreen1));
        if (signScreen2.activeSelf) StartCoroutine(FadeOutSign(signScreen2CanvasGroup, signScreen2));
    }

    IEnumerator FadeOutSign(CanvasGroup canvasGroup, GameObject signScreen)
    {
        isFading = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        signScreen.SetActive(false);
        GlobalData.Instance.doingSomething = false;
        isFading = false;
    }

    #endregion
}
