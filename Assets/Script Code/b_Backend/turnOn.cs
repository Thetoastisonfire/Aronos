using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class turnOn : MonoBehaviour //just turns on the dialogue box for runtime so I dont have to look at it constantly
{

     [Header ("init other stuff")]
    [SerializeField] private GameObject dialogeBox;

     [Header ("box debug off")]
    [SerializeField] private GameObject ynBox;
    [SerializeField] private GameObject choiceBox;

    [SerializeField] private GameObject invScreen;
    [SerializeField] private inventoryScreenScript invScript;

    [SerializeField] private GameObject signScreen;

     [Header ("init lights")]
    [SerializeField] private new Light2D light; //new keyword to hide inherited light
    [SerializeField] private float lightIntensity = 0.2f;
    
    void Awake()
    {
        dialogeBox.SetActive(true);
        //turn off any dialogue boxes from editor debugging
        ynBox.SetActive(false);
        choiceBox.SetActive(false);
        invScreen.SetActive(true); invScript.hardSetOpacity(0f); //just cause inventory works differently
        signScreen.SetActive(false);
        GlobalData.Instance.dayCount = 1;

        //lighting stuff
        Light2D light2D = light.GetComponent<Light2D>();
        if (light2D != null)
        {
            light2D.intensity = lightIntensity;
        }
    }
}
