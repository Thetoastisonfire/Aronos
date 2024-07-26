using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBarScript : MonoBehaviour
{

    public Slider slider;
    
    public void setHunger(int hunger){
        slider.value = hunger;
    }

    public void setMaxHunger(int hunger){
        slider.maxValue = hunger;
        slider.value = hunger;
    }
}
