using UnityEngine;

public class Cloud : MonoBehaviour
{
    private float speed;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
}
