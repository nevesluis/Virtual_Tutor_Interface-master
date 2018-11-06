using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class MessageFuntions : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [DllImport("__Internal")]
    private static extern void GetAF(string str);

    [DllImport("__Internal")]
    private static extern void GoToGradeRep();

    [SerializeField]
    Text text;
    [SerializeField]
    Text textB;
    [SerializeField]
    Button b;
    int reply;
    string dataToShow;
    string data;

    /// <summary>
    /// Shows the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="status">The status.</param>
    public void ShowMessage(string message, string status)
    {
        textB.fontSize = 12;
        text.text = status;
        textB.text = message;
    }

    /// <summary>
    /// Shows the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void ShowMessage(string message)
    {
        text.fontSize = 12;
        text.text = message;
        b.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the message m.
    /// </summary>
    /// <param name="message">The message.</param>
    public void ShowMessageM(string message)
    {
        Debug.Log(message);
        text.gameObject.SetActive(false);
        textB.text = message;
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
        b.onClick.AddListener(ChangeItAll);
    }

    /// <summary>
    /// Adds the function to button2.
    /// </summary>
    /// <param name="reply">The reply.</param>
    /// <param name="dataToShow">The data to show.</param>
    public void AddFunctionToButton2(int reply, string dataToShow)
    {
        textB.fontSize = 12;
        this.reply = reply;
        this.dataToShow = dataToShow;
        b.onClick.AddListener(ChangeItAll2);
    }

    /// <summary>
    /// Changes it all the info.
    /// </summary>
    public void ChangeItAll()
    {
        TutorScreen.Instance.option = reply;
        TutorScreen.Instance.folioFeedbacK = data;
        HelloString(dataToShow);
        TutorScreen.Instance.talk();
        TutorScreen.Instance.replying();
    }

    /// <summary>
    /// Changes it all the info.
    /// </summary>
    public void ChangeItAll2()
    {
        TutorScreen.Instance.option = reply;
        GetAF(dataToShow);
        TutorScreen.Instance.talk();
        TutorScreen.Instance.replying();
    }

    /// <summary>
    /// Its the is a new info.
    /// </summary>
    public void itIsANew()
    {
        ColorBlock colors = b.colors;
        colors.normalColor = Color.yellow;
        b.colors = colors;
    }

    /// <summary>
    /// The new is not seen.
    /// </summary>
    public void itIsNotSeen()
    {
        ColorBlock colors = b.colors;
        colors.normalColor = Color.red;
        b.colors = colors;
    }

    /// <summary>
    /// Hides the message.
    /// </summary>
    public void HideMessage()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Gets the data to show.
    /// </summary>
    /// <param name="data">The data.</param>
    public void GetDataToShow(string data)
    {
        this.data = data;
    }

    /// <summary>
    /// Adds the function to button.
    /// </summary>
    /// <param name="reply">The reply.</param>
    public void AddFunctionToButtonCA(int reply)
    {
        textB.fontSize = 12;
        this.reply = reply;
        b.onClick.AddListener(ChangeItAll3);
    }

    /// <summary>
    /// Changes all the info.
    /// </summary>
    public void ChangeItAll3()
    {
        TutorScreen.Instance.option = reply;
        GoToGradeRep();
        TutorScreen.Instance.talk();
        TutorScreen.Instance.replying();
    }
}
