using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class softDeathScript : MonoBehaviour
{   

#region variables
     [Header ("UI Assets")]
    [SerializeField] private Image fadeImage;             //reference to black image for the fade effect
  
     [Header ("Misc Objects")]
    [SerializeField] private GameObject birdNest; //just to know where it is
    [SerializeField] private UIManager gameOver;
    [SerializeField] private playerHealth pHealth;
    [SerializeField] private PlayerMovement pMove;

     [Header ("String Data")]
    [SerializeField] private float fadeSpeed = 10f;      // Speed of the fade effect
    [SerializeField] private int kindOfDeath; //death by what


    private float alpha = 0.0f;         // Current alpha value of the fade effect
    private int fadeDir = 1;            // Direction of the fade effect (1 for fade to black, -1 for fade to transparent)
    private bool isFading = false; 
    private float originalBackgroundMusicVolume = 0.0f;
    private float originalAmbienceVolume = 0.0f;


    public void Awake() {
        gameOver = FindObjectOfType<UIManager>();
    }

#endregion

#region start and update

    public void subsequentStart() {
        
        //lock player
        pMove.enabled = false;

        // Start cutscene
        softDeathCutsceneStart();
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
                StartCoroutine(playSoftDeathSound());
            } else if (fadeDir == -1 && alpha > 0.0f){ //if the soft death is enough to kill them
                         if (GlobalData.Instance.longSleep == 0) { //hungy die
                            gameOver.GameOver("hunger");
                        } else if (GlobalData.Instance.longSleep == 1) { //frend die
                            gameOver.GameOver("friend");
                        } else {// not die, do other stuff
                        }
            }
            else if (fadeDir == -1 && alpha <= 0.0f) // Fade to transparent complete
            {
            GlobalData.Instance.currentlyInteracting = false; //to freeze player
            isFading = false;
            
/////add something here where the player is laying down in the nest and gets up            
            }
        }
    }

#endregion

#region fade stuff

    private IEnumerator playSoftDeathSound()
    {
        //different sounds during blackout 
        switch (kindOfDeath){ //fall death, killed death (friendship), hunger death, reset death?
            case 0: //fall death
                StartCoroutine(SoundManager.Instance.PlayAudioClip("junk3", false));
                break;
            case 1: //friend death
                StartCoroutine(SoundManager.Instance.PlayAudioClip("junk3", false));
                break;
            case 2: //hunger death
                StartCoroutine(SoundManager.Instance.PlayAudioClip("junk3", false));
                break;
            case 3: //reset death
                StartCoroutine(SoundManager.Instance.PlayAudioClip("junk3", false));
                break;
            default: //other?
                StartCoroutine(SoundManager.Instance.PlayAudioClip("junk3", false));
                break;
        }
    
        yield return new WaitForSeconds(3.0f); // Display the message for 3 seconds
        StartFadeToTransparent();
    }

    private void StartFadeToBlack()
    {
        alpha = 0.0f;       // Ensure alpha starts from 0
        fadeDir = 1;        // Set fade direction to fade to black (alpha = 1)
        isFading = true;    // Activate fade effect
    }

    private void StartFadeToTransparent()
    {
        alpha = 1.0f;       // Ensure alpha starts from 1
        fadeDir = -1;       // Set fade direction to fade to transparent (alpha = 0)
        isFading = true;    // Activate fade effect
    }

#endregion

#region cutscene
    private void softDeathCutsceneStart() {
            originalBackgroundMusicVolume = SoundManager.Instance.backgroundMusicSource.volume;
            originalAmbienceVolume = SoundManager.Instance.backgroundAmbientSource.volume;
            SoundManager.Instance.backgroundMusicSource.volume = originalBackgroundMusicVolume * 0f; //reduce to 0%
            SoundManager.Instance.backgroundAmbientSource.volume = originalAmbienceVolume * 0f;

            StartCoroutine(softDeathCutscene());
    }
    private IEnumerator softDeathCutscene() {
        // Start fade to black
        StartFadeToBlack();
        yield return new WaitForSeconds(3.0f);

                //this part is what to do during the black screen
        //change stats
        pHealth.takeDamage(GlobalData.Instance.currentHunger/10); //hunger down
        pHealth.takeEmotionalDamage(GlobalData.Instance.maxFriendship/10); //friendship down

         // Teleport player to above the bird nest
        Vector3 birdNestPosition = birdNest.transform.position;
        pMove.transform.position = new Vector3(birdNestPosition.x, birdNestPosition.y + 1, birdNestPosition.z);
        pMove.transform.rotation = Quaternion.Euler(pMove.transform.rotation.x, pMove.transform.rotation.y, 90); //rotate to be sleeping
                //this part is what to do during the black screen

        // Start fade to transparent
        StartFadeToTransparent();
        SoundManager.Instance.backgroundMusicSource.volume = originalBackgroundMusicVolume; //back to 100%
        SoundManager.Instance.backgroundAmbientSource.volume = originalAmbienceVolume;
        yield return new WaitForSeconds(3.0f);
        // Unlock player movement
        pMove.enabled = true;
    }

#endregion

}
