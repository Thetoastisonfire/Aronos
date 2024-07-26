using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro; 

public class DialogScript : MonoBehaviour
{
    // Start is called before the first frame update


#region variables
     [Header ("String Data")]
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string[] lines = {""};
    [SerializeField] private float textSpeed;
    private int index = 0; //lines index
    private int whichDialogue = 0; /*dialogue index, should work by getting the index of the 
                                    current dialogue, then the index of the current line 
                                    in the dialogue array   */
     [Header ("Misc. Objects")]
    private Camera _mainCamera;
    public Image image;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject fadeInObj;
    [SerializeField] private GameObject gameobject;
    [SerializeField] private junkInteract junk;
    [SerializeField] private PlayerMovement pMove;
    [SerializeField] private npcGuyInteract npcGuy;
    [SerializeField] private choicePanelBox choicePanel;
    [SerializeField] private DaysChange daysChange;

     [Header ("Variables")]
    public string dialogueTag; //verrrry important for switching between interactions
    public bool isDialogueActive = false;
    private bool latch = false; //for dialogue fade out
    private bool meatLatch = true; //for starting meat cutscene
    private float originalBackgroundMusicVolume = 0.0f;
    private float originalAmbienceVolume = 0.0f;
    [SerializeField] private float volumeDown = 0.3f;


    private Vector3 initialPosition; // Initial position where the Yes/No box was triggered
    [SerializeField] private float maxDistance = 3f;  // Maximum allowed distance before automatically selecting "No"

#endregion

#region init
    void Awake()
    {
        _mainCamera = Camera.main;
        SetOpacity(0f); //hide on start
    }

    void Start()
    {
     if (dialogueTag != null && dialogueTag == "intro")
        {
            subsequentStart();
        }
    }

   public void subsequentStart() {
        initialPosition = pMove.transform.position;
        textComponent.text = string.Empty;
        GlobalData.Instance.doingSomething = true; // disable inventory button

        switch (dialogueTag) {
            case "intro":
                introDialogue();
                break;

            case "npcGuy":
                if (npcGuy.currentItem.Count > 0) {
                    npcDialogue(npcGuy.currentItem.Pop());
                }
                else
                {
                    Debug.Log("dialog failed");
                }
                break;

            case "npcGuyMeatNo":
                lines = new string[] { 
                    "Sorry, but you're too full of yummyness. :(",
                    "come back when your widdle bewwy is fuww"
                    };
                break;

            case "npcGuyMeatAngry":
                lines = new string[] { 
                    "I'm angry, no meat! >:(" 
                    };
                break;

            case "npcGuyMeatYes":
                choicePanel.npcAnimator.SetTrigger("opening");
                StartCoroutine(choicePanel.LoopSecondAnimation());

                if (npcGuy.currentItem.Count > 0) {
                    npcDialogue(npcGuy.currentItem.Pop());
                }
                else
                {
                    Debug.Log("dialog failed");
                }
                //meat cutscene is in this one
                break;

            default:
                break;
        } //end of switch
        startDialogue();
    }

#endregion


#region writing text
    // Update is called once per frame; CLICKING will call nextLine
    void Update() {
        if (image.color.a != 0) {
            if (dialogueTag == "npcGuyMeatYes" && index == 3-1) { //meat cutscene time //its 3 but arrays are weird
               //make it so if theres a click it shows 'skip cutscene' and then if you click again it skips
               if(meatLatch) {
                daysChange.messageText.text = " ";
                daysChange.meatCutsceneStart();
                meatLatch = false;
               }
            } 
            //everything else
            else if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)) {
                    meatLatch = true; //just to reset cutscene latch

                    StartCoroutine(SoundManager.Instance.PlayAudioClip("smallWhoosh", false)); //small whoosh
                    if (index >= 0 && index < lines.Length) {
                        if (textComponent.text == lines[index]) {
                            nextLine();
                        } else {
                            StopAllCoroutines();
                            textComponent.text = lines[index];
                        }
                    } //end of fix for annoying out of bounds error
                }

