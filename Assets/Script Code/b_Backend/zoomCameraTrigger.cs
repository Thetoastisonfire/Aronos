using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomCameraTrigger : MonoBehaviour
{
    [SerializeField] private cameraController cameraControl;

    [Header("zoom data")]
    public float normalZoom = 5f;  // Normal zoom level of the camera
    public float zoomedOutSize = 8f;  // Zoomed out size of the camera
    public int normalOffset = 1;
    public int zoomedOffset = 6;
    public bool shouldSwitch = false;
    public float zoomSpeed = 2f;
    public float cinematicSpeed = 5f;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerMain"))
        {
            StopAllCoroutines();  // Stop any existing zoom coroutines
            if (!shouldSwitch) StartCoroutine(cameraControl.ZoomCamera(zoomedOutSize, zoomSpeed)); //if not heaven area
            else if (shouldSwitch) {
                StartCoroutine(cameraControl.offsetUpdate(normalOffset, cinematicSpeed)); //if in heaven area
                StartCoroutine(cameraControl.ZoomCamera(zoomedOutSize, cinematicSpeed)); //functionality of enter/exit is switched in heaven
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("PlayerMain"))
        {
            StopAllCoroutines();  // Stop any existing zoom coroutines
            if (!shouldSwitch) StartCoroutine(cameraControl.ZoomCamera(normalZoom, zoomSpeed)); //if not heaven area
            else if (shouldSwitch) {
                StartCoroutine(cameraControl.offsetUpdate(zoomedOffset, cinematicSpeed)); //if in heaven area
                StartCoroutine(cameraControl.ZoomCamera(normalZoom, cinematicSpeed)); //functionality of enter/exit is switched in heaven
            }
        }
    }
}