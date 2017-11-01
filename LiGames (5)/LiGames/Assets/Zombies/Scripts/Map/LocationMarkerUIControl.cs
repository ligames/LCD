using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationMarkerUIControl : MonoBehaviour
{
    IncreaseAnimation increaseAnimation;
    [SerializeField] Button goButton;
    [SerializeField] RectTransform fuelInfo, foodInfo;
    [SerializeField]
    RectTransform circle;
    public Button GoButton { get { return goButton; } }

    Text fuelText, foodText;
    public Text FuelCostText { get { return fuelText; } }
    
    public Text FoodCostText { get { return foodText; } }

    public Location StartLocation { get; set; }
    public Location Location { get; set; }
    float Distance { get { return StartLocation ? StartLocation.DistanceTo(Location) : 0; } }

    public bool Interactable
    {
        get { return button.interactable; }
        set { button.interactable = value; }
    }

    TravellingManager manager;
    LocationMarkerUIControl self;
    bool active = false;
    int foodCost, fuelCost;
    Button button;
    State state;

    public int FoodCost
    {
        get { return foodCost; }
        set
        {
            foodCost = value;
            FoodCostText.text = foodCost.ToString();
        }
    }
    public int FuelCost
    {
        get { return fuelCost; }
        set
        {
            fuelCost = value;
            FuelCostText.text = fuelCost.ToString();
        }
    }
    public bool Available { get { return goButton.interactable; } }

    void Awake()
    {
        manager = FindObjectOfType<TravellingManager>();
        self = GetComponent<LocationMarkerUIControl>();
        goButton.onClick.AddListener(new UnityEngine.Events.UnityAction(manager.DriveToNextLocation));
        button = GetComponent<Button>();
        state = FindObjectOfType<State>();
        increaseAnimation = FindObjectOfType<IncreaseAnimation>();

        fuelText = fuelInfo.GetComponentInChildren<Text>();
        foodText = foodInfo.GetComponentInChildren<Text>();
    }
    public void OnClick()
    {
        if (active)
            return;
        manager.OnLocationPointClick(self);
        increaseAnimation.ShowAnimation
            (
                new List<RectTransform>
                {
                    GoButton.GetComponent<RectTransform>(),
                    foodInfo,
                    fuelInfo,
                    circle
                }
            );
        active = true;
    }
    public void Deactivate()
    {
        increaseAnimation.HideElements();
        active = false;
    }
    public void CalculateResources(float speed)
    {
        FuelCost = Round(state.FuelSpendingMult * Distance);
        FoodCost = Round(state.EatingMult * (Distance / speed));
        goButton.interactable = state.IsResOk(FuelCost, FoodCost);
    }
    int Round(float val)
    {
        int addition = 0;
        if (val - (int)val - 0.5 >= 0)
            addition = 1;
        return (int)val + addition;
    }
}