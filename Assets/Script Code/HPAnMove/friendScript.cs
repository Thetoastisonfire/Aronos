using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class friendScript : MonoBehaviour
{
   
    public Slider slider;
    
    public void setFriend(int fren){
        slider.value = fren;
    }

    public void setMaxFriend(int fren){
        slider.maxValue = fren;
        slider.value = fren;
    }
}

