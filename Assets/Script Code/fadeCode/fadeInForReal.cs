using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInForReal : MonoBehaviour
{
    public Texture2D fadeTexture;  // Texture for fade effect
    public float fadeSpeed = 0.2f; // Speed of the fade effect
    public string nextSceneName = "BasementScene";  // Name of the next scene to load
    public int drawDepth = -1000;  // Depth of the fade texture

    private float alpha = 1.0f;    // Current alpha value of the fade effect
    private int fadeDir = -1;      // Direction of the fade effect (-1 for fade out)
    private bool isFading = false; 

    public void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }
    
    private void Update()
    {
        if (isFading && alpha <= 0.0f)
        {
            isFading = false;  // Stop the fade effect
           // SceneManager.LoadScene(nextSceneName);
        }
    }

    public void Awake(){
        //public FadeInForReal fadeScript;
        // Trigger fade out
        StartFade();//fadeScript.StartFade();
      //  Debug.Log("start func");
        
        // Trigger fade in after some delay
      //  StartCoroutine(TriggerFadeIn());
    }

    public IEnumerator TriggerFadeIn()
    {
        yield return new WaitForSeconds(5.0f);
    //    Debug.Log("fadeTrigger");
        StartFadeIn();//fadeScript.StartFadeIn();
    }

    public void StartFade()
    {
        alpha = 1.0f;       // Reset alpha value
        fadeDir = -1;       // Set fade direction to fade to 0.0
        isFading = true;    // Activate fade effect
    //    Debug.Log("start fade called");
    }

    public void StartFadeIn()
    {
        alpha = 0.0f;       // Reset alpha value
        fadeDir = 1;        // Set fade direction to fade to 1.0
        isFading = true;    // Activate fade effect
     //   Debug.Log("start fade INnNNNN called");
    }
}
