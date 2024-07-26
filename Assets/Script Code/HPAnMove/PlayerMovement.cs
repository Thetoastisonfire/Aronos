using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
     [Header ("Attribute Data:")]
     #region NoteOnStatMaxes
        [Space]
        [Header ("Highest player speed without breaking everything")]
        [Header ("is around 80, highest jump is around 40. This is")]
        [Header ("for modifying stats on game completion. Also make")]
        [Header ("that any stat mods can be turned off and also")]
        [Header ("additive until their respective max value.")]
        [Space]

     #endregion
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 1f;
   // private bool isWallSliding = false; //use this to freeze player or smth when they hit a wall or smth idk
    private float wallSlideSpeed = 2f;

     [Header ("Collision Data")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheck;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    public bool hasHitGroundFirstTime = false;

     [Header ("Move Data")]
    public float moveInput = 0f;
    private bool isAtSideCoroutineRunning = false;
    private Coroutine fallCoroutine;

     [Header ("Misc Objects")]
    [SerializeField] private BirdNest birdNest;
    [SerializeField] private DialogScript dialogueScript;
    [SerializeField] private junkInteract junkInteract;
    private bool backgroundMusicLatch = true;

     [Header ("Anim and Audio")]
    private Animator anim;
   // private bool hasSatDown = false; //for animation
   
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>(); //rigidbody
        boxCollider = GetComponent<BoxCollider2D>(); //hitbox
        anim = GetComponent<Animator>(); //animation

        //set move stats
        speed = GlobalData.Instance.speed;
        jumpForce = GlobalData.Instance.jumpForce;
    }

    void Update() {
         //play background music once
        if (backgroundMusicLatch && SoundManager.Instance.initializationComplete) {
            StartCoroutine(SoundManager.Instance.PlayAudioClip("breakingThroughFast", false));
            StartCoroutine(waitASec(3.0f)); //wait before starting background music
            StartCoroutine(SoundManager.Instance.PlayAmbient("caveAmbient"));
             //Debug.Log("soundPlayed: start");
            backgroundMusicLatch = false; 
        }

        //default not moving
        else if (!GlobalData.Instance.currentlyInteracting || !dialogueScript.isDialogueActive) { //if interacting or talking

           //movement   //handle when player is on their side
            if (!IsOnSide()) HandleMovementInput();
            else {
                if (!isAtSideCoroutineRunning) StartCoroutine(CheckIfAtSide());//2 second time wait
            }

          //handle jumping
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) Jump();
          //handle wall sliding
            wallSlide();
          //handle animation
            anim.SetBool("run", moveInput != 0);
        //    hasSatDown = false;
        } else {
            body.velocity = new Vector2(body.velocity.x, -(moveInput * speed));

        }
        
    }

    private IEnumerator waitASec(float time) {
        yield return new WaitForSeconds(time);
        StartCoroutine(SoundManager.Instance.PlayBackgroundMusic("WhispersInTheWind2"));

    }

#region movement
    //movement input
    private void HandleMovementInput() {
        moveInput = 0f;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (!onWall() && isGrounded()) //if on ground and not on wall
        {
            moveInput = Input.GetAxis("Horizontal");
            if (fallCoroutine != null) //stop falling timer
            {
                StopCoroutine(fallCoroutine);
                fallCoroutine = null;
            }
        }
        else if (!isGrounded()) //falling check
        {
            if (fallCoroutine == null)
            {
                fallCoroutine = StartCoroutine(CheckFalling());
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (!onWall() && isGrounded())  //if on ground and not on wall
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    moveInput = touch.deltaPosition.x * Time.deltaTime;
                }
            }
            if (fallCoroutine != null) //stop falling timer
            {
                StopCoroutine(fallCoroutine);
                fallCoroutine = null;
            }
        }
        else if (!isGrounded()) //falling check
        {
            if (fallCoroutine == null)
            {
                fallCoroutine = StartCoroutine(CheckFalling());
            }
        }
