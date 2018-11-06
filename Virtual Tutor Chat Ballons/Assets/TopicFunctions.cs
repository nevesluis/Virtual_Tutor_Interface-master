using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class TopicFunctions : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [SerializeField] Text textButton;
    [SerializeField] Button InfoButton;
    int reply;
    string dataToShow;

    /// <summary>
    /// Shows the information.
    /// </summary>
    /// <param name="info">The information.</param>
    public void ShowInfo(string info)
    {
        textButton.text = info;
    }

    /// <summary>
    /// Adds the function to button.
    /// </summary>
    /// <param name="reply">The reply.</param>
    /// <param name="dataToShow">The data to show.</param>
    public void AddFunctionToButton(int reply, string dataToShow)
    {
        this.reply = reply;
        this.dataToShow = dataToShow;
        InfoButton.onClick.AddListener(ChangeItAll);
    }

    /// <summary>
    /// Changes it all.
    /// </summary>
    public void ChangeItAll()
    {
        TutorScreen.Instance.option = reply;
        HelloString(dataToShow);
        TutorScreen.Instance.talk();
        TutorScreen.Instance.replying();
    }

    /// <summary>
    /// Its the is a new.
    /// </summary>
    public void itIsANew()
    {
        ColorBlock colors = InfoButton.colors;
        colors.normalColor = Color.yellow;
        InfoButton.colors = colors;
    }

    /// <summary>
    /// Its the is not seen.
    /// </summary>
    public void itIsNotSeen()
    {
        ColorBlock colors = InfoButton.colors;
        colors.normalColor = Color.red;
        InfoButton.colors = colors;
    }

    /// <summary>
    /// Deletes this instance.
    /// </summary>
    public void Delete()
    {
        Destroy(gameObject);
    }
}
