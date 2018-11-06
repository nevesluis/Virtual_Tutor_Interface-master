using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicsFunctions : MonoBehaviour {

    [SerializeField]
    ContentSizeFitter contentSizeFitterInfo;
    [SerializeField]
    Transform infoParentPanel;
    [SerializeField]
    GameObject newInfoPrefab;

    public static TopicsFunctions Instance;
    private string inform = "";

    public void Start()
    {
        Instance = this;
    }

    public void SetInfo(string info)
    {
        inform = info;
    }

    public void ShowInfo()
    {
        GameObject response = (GameObject)Instantiate(newInfoPrefab);
        response.transform.SetParent(infoParentPanel);
        response.transform.SetSiblingIndex(infoParentPanel.childCount - 2);
        response.GetComponent<TopicFunctions>().ShowInfo(inform);
    }
}
