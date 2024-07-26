using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryStat : MonoBehaviour
{
    [SerializeField] private TMP_Text hungerStat;
    [SerializeField] private TMP_Text friendStat;
    [SerializeField] private TMP_Text runeStat;

    // Start is called before the first frame update
    void Start()
    {
        hungerStat.text = $"Hunger-------- {GlobalData.Instance.maxHunger}";
        friendStat.text = $"Affinity-------- {GlobalData.Instance.maxFriendship}";
    }

    public void updateStats(int hunger, int friendship) {
        hungerStat.text = $"Hunger-------- {hunger}";
        friendStat.text = $"Affinity-------- {friendship}";
    }

    public void updateRune() {
        GlobalData.Instance.runesPickedUp += 1;
        runeStat.text = $"Runes-------- {GlobalData.Instance.runesPickedUp}";
    }


}
