using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SavingHelper
{
    public static readonly string ResourcesCount = "ResourcesCount";
    public static readonly string Resource = "Resource";
    public static readonly string SavedGame = "IsGameSaved";
    public static readonly string Medical = "Medical";

    public static readonly string XCoordinate = "X";
    public static readonly string YCoordinate = "Y";
    public static readonly string Tile = "Tile";
    public static readonly string Location = "Location";
    public static readonly string LocationCount = "LocationCount";
    public static readonly string Center = "Center";
    public static string TileCenter { get { return Tile + Center; } }
    public static readonly string IsSaved = "IsSaved";

    public static readonly string State = "State";
    public static readonly string Char = "Char";
    public static readonly string Gun = "Gun";
    public static readonly string CharCount = "CharCount";
    public static readonly string Car = "Car";
    public static readonly string Speed = "Speed";
    public static readonly string Food = "Food";
    public static readonly string Fuel = "Fuel";
    public static readonly string Backpack = "backpack";

    public static readonly string SavingIsNotAllowed = "SaveIsNotAllowed";

    public static bool IsGameSaved
    {
        get { return PlayerPrefs.GetInt(SavedGame) > 0; }
    }
    public static void GameOver()
    {
        PlayerPrefs.SetInt(SavingIsNotAllowed, 1);
    }
}

public class SaveLoadManager : MonoBehaviour
{
    bool isAppClosed = false;
    List<SaveableBehavior> saveableContainer;
    bool IsSavingAllowed
    {
        get
        {
            int notAllowed = PlayerPrefs.GetInt
            (
                SavingHelper.SavingIsNotAllowed
            );
            return notAllowed == 0;
        }
    }
    void Start()
    {
        Load();
    }
    void Save()
    {
        if(!IsSavingAllowed)
        {
            PlayerPrefs.DeleteAll();
            return;
        }
        Debug.Log("Save");
        PlayerPrefs.SetInt(SavingHelper.SavedGame, 1);
        if (saveableContainer != null)
            foreach (SaveableBehavior saveable in saveableContainer)
                if (saveable)
                    saveable.Save();
    }
    void Load()
    {
        if (!SavingHelper.IsGameSaved)
            return;
        Debug.Log("Load");
        if (saveableContainer != null)
            foreach (SaveableBehavior saveable in saveableContainer)
                if (saveable)
                    saveable.Load();
    }
    void DestroySaveFile()
    {
        PlayerPrefs.DeleteAll();
    }
    void OnApplicationQuit()
    {
        DestroySaveFile();
        isAppClosed = true;
    }
    void OnDestroy()
    {
        if (!isAppClosed)
            Save();
    }

    public void AddSaveable(SaveableBehavior saveable)
    {
        if (saveableContainer == null)
            saveableContainer = new List<SaveableBehavior>();
        saveableContainer.Add(saveable);
    }
}
