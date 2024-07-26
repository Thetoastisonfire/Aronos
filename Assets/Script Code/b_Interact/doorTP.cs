using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class doorTP : MonoBehaviour, Interactible
{
    public playerHealth player;
    public PlayerMovement pMove;
    public GameObject playerAgain;
    public GameObject otherDoor;
    public SpriteRenderer playerSprite;
    public yesNoBox yesNoBoxScript;
    [SerializeField] private bool heavenDoor;
    [SerializeField] private GameObject heaven;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer playerSprite = playerAgain.GetComponent<SpriteRenderer>();
        if (heaven != null) heaven.SetActive(false);
    }

    public void Interact(){
        if (otherDoor.activeSelf) doorUpdate();
        else yesNoBoxScript.PromptYesNoBox(pMove.transform.position, "doorGone");
    }

    void doorUpdate(){
        StartCoroutine(doorCutscene());
    }

    IEnumerator doorCutscene()
    {   
         StartCoroutine(SoundManager.Instance.PlayAudioClip("portal", false));
          Debug.Log("soundPlayed");

        SetOpacity(0f);
        if (heavenDoor) heaven.SetActive(true);
        yield return new WaitForSeconds(1f);
        teleport(otherDoor.transform.position);
        SetOpacity(1f);

         StartCoroutine(SoundManager.Instance.PlayAudioClip("blink", false));
          Debug.Log("soundPlayed");
    }

    public void teleport(Vector3 newPosition) {
        playerAgain.transform.position = newPosition;
    }


    public void SetOpacity(float targetOpacity) // Adds duration parameter for smooth fade
    {
        float duration = 0.5f;
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

        if (heavenDoor && targetOpacity == 1f) this.gameObject.SetActive(false);
    }

}
