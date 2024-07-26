using UnityEngine;
using UnityEngine.UI;

public class ScrollViewDebug : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Mouse Click Detected");
            if (RectTransformUtility.RectangleContainsScreenPoint(
                GetComponent<RectTransform>(), 
                Input.mousePosition, 
                Camera.main))
            {
                Debug.Log("Click Inside Scroll View");
            }
            else
            {
                Debug.Log("Click Outside Scroll View");
            }
        }
    }
}