#endif
        // Handle directional movement
        body.velocity = new Vector2(moveInput * speed, body.velocity.y);

        // Handle sprite's direction
        if (moveInput > 0.01f) {
            transform.localScale = new Vector3(0.25f, 0.25f, 1); // face right
      //      walkSoundTime();
        } else if (moveInput < -0.01f) {
            transform.localScale = new Vector3(-0.25f, 0.25f, 1); // face left
      //      walkSoundTime();
        } else {
         //   SoundManager.Instance.StopSEClip();
        }
    }

    private IEnumerator CheckFalling()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("falling");
        StartCoroutine(SoundManager.Instance.PlayAudioClip("falling", false));
        hasHitGroundFirstTime = false;
    }


    void Jump()
    {
         if (isGrounded()) body.velocity = new Vector2(body.velocity.x, jumpForce);
    }

    bool isGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.5f, groundLayer);
        return rayHit.collider != null;
    }

    private bool onWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void wallSlide(){
        if (onWall() && !isGrounded() && moveInput != 0f) { 
     //       isWallSliding = true;
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlideSpeed, float.MaxValue));
        } //else isWallSliding = false;
    }

#endregion


#region walk sounds
     public void walkSoundTime() {
        if (Math.Abs(moveInput) <= 0.01f)  SoundManager.Instance.StopWalkClip(); //stop walking if not moving
         else if (isGrounded()) {
            int randomIndex = UnityEngine.Random.Range(1, 4); // Changed to 4 to include the range properly

            if (!birdNest.inNest)
            {
                PlayWalkSound("walkSound", randomIndex);//PlayWalkSound("nest", randomIndex);
                 //Debug.Log("soundPlayed: Walk, Nest");
            }
            else if (junkInteract.inJunk)
            {
                PlayWalkSound("junk", randomIndex);
                 //Debug.Log("soundPlayed: Walk, Junk");
            }
            else
            {
                 PlayWalkSound("nest", randomIndex);//PlayWalkSound("walkSound", randomIndex);
            }
        }
    }

    public void PlayWalkSound(string soundBaseName, int randomIndex) {
        string clipName = $"{soundBaseName}{randomIndex}";
        StartCoroutine(SoundManager.Instance.PlayWalkingAudioClip(clipName));
         //Debug.Log("soundPlayed: Walk, Default");
    }

    private void blink() {
        StartCoroutine(SoundManager.Instance.PlayAudioClip("blink", false));
    }

    //for the thump when hit ground first time
     private void OnCollisionEnter2D(Collision2D collision) {
        // Check if the collision is with the ground
        if (!hasHitGroundFirstTime) {
            if (collision.collider.CompareTag("Ground")) {
                // Set the flag to true to indicate the player has hit the ground for the first time
                hasHitGroundFirstTime = true;

                // Play the sound using the SoundManager
                StartCoroutine(SoundManager.Instance.PlayAudioClip("thump", false));
            }
        }
    }

#endregion

#region player on side
//if player falls on their side

    private bool IsOnSide()
    {
        float zAngle = transform.rotation.eulerAngles.z;
        // Normalize the angle to the range of -180 to 180
        if (zAngle > 180)
        {
            zAngle -= 360;
        }

        // Check if the absolute value of the z-angle is greater than 30 degrees
        return Mathf.Abs(zAngle) > 30f;
    }

    private IEnumerator CheckIfAtSide()
    {
        moveInput = 0; //stop movement

        isAtSideCoroutineRunning = true;
        float timeAtSide = 0f;
        float requiredTime = 1f; // Time in seconds to stay at 90 or -90

        while (IsOnSide())
        {
            timeAtSide += Time.deltaTime;
            if (timeAtSide >= requiredTime)
            {
                onSide();
                isAtSideCoroutineRunning = false;
                yield break;
            }
            yield return null;
        }
        isAtSideCoroutineRunning = false;
    }


    private void onSide() {
        if (isGrounded() )//&& !birdNest.inNest)
        {
            // Start a coroutine to handle the smooth rotation
            StartCoroutine(RotateToStanding());
        }
    }

    private IEnumerator RotateToStanding()
    {
      //  //Debug.Log("rotating");
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 0);
        float duration = 0.5f; // Duration of the rotation
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the rotation is exactly set to the end rotation
        transform.rotation = endRotation;
    }

#endregion

}