                //turn off dialog script if leave
            if (pMove.transform.position.x > initialPosition.x + maxDistance || 
                pMove.transform.position.x < initialPosition.x - maxDistance) {
                    turnOffDialog(); //should automatically say no on leaving area
            }
        } //end of ifs
    }

    void startDialogue() {
         //turn down other sources for dialogue
        originalBackgroundMusicVolume = SoundManager.Instance.backgroundMusicSource.volume;
        originalAmbienceVolume = SoundManager.Instance.backgroundAmbientSource.volume;
        SoundManager.Instance.backgroundMusicSource.volume = originalBackgroundMusicVolume * volumeDown; // Example: reduce to 50%
        SoundManager.Instance.backgroundAmbientSource.volume = originalAmbienceVolume * volumeDown;

        StartCoroutine(SoundManager.Instance.PlayAudioClip("dialogueWhoosh", false)); //not dialogue so false

        SetOpacity(1f);//dialogueBox.SetActive(true);
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine(){ //types each char one by one, plays dialogue

        StartCoroutine(SoundManager.Instance.PlayAudioClip($"{whichDialogue},{index+1}", true)); //start dialogue for line

        if (lines.Length > index) {
            foreach (char c in lines[index].ToCharArray()) {
                    textComponent.text += c;
                    yield return new WaitForSeconds(textSpeed);
            }
        } else {
            turnOffDialog();
        }
    }

    public void nextLine(){
        if (index < lines.Length - 1){ //normal dialog workings
            index++;
            latch = true;
            textComponent.text = string.Empty;
            SoundManager.Instance.StopDialogueClip();
            StartCoroutine(TypeLine());


            /*if (dialogueTag == "npcGuyMeatYes" && index == 3 ) { //meat cutscene
                index--; //idk what this'll do
            } */

        } else { //no lines left
            //Debug.Log("no lines left");
            turnOffDialog();
            if (dialogueTag == "intro") fadeInObj.SetActive(true); //change to fade out to new scene
        }
    }

    private void turnOffDialog() {
        if (latch) {
                
                //revert changes to audio
                SoundManager.Instance.StopDialogueClip();
                SoundManager.Instance.backgroundMusicSource.volume = originalBackgroundMusicVolume;
                SoundManager.Instance.backgroundAmbientSource.volume = originalAmbienceVolume;

                SetOpacity(0f);//dialogueBox.SetActive(false);
                latch = false;

            } 
            isDialogueActive = false;
            GlobalData.Instance.currentlyInteracting = false;
            GlobalData.Instance.doingSomething = false;
            SetOpacity(0f);
    }

    public void OnClick(InputAction.CallbackContext context){
        if (!context.started) return;
        
        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        if(textComponent.text == lines[index]) {
            nextLine();
        } else {
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }

#endregion

#region DIALOGUE SECTION
//just changes current dialogue; should be for NPC and INTRO but thats IT
   public void introDialogue() {
    //Debug.Log("sets up intro");
    if (!isDialogueActive)
        {
            isDialogueActive = true;
            lines = new string[] {
                "The winding path begins anew.",
                "You will be judged.",
                "Now awake. Broken. Useless."
            };
        }
    }

    public void npcDialogue((int indexer, string[] item) npcTuple) {
         if (!isDialogueActive) {
            isDialogueActive = true;

            // Create a list to store the lines
            List<string> lineList = new List<string>();

            // Add your predefined dialogue part and append the passed string
            if (npcTuple.item == null || npcTuple.item.Length == 0 || (npcTuple.item.Length == 1 && npcTuple.item[0] == "")) {
                lineList.Add("...");// if empty string or empty array
            }

             foreach (string s in npcTuple.item) lineList.Add(s); //loop to just add everything straight up

            // Convert the list to an array
            lines = lineList.ToArray();

            //set the dialogue index
            whichDialogue = npcTuple.indexer;
        }
    }

 #endregion

#region opacityCode
//opacity change code
   public void SetOpacity(float targetOpacity) // Adds duration parameter for smooth fade
    {
        float duration = 0.25f;
        StartCoroutine(FadeTo(targetOpacity, duration));
    }

    private IEnumerator FadeTo(float targetOpacity, float duration)
    {
        Color imageColor = image.color;
        Color textMeshProColor = textMeshPro.color;
        float startImageOpacity = imageColor.a;
        float startTextMeshProOpacity = textMeshProColor.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startImageOpacity, targetOpacity, elapsed / duration);
            imageColor.a = newOpacity;
            textMeshProColor.a = newOpacity;
            image.color = imageColor;
            textMeshPro.color = textMeshProColor;
            yield return null;
        }

        // Ensure the final opacity is set correctly
        imageColor.a = targetOpacity;
        textMeshProColor.a = targetOpacity;
        image.color = imageColor;
        textMeshPro.color = textMeshProColor;
    }

 #endregion
}