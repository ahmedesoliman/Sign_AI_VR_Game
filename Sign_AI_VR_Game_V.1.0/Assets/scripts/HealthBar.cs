using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is connected to Runtime
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = Runtime.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
	    //Update the players health by every frame
        slider.value = Runtime.health;

    }
}
