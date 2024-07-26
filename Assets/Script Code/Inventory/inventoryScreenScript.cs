using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class inventoryScreenScript : MonoBehaviour
{

#region variables
   // public static inventoryScreenScript Instance { get; private set; }

   // public Dictionary
    [Header("ui prefab script data")]
   [SerializeField] private UIinventoryItem itemPrefab;
   [SerializeField] private UIinventoryDescription itemDesc;
   [SerializeField] private RectTransform contentPanel;
   public CanvasGroup canvasGroup;

     [Header("inventory data")]
     //dictionary data structure has index followed by a tuple of the inventory item's physical object and its item data
    private Dictionary<int, (UIinventoryItem itemObject, InventoryItem itemData)> listOfItems = new Dictionary<int, (UIinventoryItem, InventoryItem)>();
    public UIinventoryItem currentlySelectedItem;  // Reference to the currently selected item
    private int nextItemId = 0;

     [Header ("Anim and Audio")]
    private Animator buttonAnimation;
    [SerializeField] private GameObject inventoryButton;
    private float fadeDuration = 0.5f;
    
     [Header("button data")]
    [SerializeField] private GameObject buttonPanel1; //back settings audio
    [SerializeField] private GameObject buttonPanel2; //equip use discard

    //ugh
     [Space(20)]
    [SerializeField] private MenuButton buttonPanel2_EquipButton;
    [SerializeField] private TextMeshProUGUI buttonPanel2_EquipButton_Text;
    [SerializeField] private MenuButton buttonPanel2_UseButton;
    [SerializeField] private TextMeshProUGUI buttonPanel2_UseButton_Text;
    [SerializeField] private MenuButton buttonPanel2_DiscardButton;
    [SerializeField] private TextMeshProUGUI buttonPanel2_DiscardButton_Text;

#endregion

#region initializing
    private void Awake() {
        buttonAnimation = inventoryButton.GetComponent<Animator>(); //animation
        itemDesc.ResetDesc();
       // Instance = this;
    }

    public void InitInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            AddNewItem(null, -1, "", "", -1); //image null, quantity -1, title empty, description empty, item id 0 (not interactible)
        }
   }

#endregion

#region adding items to inventory
    //master adder
     public void AddNewItem(Sprite image, int quantity, string title, string description, int ItemID)
    {
        int itemIdInListIndex = nextItemId++;
       // Debug.Log("item id in addnewitem is "+ ItemID);
        AddItemToUI(itemIdInListIndex, new InventoryItem (image, quantity, title, description, ItemID)); //should do everything

        //make inventory button show that theres a new item
        buttonAnimationOn();
    }


    public void AddItemToUI(int itemIdInListIndex, InventoryItem inventoryItem)
    {
        if (listOfItems.ContainsKey(itemIdInListIndex))
        {
            Debug.LogWarning("Item with the same ID already exists in the UI.");
            return;
        }

        UIinventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        uiItem.transform.SetParent(contentPanel, false);
        uiItem.SetData(inventoryItem.image, inventoryItem.quantity);

     //   Debug.Log("item id in additemtoui is "+ inventoryItem.ItemID);
        listOfItems.Add(itemIdInListIndex, (uiItem, inventoryItem));

        // OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag
        uiItem.OnItemClicked += HandleItemSelection;
        uiItem.OnItemDroppedOn += HandleSwap;
        uiItem.OnItemBeginDrag += HandleBeginDrag;
        uiItem.OnItemEndDrag += HandleEndDrag;
    }

    public void overwriteItem(Sprite image, int quantity, string title, string description, int ItemID) {
        // Iterate through the listOfItems to find the first empty slot (where image is null)
        for (int itemIdInListIndex = 0; itemIdInListIndex < listOfItems.Count; itemIdInListIndex++) {
            if(listOfItems[itemIdInListIndex].itemData.quantity == -1) { //if quantity shows its an empty one
                UpdateData(itemIdInListIndex, image, quantity, title, description, ItemID); //update data
                return;
            }
        }

        //if no empty things found, add new item
        AddNewItem(image, quantity, title, description, ItemID);
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity, string title, string description, int ItemID)
    {
        if (listOfItems.ContainsKey(itemIndex))
        {
            var (uiItem, itemData) = listOfItems[itemIndex];

            // Update the item data
            itemData.image = itemImage;
            itemData.quantity = itemQuantity;
            itemData.title = title;
            itemData.description = description;
            itemData.ItemID = ItemID;

            // Update the UI item
            uiItem.SetData(itemImage, itemQuantity);

            // Update the dictionary entry with the modified item data
            listOfItems[itemIndex] = (uiItem, itemData);
        }

        //make inventory button show that theres a new item
        buttonAnimationOn();
    }

