using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class UIinventoryItem : MonoBehaviour
{
    //[Header("Shown Data")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image EquippedImage;
    private bool empty = true;

    //[Header("Misc.")]
    public event Action<UIinventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag;
    

    public void Awake() {
        ResetData();
        Deselect();
        Unequipped();
    }

    public void ResetData() {
        this.itemImage.gameObject.SetActive(false);
        empty = true;
    }

    public void Deselect() {
        borderImage.enabled = false;
    }

    public void Select() {
        borderImage.enabled = true;
    }

    public void Equipped() {
        EquippedImage.enabled = true;
    }

    public void Unequipped() {
        EquippedImage.enabled = false;
    }

    public void SetData(Sprite sprite, int quantity) {
        if (sprite != null) {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        }

        if (quantity <= 1) this.quantityText.text = ""; //to make it a string
         else this.quantityText.text = quantity + ""; //doesn't show quantity if only have one
        empty = false;
    }

    public void OnBeginDrag() {
        if (empty) return;
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop() {
        OnItemDroppedOn?.Invoke(this);
    }

    public void OnEndDrag() {
        OnItemEndDrag?.Invoke(this); // invokes the action or whatever
    }

    public void OnPointerClick() {
        Deselect();
        OnItemClicked?.Invoke(this);
    }
    
}
