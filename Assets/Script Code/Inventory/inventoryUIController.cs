using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryUIController : MonoBehaviour
{
    [SerializeField] private inventoryScreenScript inventoryUI;

    public int inventorySize = 10;

    public void Start() {
        inventoryUI.AddNewItem(GlobalData.Instance.spriteBank[10], 1, "Cracked Rune", //13011
        "Allows one to speak in the voice of angels. Alas, it is broken, like you.", 13011);//0); //should be 0, other num are debug
        /* ItemID Guide--
            ------------------------------
                First Part of Tree
                ---------------
                -1: null
                0: not able to interact
                1: able to equip
                2: able to unequip
                3: able to use
                4: able to discard

                e.g. rune 1 not equipped = 13

                e.g. hat equipped = 24
         */
        inventoryUI.InitInventoryUI(inventorySize); //adding blank ones
       // inventoryUI.SetDataStuff();
    }
  
}