#endregion

#region delete item from inventory
    public void RemoveItemFromUI(int itemIdInListIndex) {
        if (listOfItems.ContainsKey(itemIdInListIndex))
        {
            var (uiItem, _) = listOfItems[itemIdInListIndex];

            // Unsubscribe from events
            uiItem.OnItemClicked -= HandleItemSelection;
            uiItem.OnItemDroppedOn -= HandleSwap;
            uiItem.OnItemBeginDrag -= HandleBeginDrag;
            uiItem.OnItemEndDrag -= HandleEndDrag;

            // Destroy the UI item GameObject
            Destroy(uiItem.gameObject);

            // Remove the item from the dictionary
            listOfItems.Remove(itemIdInListIndex);

            // Optional: handle the currently selected item
            if (currentlySelectedItem == uiItem)
            {
                currentlySelectedItem = null;
            }

        //    Debug.Log("Item removed successfully.");

            AddNewItem(null, -1, "", "", -1); //to fill the gap of the deleted item in inventory
        }
        else
        {
            Debug.LogWarning("Item with the specified ID does not exist in the UI.");
        }
    }

#endregion

#region item selection
    private void HandleItemSelection(UIinventoryItem obj) //if an item is clicked
    {   
        int index = GetUIItemIndex(obj);
        if (index == -1) return;

        // Deselect the previously selected item
        if (currentlySelectedItem != null && currentlySelectedItem != obj)
        {
            currentlySelectedItem.Deselect();
            turnOffEquipPanel(); //turn off by default
        }

        var (uiItem, item) = listOfItems[index];
        if (item.quantity != -1) obj.Select();
        itemDesc.SetDesc(item.image, item.title, item.description);

        //logic for when an item is opened
        if (!string.IsNullOrEmpty(item.title)) {
            //get item's id for panel
            GlobalData.Instance.ItemID = item.ItemID;
         //   Debug.Log("id is now " + item.ItemID);

            //initialize buttons and texts to default state, all start turned off
            InitializePanelButtons();
            
            //turn on buttons that can be pressed
            string itemString = item.ItemID.ToString();
            for (int i = 0; i < itemString.Length; i++) {
                switch (itemString[i]) {
                    case '0':
                        i+=999;
                        break;
                    case '1': //equip
                        EnableButton(buttonPanel2_EquipButton, buttonPanel2_EquipButton_Text, "Equip");
                        break;
                    case '2': //unequip
                        EnableButton(buttonPanel2_EquipButton, buttonPanel2_EquipButton_Text, "Unequip");
                        break;
                    case '3': //use
                        EnableButton(buttonPanel2_UseButton, buttonPanel2_UseButton_Text, "Use");
                        break;
                    case '4': //discard
                        EnableButton(buttonPanel2_DiscardButton, buttonPanel2_DiscardButton_Text, "Discard");
                        break;
                    default:
                        break;
                }
            }
            turnOnEquipPanel(); //options for item IF item is filled
        }

        currentlySelectedItem = obj; // Update the reference to the currently selected item

    }
    #region item selection helper methods
    private void InitializePanelButtons() {
        buttonPanel2_EquipButton.action = "notEnabled";
        buttonPanel2_UseButton.action = "notEnabled";
        buttonPanel2_DiscardButton.action = "notEnabled";

        buttonPanel2_EquipButton_Text.text = "Equip";
        setColor(buttonPanel2_EquipButton_Text, 0.5f);
        buttonPanel2_UseButton_Text.text = "Use";
        setColor(buttonPanel2_UseButton_Text, 0.5f);
        buttonPanel2_DiscardButton_Text.text = "Discard"; 
        setColor(buttonPanel2_DiscardButton_Text, 0.5f);
    }

    private void EnableButton(MenuButton button, TextMeshProUGUI buttonText, string action)
    {
        button.action = action;
        buttonText.text = action;
        setColor(buttonText, 1f);
    }

    public void turnOnEquipPanel(){
        //equip use discard
        buttonPanel1.SetActive(false);
        buttonPanel2.SetActive(true);
    }

    public void turnOffEquipPanel(){
        //back audio settings
        buttonPanel1.SetActive(true);
        buttonPanel2.SetActive(false);
    }
    #endregion

    private int GetUIItemIndex(UIinventoryItem uiItem) //helps get the correct item from the dictionary
    {
        foreach (var kvp in listOfItems)
        {
            if (kvp.Value.itemObject == uiItem) {
                actionIDSystem.Instance.currentlySelectedItemListID = kvp.Key;
                return kvp.Key;
            }
        }
        return -1;
    }

    public int changeIDinUIItemIndex(UIinventoryItem uiItem, int newID) //helps get the correct item from the dictionary
    {
       // Debug.Log("new id is " + newID);
        foreach (var kvp in listOfItems)
        {
            if (kvp.Value.itemObject == uiItem) {
                actionIDSystem.Instance.currentlySelectedItemListID = kvp.Key;
                kvp.Value.itemData.ItemID = newID;
                return kvp.Key;
            }
        }
        return -1;
    }

    private void setColor(TextMeshProUGUI textComponent, float opacity){
        // Get the current color
            Color currentColor = textComponent.color;
            
            // Halve the alpha value
            currentColor.a = opacity;

            // Set the new color back to the text component
            textComponent.color = currentColor;
    }

    public void hardSetOpacity(float targetOpacity) { //hard set without fading
        canvasGroup.interactable = (targetOpacity != 0);
        canvasGroup.blocksRaycasts = (targetOpacity != 0);
        canvasGroup.alpha = targetOpacity;
    }
    public void SetOpacity(float targetOpacity) // Adds duration parameter for smooth fade
    {
        float duration = fadeDuration;
        bool onOffSwitch = (targetOpacity != 0); //if not 0 then true, turning inventory on, if 0 then false, turning it off
        
        // disables interaction if opacity is zero
        canvasGroup.interactable = onOffSwitch;
        canvasGroup.blocksRaycasts = onOffSwitch;
        if (onOffSwitch) buttonAnimation.SetBool("buttonTime", false);

        StartCoroutine(FadeTo(targetOpacity, duration));
    }

    private IEnumerator FadeTo(float targetOpacity, float duration) //changes canvas group opacity
    {
        float startOpacity = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startOpacity, targetOpacity, elapsed / duration);
            yield return null;
        }

        // Ensure the final opacity is set correctly
        canvasGroup.alpha = targetOpacity;
    }

#endregion

    public void buttonAnimationOn() {
        buttonAnimation.SetBool("buttonTime", true);
    }

    public void buttonAnimationOff() {
        buttonAnimation.SetBool("buttonTime", false);
    }

#region misc action events

    private void HandleSwap(UIinventoryItem obj) {
        
    }

    private void HandleBeginDrag(UIinventoryItem obj) {
        
    }

    private void HandleEndDrag(UIinventoryItem obj) {
        
    }
#endregion

}
