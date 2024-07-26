using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fadeOut : MonoBehaviour
{
    public Texture2D fadeTexture;  // Texture for fade effect
    public float fadeSpeed = 0.2f; // Speed of the fade effect
    public string nextSceneName = "BasementScene";  // Name of the next scene to load
    public int drawDepth = -1000;  // Depth of the fade texture
    public GameObject dayChange;

    private float alpha = 0.0f;    // Current alpha value of the fade effect
    private int fadeDir = 1;      // Direction of the fade effect (-1 for fade out)

    void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }

    void Update() {
        if (alpha >= 1.0f) dayChange.SetActive(true); //used to be loading the next scene, now if screen is fully black, do this
    }
}
