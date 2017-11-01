using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellDescriptor : MonoBehaviour
{
    public event ResourceInfoHandler onClick;
    public Button button;
    public RawImage image;
    public delegate void ResourceInfoHandler(ResourceInfo info);
    ResourceInfo resource;

    void Start()
    {
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(OnButtonClick));
    }
    void OnButtonClick()
    {
        if (onClick == null || resource == null)
            return;
        onClick(resource);
    }
    #region API
    public void Clear()
    {
        image.texture = null;
        resource = null;
    }
    public void MountResourceInfo(ResourceInfo info)
    {
        resource = info;
        image.texture = info.image.texture;
    }
    #endregion
}
