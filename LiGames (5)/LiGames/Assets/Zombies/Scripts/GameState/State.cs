using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PersonState
{
    public PersonState(int id)
    {
        Id = id;
    }
    public int Id { get; private set; }
    public int GunId { get; set; }
    public void Save(int number)
    {
        PlayerPrefs.SetInt
       (
           SavingHelper.State + SavingHelper.Char + number,
           Id
       );
        PlayerPrefs.SetInt
        (
            SavingHelper.State + SavingHelper.Char + number + SavingHelper.Gun,
            GunId
        );
    }
}
public class CarState
{
    public CarState(int id, float speed)
    {
        Id = id;
        Speed = speed;
    }
    public int Id { get; private set; }
    public float Speed { get; private set; }
    public void Save()
    {
        PlayerPrefs.SetInt
        (
            SavingHelper.State + SavingHelper.Car,
            Id
        );
        PlayerPrefs.SetFloat
        (
            SavingHelper.State + SavingHelper.Car + SavingHelper.Speed,
            Speed
        );
    }
}

public class State : SaveableBehavior
{
    [Inject]
    GlobalInfoAccessor globalInfo;
    [Inject]
    StateDefaultInfo defaultInfo;
    [Inject]
    BackpackContents backpack;

    [SerializeField]
    StateVisual visual;

    int fuel, food;
    float speed = 60.0f;
    float eachPersonEatingMult = .75f;
    float fuelSpendingMult = .1f;
    bool isDataLoaded = false;

    CarState carState;
    public CarState CarState
    {
        get
        {
            if (!isDataLoaded)
                LoadData();
            return carState;
        }
        private set
        {
            carState = value;
        }
    }
    List<PersonState> chars;
    public List<PersonState> Chars
    {
        get
        {
            if (!isDataLoaded)
                LoadData();
            return chars;
        }
        set { chars = value; }
    }

    public event System.Action<int> onFuelChanged, onFoodChanged;
    public event System.Action<float> onSpeedChanged;

    new void Awake()
    {
        base.Awake();
        LoadData();
    }
    void Start()
    {
        StartCoroutine(SpeedObserving());
    }
    public int Fuel
    {
        get { return fuel; }
        set
        {
            fuel = value;
            if (onFuelChanged != null)
                onFuelChanged(fuel);
        }
    }
    public int Food
    {
        get { return food; }
        set
        {
            food = value;
            if (onFoodChanged != null)
            {
                onFoodChanged(food);
            }
        }
    }
    public float EatingMult { get { return eachPersonEatingMult; } }
    public float FuelSpendingMult { get { return fuelSpendingMult; } }

    public bool IsResOk(float fuelCost, float foodCost)
    {
        return fuelCost <= Fuel && foodCost <= Food;
    }
    public void RemoveChar(int id)
    {
        PersonState charToRemove = Chars.Find
        (
            (character) => character.Id == id
        );
        Chars.Remove(charToRemove);
    }
    public void UseFuel(int value)
    {
        Fuel -= value;
    }
    public void UseFood(int value)
    {
        Food -= value;
    }

    void GenerateState()
    {
        GenCharsState();
        GenCarState();
        GenFoodState();
        GenFuelState();
        GenResources();
    }

    List<int> GetIdentificators(List<PersonDescriptor> container)
    {
        List<int> identificators = new List<int>();
        for (int i = 0; i < container.Count; ++i)
            identificators.Add(i);
        return identificators;
    }
    List<int> GetIdentificators(List<GameObject> container)
    {
        List<int> identificators = new List<int>();
        for (int i = 0; i < container.Count; ++i)
            identificators.Add(i);
        return identificators;
    }

