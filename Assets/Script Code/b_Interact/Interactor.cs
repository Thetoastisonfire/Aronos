using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

interface Interactible {
    public void Interact();
}


public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public float interactRange;
    public DialogScript dialogueScript;
    public PlayerMovement pMove; //stop movement when interacting

    //interact light stuff
    public GameObject interactLight;
    public InteractLight interactLightScript;
    private float lightColor;

    // Start is called before the first frame update
    void Start()
    {
        interactRange = 2f;


        if (interactLight == null)
        {
            interactLight = this.gameObject;
        }
    }

    private void Update() {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            // If the pointer is over a UI element, do not proceed with the raycast
            return;
        }

        //if E or CLICK or TAP
        bool interactPressed = Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (interactPressed && dialogueScript.image.color.a == 0 && !GlobalData.Instance.doingSomething) {

            lightColor = interactLight.GetComponent<SpriteRenderer>().color.a;
            if (lightColor != 0f) { //only if the interact light is on

                // Cast a ray from the interactorSource position in the forward direction
                RaycastHit2D hit = Physics2D.Raycast(interactorSource.position, interactorSource.right, interactRange);

                // Draw the ray in the Scene view for debugging
                Debug.DrawRay(interactorSource.position, interactorSource.right * interactRange, Color.red, 2f);

                if (hit.collider != null) {
                    Debug.Log("working, hit: " + hit.collider.name);

                    if (hit.collider.TryGetComponent(out Interactible interactObj)) {
                        ////Debug.Log("working3");
                        GlobalData.Instance.currentlyInteracting = true;
                        interactObj.Interact();
                        interactLightScript.turnOffInteractLight();
                    } else {
                        ////Debug.Log("No Interactible component found on " + hit.collider.name);
                    }
                } else {
                    ////Debug.Log("Raycast did not hit anything");
                }
            }
        }
    }
}
