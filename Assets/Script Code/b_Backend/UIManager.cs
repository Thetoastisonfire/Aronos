using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header ("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;
    // Start is called before the first frame update

    [Header ("Inventory Menu")]
    public inventoryScreenScript invScreen;

    public void Awake() {
        gameOverScreen.SetActive(false);
    }

#region GameOver
    public void GameOver(string which){
        gameOverScreen.SetActive(true);
        GlobalData.Instance.doingSomething = true; //disable inventory button
        StartCoroutine(SoundManager.Instance.PlayAudioClip("gameOverSound", false)); //game over sound, is NOT dialogue so false
         Debug.Log("soundPlayed");
        switch(which){
            case "hunger":
                Debug.Log("hungy end");
                break;
            case "friend":
                Debug.Log("friend end");
                break;
            default:
                break;
        }
    }

    public void buttonRestart() {
        StartCoroutine(SoundManager.Instance.PlayAudioClip("placeholder", false)); //not dialogue so false
         Debug.Log("soundPlayed");
        SceneManager.LoadScene("MainMenu");
    }

    public void buttonMenu() {
        StartCoroutine(SoundManager.Instance.PlayAudioClip("placeholder", false)); //not dialogue so false
         Debug.Log("soundPlayed");
        SceneManager.LoadScene("MainMenu");
    }
#endregion


#region InventoryMenu
    public void PauseGame(){
        Debug.Log("pause game call ");
    
         if (invScreen.canvasGroup.alpha != 0) { //if inventory is active
            StartCoroutine(SoundManager.Instance.PlayAudioClip("placeholder", false)); //not dialogue so false
             Debug.Log("soundPlayed");

            // inventory screen is active, so disable it
            invScreen.SetOpacity(0f);
            GlobalData.Instance.currentlyInventory = false;
            Debug.Log("disabled");
        } else {
            StartCoroutine(SoundManager.Instance.PlayAudioClip("placeholder", false)); //not dialogue so false 
             Debug.Log("soundPlayed");

            // Inventory screen is not active, so enable it
            invScreen.SetOpacity(1f);
            GlobalData.Instance.currentlyInventory = false;
            Debug.Log("enabled");
        }
    }





#endregion


}
