using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killBox : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject softDeath;
    [SerializeField] private softDeathScript sdScript;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null) {
       // Debug.LogError("someReference is null");
    }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is the player
        if (other.gameObject == player)
        {
            // Move the player to the specified coordinates
            //player.transform.position = new Vector3(2.54f, 22.3f, 1f);

            softDeath.SetActive(true); //turn on soft death and do cutscene
            sdScript.subsequentStart();
        }
    }

}
