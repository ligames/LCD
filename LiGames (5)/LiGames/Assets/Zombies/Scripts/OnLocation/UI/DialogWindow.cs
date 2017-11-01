using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DialogResult
{
    None,
    Yes,
    No
}

public class DialogWindow : MonoBehaviour
{
    [SerializeField]
    Button yesButton, noButton;
    [SerializeField]
    Text messageText, headerText;
    public event System.Action onClick;
    [SerializeField]
    GameObject window;

    public string YesString { get; set; }
    public string NoString { get; set; }
    public string MessageString { get; set; }
    public string HeaderString { get; set; }

    public DialogResult Result { get; private set; }

    Text yesText, noText;

    void Awake()
    {
        yesText = GetButtonText(yesButton);
        yesButton.onClick.AddListener(
            new UnityEngine.Events.UnityAction(() => Result = DialogResult.Yes));
        if (noButton)
        {
            noText = GetButtonText(noButton);
            noButton.onClick.AddListener(
                new UnityEngine.Events.UnityAction(() => Result = DialogResult.No));
        }
    }
    void Close()
    {
        window.SetActive(false);
    }
    public void Show()
    {
        SetText(yesText, YesString);
        SetText(noText, NoString);
        SetText(messageText, MessageString);
        SetText(headerText, HeaderString);

        StartCoroutine(WaitUserResponse());
        window.SetActive(true);
    }
    IEnumerator WaitUserResponse()
    {
        Result = DialogResult.None;
        while (Result == DialogResult.None)
            yield return null;
        if (onClick != null)
            onClick();
        Close();
    }
    Text GetButtonText(Button b)
    {
        return b.GetComponentInChildren<Text>();
    }
    void SetText(Text textHandler, string text)
    {
        if (textHandler)
            textHandler.text = text;
    }
}
