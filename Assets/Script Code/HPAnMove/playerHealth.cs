using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{

    [SerializeField] private healthBarScript healthHungerBar;
    [SerializeField] private friendScript frenBar;
    [SerializeField] private npcGuyInteract npc;
    [SerializeField] private InventoryStat invStats;


    // Start is called before the first frame update
    void Start()
    {
        GlobalData.Instance.currentFriendship = GlobalData.Instance.maxFriendship;
        GlobalData.Instance.currentHunger = GlobalData.Instance.maxHunger;
        healthHungerBar.setMaxHunger(GlobalData.Instance.maxHunger);
        frenBar.setMaxFriend(GlobalData.Instance.maxFriendship);

    }

    // can change values on press of buttons
    void Update()
    {
     //   if (Input.GetKeyDown(KeyCode.Space)) takeDamage(20);

      //  if (Input.GetKeyDown(KeyCode.E)) takeEmotionalDamage(2)
    }

    public void takeDamage(int damage) {
        if (GlobalData.Instance.currentHunger > 0) {
            GlobalData.Instance.currentHunger -= damage;

            if (GlobalData.Instance.currentHunger > GlobalData.Instance.maxHunger)
                GlobalData.Instance.currentHunger = GlobalData.Instance.maxHunger;
            else if (GlobalData.Instance.currentHunger < 0)
                GlobalData.Instance.currentHunger = 0;

            healthHungerBar.setHunger(GlobalData.Instance.currentHunger);
            invStats.updateStats(GlobalData.Instance.currentHunger, GlobalData.Instance.currentFriendship);
        }
    }

    public void takeEmotionalDamage(int frenDamage) {
        if (GlobalData.Instance.currentFriendship > 0) {
            GlobalData.Instance.currentFriendship -= frenDamage;
            
            //make sure it doesn't go over max
            if (GlobalData.Instance.currentFriendship > GlobalData.Instance.maxFriendship)
                GlobalData.Instance.currentFriendship = GlobalData.Instance.maxFriendship;
            else if (GlobalData.Instance.currentFriendship < 0)
                GlobalData.Instance.currentFriendship = 0;

            frenBar.setFriend(GlobalData.Instance.currentFriendship);
            invStats.updateStats(GlobalData.Instance.currentHunger, GlobalData.Instance.currentFriendship);
        }
    }

    public void checkHealth() { //check if they're dead
        if (GlobalData.Instance.currentHunger <= 1) {
            GlobalData.Instance.longSleep = 0;                         // 0 = hunger death
        } else if (GlobalData.Instance.currentFriendship <= 1) {
            GlobalData.Instance.longSleep = 1;                         // 1 = friendship death
        } else {
             npc.warningGiven = true;
             npc.healthWarning(); //if still alive
            //  GlobalData.Instance.longSleep = 2; //not dead
        }

    }


}