    void GenResources()
    {
        for (int i = 0; i < 10; ++i)
            backpack.AddItem(globalInfo.AllResources[(int)Random.Range(0, globalInfo.AllResources.Count)]);
    }
    void GenCharsState()
    {
        Chars = new List<PersonState>();
        //
        for (int i = 22; i < 26; ++i)
            Chars.Add(new PersonState(i) { GunId = 0 });
        return;
        //
        List<int> charsIdentificators = GetIdentificators(globalInfo.CharsPrefabs);
        System.Random numberGenerator = new System.Random();
        for (int i = 0; i < defaultInfo.PersonsCount; ++i)
        {
            int randomIndex = numberGenerator.Next(charsIdentificators.Count);
            int index = charsIdentificators[randomIndex];
            charsIdentificators.Remove(index);
            Chars.Add
            (
                new PersonState(index) { GunId = -1 }
            );
        }
    }
    void GenCarState()
    {
        List<int> carsIdentificators = GetIdentificators(globalInfo.CarsPrefabs);
        System.Random numberGenerator = new System.Random();
        int id = 5;// numberGenerator.Next(carsIdentificators.Count);
        float speed = GenFloatState
        (
            defaultInfo.MinCarSpeed,
            defaultInfo.MaxCarSpeed
        );
        CarState = new CarState(id, speed);
    }
    void GenFoodState()
    {
        Food = GenIntState
        (
            defaultInfo.MinFood,
            defaultInfo.MaxFood
        );
    }
    void GenFuelState()
    {
        Fuel = GenIntState
        (
            defaultInfo.MinFuel,
            defaultInfo.MaxFuel
        );
    }

    int GenIntState(int minVal, int maxVal)
    {
        return (int)GenFloatState(minVal, maxVal);
    }
    float GenFloatState(float minVal, float maxVal)
    {
        return Random.Range(minVal, maxVal);
    }

    IEnumerator SpeedObserving()
    {
        while (CarState == null)
            yield return null;
        if (onSpeedChanged != null)
            onSpeedChanged(CarState.Speed);
        while (true)
        {
            float carSpeed = CarState.Speed;
            while (carSpeed == CarState.Speed)
                yield return null;
            if (onSpeedChanged != null)
                onSpeedChanged(CarState.Speed);
            yield return null;
        }
    }

    public override void Save()
    {
        PlayerPrefs.SetInt
        (
            SavingHelper.State + SavingHelper.CharCount,
            Chars.Count
        );
        for (int i = 0; i < Chars.Count; ++i)
            Chars[i].Save(i);
        CarState.Save();
        //Save food
        PlayerPrefs.SetInt
        (
            SavingHelper.State + SavingHelper.Food,
            Food
        );
        //Save fuel
        PlayerPrefs.SetInt
        (
            SavingHelper.State + SavingHelper.Fuel,
            Fuel
        );
    }
    void LoadData()
    {
        if (isDataLoaded)
            return;
        //StartCoroutine(SpeedObserving());
        if (visual)
            visual.Bind(GetComponent<State>());
        isDataLoaded = true;
        if (!SavingHelper.IsGameSaved)
        {
            GenerateState();
            return;
        }

        // Chars loading
        Chars = new List<PersonState>();
        int count = PlayerPrefs.GetInt
        (
            SavingHelper.State + SavingHelper.CharCount
        );
        for (int i = 0; i < count; ++i)
            Chars.Add
            (
                new PersonState
                (
                    PlayerPrefs.GetInt
                    (
                        SavingHelper.State + SavingHelper.Char + i
                    )
                )
                {
                    GunId = PlayerPrefs.GetInt
                    (
                        SavingHelper.State + SavingHelper.Char + i + SavingHelper.Gun
                    )
                }
            );
        // Car loading
        CarState = new CarState
        (
            PlayerPrefs.GetInt
            (
                SavingHelper.State + SavingHelper.Car
            ),
            PlayerPrefs.GetFloat
            (
                SavingHelper.State + SavingHelper.Car + SavingHelper.Speed
            )
        );
        // Food loading
        Food = PlayerPrefs.GetInt
        (
            SavingHelper.State + SavingHelper.Food
        );
        // Fuel loading
        Fuel = PlayerPrefs.GetInt
        (
            SavingHelper.State + SavingHelper.Fuel
        );
    }
}
