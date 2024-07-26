using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class childScriptForJumpPillar : MonoBehaviour
{
    [SerializeField] private jumpPillarScript jumpPillar;
    // Start is called before the first frame update
    private void jumpingEvent(){
        jumpPillar.pillarUpdate();
    }
}
