using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeSure : MonoBehaviour
{
    public GameObject yesNoBoxScript;
    public yesNoBox yesNoBoxScriptAgain;
    public makeSure theButton;
    // Start is called before the first frame update
    void Start()
    {
      //  theButton.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick() {
        yesNoBoxScript.SetActive(true);
        yesNoBoxScriptAgain.OnYesButton();
    }
}
