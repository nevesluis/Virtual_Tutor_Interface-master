using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EFoliosBox : MonoBehaviour {

    [SerializeField]
    ContentSizeFitter contentSizeFitter;
    [SerializeField]
    Transform messageParentPanel;
    [SerializeField]
    GameObject newMessagePrefab;

    public static EFoliosBox Instance;

    public void Start()
    {
        Instance = this;
    }

    /// <summary>
    /// New efolio.
    /// </summary>
    /// <param name="name">The name.</param>
    public void NewEfolio(string name)
    {
        GameObject response = (GameObject)Instantiate(newMessagePrefab);
        response.transform.SetParent(messageParentPanel);
        response.transform.SetSiblingIndex(messageParentPanel.childCount - 2);
        response.GetComponent<MessageFuntions>().ShowMessage(name);
    }

    /// <summary>
    /// New efolio.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="status">The status.</param>
    public void NewEfolio(string name, string status)
    {
        GameObject response = (GameObject)Instantiate(newMessagePrefab);
        response.transform.SetParent(messageParentPanel);
        response.transform.SetSiblingIndex(messageParentPanel.childCount - 2);
        response.GetComponent<MessageFuntions>().ShowMessage(name, status);
    }
}