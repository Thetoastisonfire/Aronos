using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreamyCrateTime : MonoBehaviour, Interactible
{
    public yesNoBox yesNoBoxScript;
    public GameObject yesNoBox;
    public CanvasGroup yesNoPanel;  // CanvasGroup of the Yes/No panel
    public playerHealth player;
    public PlayerMovement pMove;
    public GameObject playerAgain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Interact(){
        //Debug.Log("working");
         if (yesNoPanel.alpha == 0) {
            yesNoBox.SetActive(true);
            yesNoBoxScript.PromptYesNoBox(pMove.transform.position, "crate");
         }
    }

    public void InteractYes() {
        //Debug.Log("working");
        GlobalData.Instance.hideCounter += 1;
        player.takeDamage(2); 
        GlobalData.Instance.currentlyInteracting = true;
        nestUpdate();
    }


    void nestUpdate(){
        Debug.Log("hide in crate");
        StartCoroutine(nestWait());
    }

    IEnumerator nestWait()
    {   
        StartCoroutine(SoundManager.Instance.PlayAudioClip("placeholder", false));
         Debug.Log("soundPlayed");

        SetOpacity(0f);
        yield return new WaitForSeconds(3f);
        if (yesNoPanel.alpha == 0) {
            yesNoBox.SetActive(true);
            yesNoBoxScript.PromptYesNoBox(pMove.transform.position, "crateFail");
         }
        SetOpacity(1f);
         StartCoroutine(SoundManager.Instance.PlayAudioClip("placeholder", false));
          Debug.Log("soundPlayed");
    }

    public void SetOpacity(float targetOpacity) // Adds duration parameter for smooth fade
    {
        float duration = 1.0f;
        SpriteRenderer playerSprite = playerAgain.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeTo(playerSprite, targetOpacity, duration));
    }

    private IEnumerator FadeTo(SpriteRenderer spriteRenderer, float targetOpacity, float duration)
    {
        Color color = spriteRenderer.color;
        float startOpacity = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startOpacity, targetOpacity, elapsed / duration);
            spriteRenderer.color = color;
            yield return null;
        }

        // Ensure the final opacity is set correctly
        color.a = targetOpacity;
        spriteRenderer.color = color;
    }

}
