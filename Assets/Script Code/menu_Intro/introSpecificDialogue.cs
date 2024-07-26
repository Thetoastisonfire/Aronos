using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro; 

public class introSpecificDialogue : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed;
    private int index; //lines index


    private Camera _mainCamera;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject fadeInObj;

    void Awake()
    {
        _mainCamera = Camera.main;
        SetOpacity(0f);
    }

    void Start()
    {
      //  Debug.Log("intro hit..??");
        textComponent.text = string.Empty;
        introDialogue();
        startDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (image.color.a != 0) {
            if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)){
                if(textComponent.text == lines[index]) {
                    nextLine();
                } else {
                    StopAllCoroutines();
                    textComponent.text = lines[index];
                }

            }
        }
    }

    void startDialogue(){
        SetOpacity(255f);//dialogueBox.SetActive(true);
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine() {
        int charCount = 0;

        // Generate a random starting point for the sound effect
        int trueRand = Random.Range(1, 6);

        // Type each character one by one
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            charCount++;

            // Play sound effect on every 3rd character
            if (charCount % 4 == 0) {
                StartCoroutine(PlayRandomAngelSound(trueRand));

                // Loop through sounds 1 to 5
                trueRand = (trueRand % 5) + 1;
            }

            yield return new WaitForSeconds(textSpeed);
        }
    }

    // Method to play a random angel sound
    private IEnumerator PlayRandomAngelSound(int soundIndex) {
        string clipName = $"angel{soundIndex}";
        yield return SoundManager.Instance.PlayStuntedClip(clipName);
    }


    void nextLine(){
        if (index < lines.Length - 1){
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }

        else { //no lines left
            //Debug.Log("no lines left");
            SetOpacity(0f);//dialogueBox.SetActive(false);
            fadeInObj.SetActive(true); //change to fade out to new scene
        }
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

        //Debug.Log("click");
    }


///////////----DIALOGUE SECTION------/////////////



//just changes current dialogue
   public void introDialogue() {
        lines = new string[] {
            "Disgusting creature.",
            "Judgement has been passed.",
            "To return is to die.",
            "Now awake. Broken. Useless."
        };
    }






//opacity change code
    public void SetOpacity(float opacity) //basically turns the dialogue box on and off
    {
        // Get the current color of the image
        Color color = image.color;
        Color textMeshProColor = textMeshPro.color;
        // Set the opacity value
        color.a = opacity;
        textMeshProColor.a = opacity;

        // Update the color of the image
        image.color = color;
        textMeshPro.color = textMeshProColor;

    }
}
