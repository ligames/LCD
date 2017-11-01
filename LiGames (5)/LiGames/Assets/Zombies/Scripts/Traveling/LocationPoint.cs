using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationPoint : MonoBehaviour
{
    [SerializeField] IncreaseAnimation increaseAnimation;
    //public int sceneIndex = 0;
    public Button goButton;
    public Transform realLocTransform;
    public float distance;
    public Text fuelCostText, foodCostText;

    public bool Interactable
    {
        get { return button.interactable; }
        set { button.interactable = value; }
    }

    MapManager manager;
    LocationPoint self;
    bool active = false;
    int foodCost, fuelCost;
    Button button;

    int FoodCost
    {
        get { return foodCost; }
        set
        {
            foodCost = value;
            foodCostText.text = foodCost.ToString();
        }
    }
    int FuelCost
    {
        get { return fuelCost; }
        set
        {
            fuelCost = value;
            fuelCostText.text = fuelCost.ToString();
        }
    }

    void Awake()
    {
        manager = FindObjectOfType<MapManager>();
        self = GetComponent<LocationPoint>();
        transform.position = Camera.main.WorldToScreenPoint(realLocTransform.position);
        goButton.onClick.AddListener(new UnityEngine.Events.UnityAction(manager.DriveToNextLocation));
        button = GetComponent<Button>();
    }
    public void OnClick()
    {
        if (active)
            return;
        //increaseAnimation.ShowAnimation();
        manager.OnLocationPointClick(self);
        active = true;
    }
    public void Deactivate()
    {
        increaseAnimation.HideElements();
        active = false;
    }
    public void CalculateResources(float speed)
    {
        FuelCost = Round(manager.State.FuelSpendingMult * distance);
        FoodCost = Round(manager.State.EatingMult * (distance / speed));
        goButton.interactable = manager.State.IsResOk(FuelCost, FoodCost);
    }
    int Round(float val)
    {
        int addition = 0;
        if (val - (int)val - 0.5 >= 0)
            addition = 1;
        return (int)val + addition;
    }
}
