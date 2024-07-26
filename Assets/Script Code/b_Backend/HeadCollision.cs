using System.Collections;
using UnityEngine;

public class HeadCollision : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    [SerializeField] private PlayerMovement pMove;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collision is with the ground or relevant layer
        
        if (collision.CompareTag("Ground")) {
            Debug.Log("ground collide");
            HandleCollision(0);
        }  else if (collision.CompareTag("stairs")) {
            Debug.Log("stairs collide");
            HandleCollision(1);
        }  else if (collision.CompareTag("nest")) {
            Debug.Log("nest collide");
            HandleCollision(2);
        }
    }

    private void HandleCollision(int whichCollision) //ground = 0, stairs = 1, nest = 2
    {
        float playerRotation = player.transform.eulerAngles.z;

        if (IsSlidingRotation(playerRotation))
        {
            StartCoroutine(SoundManager.Instance.PlayAudioClip("slide", false));
            Debug.Log("slide sound");
        }
        else
        {
            switch (whichCollision){
                case 0: //ground = 0
                    StartCoroutine(SoundManager.Instance.PlayAudioClip("hitSound", false)); //stairs sound
                    break;
                case 1: //stairs = 1
                    StartCoroutine(SoundManager.Instance.PlayAudioClip("hitSound", false)); //stairs sound
                    break;
                case 2: //nest = 2
                    StartCoroutine(SoundManager.Instance.PlayAudioClip("hitSound", false)); //stairs sound
                    pMove.PlayWalkSound("nest", Random.Range(1,4));
                    break;
            }
             Debug.Log("hit sound");
        }
    }

    private bool IsSlidingRotation(float rotation) {
        // Normalize the rotation to be within [0, 360)
        rotation = rotation % 360;

        // Define the tolerance for checking the angle
        float tolerance = 2.0f;

        // Check if the rotation is within the tolerance range of 90, 180, or 270 degrees
        return Mathf.Abs(rotation - 90) <= tolerance || 
            Mathf.Abs(rotation - 180) <= tolerance || 
            Mathf.Abs(rotation - 270) <= tolerance;
    }

}
