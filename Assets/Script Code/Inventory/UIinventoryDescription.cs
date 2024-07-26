using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIinventoryDescription : MonoBehaviour
{
    
     //[Header("Shown Data")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    

    public void Awake() {
        ResetDesc();
    }

    public void ResetDesc() {
        this.itemImage.gameObject.SetActive(false);
        this.titleText.text = ""; //maybe change later to like "you have naught." or smth
        this.descText.text = "";
    }


    public void SetDesc(Sprite sprite, string title, string desc) {

        this.itemImage.gameObject.SetActive(true); 
        if (sprite == null) this.itemImage.enabled = false;
         else {
            this.itemImage.enabled = true;
            this.itemImage.sprite = sprite; //if no sprite then it doesn't show anything
         }
        this.titleText.text = title; //already a string
        this.descText.text = desc;
    }

    
}
