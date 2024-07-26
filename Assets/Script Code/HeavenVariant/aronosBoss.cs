using System.Collections;
using UnityEngine;

public class aronosBoss : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f; // Speed of falling
    [SerializeField] private float fallDuration = 3f; // Duration of falling
    private Vector3 initialPosition;


    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        StartCoroutine(Rise(new Vector3(initialPosition.x, initialPosition.y + (fallSpeed * fallDuration), initialPosition.z)));
    }

    public void Update() {

    }

     private IEnumerator Rise(Vector3 targetPosition) {
        float elapsedTime = 0f;
        Debug.Log("boss rising");
        while (elapsedTime < fallDuration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set
        transform.position = targetPosition;
    }

}
