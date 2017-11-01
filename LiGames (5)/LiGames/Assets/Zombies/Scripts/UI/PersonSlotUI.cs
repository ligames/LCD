using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PersonSlotUI : MonoBehaviour
{
    [Inject]
    PersonManagmentUI personManagementUI;
    [Inject]
    GlobalInfoAccessor globalInfo;
    [SerializeField]
    RawImage icon;

    PersonState state;
    public PersonState State { get { return state; } }
    public void Assign(PersonState newState)
    {
        state = newState;
        UpdateView();
    }
    public void Select()
    {
        personManagementUI.Assign(state);
    }
    void UpdateView()
    {
        bool visible = state != null;
        if (visible)
            icon.texture = globalInfo.CharsPrefabs[state.Id].Icon.texture;
        icon.gameObject.SetActive(visible);
    }
}
