using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallAndBegin : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Object to activate after falling
    [SerializeField] private float liftSpeed = -1f; // Speed of falling
    [SerializeField] private float fallSpeed = 5f; // Speed of falling
    [SerializeField] private float fallDuration = 1f; // Duration of falling

    private Vector3 initialPosition;
    private bool isFalling = false;

    private void Start()
    {
        initialPosition = transform.position;
        targetObject.SetActive(false); // Ensure the target object is initially inactive
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerMain") && !isFalling)
        {
            isFalling = true;
            StartCoroutine(FallAndActivate());
        }
    }

    private IEnumerator FallAndActivate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            transform.position = new Vector3(
                transform.position.x,
                initialPosition.y - (liftSpeed * elapsedTime),
                transform.position.z
            );

            liftSpeed -= liftSpeed * 0.1f * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        initialPosition = transform.position;
        elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            transform.position = new Vector3(
                transform.position.x,
                initialPosition.y - (fallSpeed * elapsedTime),
                transform.position.z
            );
            fallSpeed += fallSpeed * 0.1f * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetObject.SetActive(true); // Activate the target object after falling
        this.gameObject.SetActive(false);
    }
}
