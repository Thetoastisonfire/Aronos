using UnityEngine;

public class GlobalData : MonoBehaviour
{
    // global data
    public static GlobalData Instance { get; private set; }

     [Header ("Current Day")]
    public int dayCount = 1; //current day

    //data for times interacted
     [Header ("Interact Count")]
    public int npcCounter = 0;
    public int junkCounter = 0;
    public int hatchCounter = 0;
    public int hideCounter = 0;
    public int runesPickedUp = 1;

    //junk items
     [Header ("Junk Data")]
    public int currentJunkItem = 10;
    public bool alreadyMentionedJunk = false; //this like, makes sure junk dialogue isnt called multiple times

     [Header ("Hatch Data")]
    public bool inHatch = false;


    //hunger and friendship data
     [Header ("Health Data")]
    public int currentHunger = 0;
    public int maxHunger = 100;
    public int currentFriendship = 0;
    public int maxFriendship = 100;

    //tired counter; Things that make player tired: junk, hatch. Things that don't: npc, crate, nest
     [Header ("Tired Data")]
    public int tiredCounter = 0;
    public int minimumTired = 2;

    //death tracker
     [Header ("Player State")]
    public int longSleep = 2; //0 is hunger die, 1 is frend die, 2 is alive


    //player blocking data
     [Header ("Interaction Blocking")]
    public bool currentlyInteracting = false; //this prevents player from moving during important things
    public bool doingSomething = false; //this prevents player from triggering unwanted ui during ui interactions
    public bool currentlyInventory = false; //in inventory

    //player data and inventory
     [Header ("Misc Variables")]
    public float speed = 5f;
    public float jumpForce = 10f;

    //number will tell what action and what id is being called
    [Header ("Item ID System")]
    public int ActionID = -1;
    #region action ID guide
      /* Action ID Guide--
            ------------------------------
                First Part of Tree
                ---------------
                -1: null
                0: not doing anything
                1: equipping something
                2: using something
                3: discarding something

                -----------
                Second Part of Tree
                --------------
                0: unable to perform action
                1: able to perform action
                2: able to perform reverse of action, e.g. unequipping
        
                -----------
                Third Part of Tree:
                --------------
                00: unknown item
                01: pipe
                02: knife
                03: meat lump
                04: puppet
                05: tire
                06: hat
                07: trinket
                08: egg
                09: mask
                10: timepiece
                11: rune 1
                12: rune 2
                13: rune 3
            ------------------------------

            e.g. equipping a hat that is already equipped
                item id = 1206 (1, 2, 06)

            e.g. equipping a hat that is not equipped
                item id = 1106 (1, 2, 06)

            e.g. discarding rune 3
                item id = 3013 (3, 0, 13)
            
            e.g. using a meat lump
                item id = 2103 (2, 1, 03)
    
            */
    #endregion

    public int ItemID  = -1; //all items have an id, global is updated when item is selected
    #region item ID guide
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

                --add a zero as a spacer--

                Second Part of Tree
                -------------
                00: unknown item
                01: pipe
                02: knife
                03: meat lump
                04: puppet
                05: tire
                06: hat
                07: trinket
                08: egg
                09: mask
                10: timepiece
                11: rune 1
                12: rune 2
                13: rune 3

                e.g. rune 1 not equipped = 13 + 0 + 11: 13011

         */
    
    #endregion
    
    public string equippedItemEffect = "none";
        //none = no effect
        //hatch = free hatch scratch


     [Header ("Sprite Bank")] //for inventory items
    public Sprite[] spriteBank;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Enforce singleton pattern
        }
    }
}
