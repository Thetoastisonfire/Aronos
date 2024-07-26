using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public Sprite image;
    public int quantity;
    public string title;
    public string description;
    public int ItemID;

    public InventoryItem(Sprite image, int quantity, string title, string description, int ItemID)
    {
        this.image = image;
        this.quantity = quantity;
        this.title = title;
        this.description = description;
        this.ItemID = ItemID;
    }
}
