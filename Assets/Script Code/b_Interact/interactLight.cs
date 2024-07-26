using System.Collections;
using UnityEngine;

public class InteractLight : MonoBehaviour
{
    public GameObject targetObject;
    public float fadeDuration = 1f;
    private Coroutine fadeCoroutine = null;
    private Color originalColor;
    private bool canFade = false;
    [SerializeField] private float alphaThingie = 0.3f;

    void Start()
    {
        if (targetObject == null)
        {
            targetObject = this.gameObject;
        }

        originalColor = targetObject.GetComponent<SpriteRenderer>().color;
        targetObject.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        StartCoroutine(DelayedFadeEnable());
    }

    IEnumerator DelayedFadeEnable()
    {
        yield return new WaitForSeconds(1f);//5f); //change for debugging?
        canFade = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      //  Debug.Log("interact enter");

        if (other.CompareTag("Interactible") && canFade)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (gameObject.activeInHierarchy)
            {
                fadeCoroutine = StartCoroutine(Fade(true));
            }
            else
            {
                Debug.LogWarning("Coroutine couldn't be started because the game object 'InteractLight' is inactive!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
      //  Debug.Log("interact exit");

        if (other.CompareTag("Interactible") && canFade)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (gameObject.activeInHierarchy)
            {
                fadeCoroutine = StartCoroutine(Fade(false));
            }
            else
            {
                Debug.LogWarning("Coroutine couldn't be started because the game object 'InteractLight' is inactive!");
            }
        }
    }

    public void turnOffInteractLight() {
        if (canFade) {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (gameObject.activeInHierarchy)
            {
                fadeCoroutine = StartCoroutine(Fade(false));
            }
            else
            {
                Debug.LogWarning("Coroutine couldn't be started because the game object 'InteractLight' is inactive!");
            }
        }
    }

    IEnumerator Fade(bool inOrOut)
    {
        float elapsedTime = 0f;
        float targetAlpha = inOrOut ? alphaThingie : 0f;
        Color startColor = targetObject.GetComponent<SpriteRenderer>().color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startColor.a, targetAlpha, elapsedTime / fadeDuration);
            targetObject.GetComponent<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        targetObject.GetComponent<SpriteRenderer>().color = targetColor;
    }
}