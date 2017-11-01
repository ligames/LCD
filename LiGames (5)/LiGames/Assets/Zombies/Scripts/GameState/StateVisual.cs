using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateVisual : MonoBehaviour
{
    public Text fuelText, foodText;
    public void Bind(State state)
    {
        state.onFuelChanged += RenderFuel;
        state.onFoodChanged += RenderFood;
    }
    void RenderFuel(int val)
    {
        fuelText.text = val.ToString();
    }
    void RenderFood(int val)
    {
        foodText.text = val.ToString();
    }
}
