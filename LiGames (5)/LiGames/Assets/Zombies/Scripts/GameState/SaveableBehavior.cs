using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveableBehavior : MonoBehaviour
{
    SaveLoadManager manager;
    SaveableBehavior self;

    protected void Awake()
    {
        manager = FindObjectOfType<SaveLoadManager>();
        self = this;
        manager.AddSaveable(self);
    }
    public virtual void Save() { }
    public virtual void Load() { }
}
