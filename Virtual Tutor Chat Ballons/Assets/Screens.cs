using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screens : MonoBehaviour {

    [SerializeField] GameObject Panel;
    [SerializeField] GameObject Panel2;
    UserInfo.UserData user;

    public static Screens Instance;

    bool already = false;
    string ava;

    DataManager dm;
    WebManager wm;
    // Use this for initialization
    void Start()
    {
        //WebManager.Instance.Get_userId(3);
        //WebManager.Instance.Get_courseId(5);
        //WebManager.Instance.makeConnection();
        Panel.SetActive(false);
        Panel2.SetActive(false);
        GameObject log = GameObject.Find("moodleLogin");
        user = log.GetComponent("UserData") as UserInfo.UserData;
        Instance = this;
        dm = log.GetComponent("DataManager") as DataManager;
        wm = log.GetComponent("WebManager") as WebManager;
        TutorScreen.Instance.InicialSet();
    }

    void Update()
    {
        GameObject log = GameObject.Find("moodleLogin");
        user = log.GetComponent("UserData") as UserInfo.UserData;
        if (dm.everyThingDone && !already && wm.tutorchosen>0)
        {
            chooseScreen();
            already = !already;
        } 
    }

    /// <summary>
    /// Chooses the screen.
    /// </summary>
    public void chooseScreen()
    {
        ava = wm.tutor;
        Debug.Log(ava);
        if (ava != "")
        {
            if (ava == "joao")
            {
                TutorScreen.Instance.SetNAva(1);
            }
            else
            {
                TutorScreen.Instance.SetNAva(2);
            }
            ChangeSet();
        }
        else
        {
            InicialSet();
        }
    }

    /// <summary>
    /// Inicial set for the tutor.
    /// </summary>
    public void InicialSet()
    {
        Panel.SetActive(true);
        Panel2.SetActive(false);
    }

    /// <summary>
    /// Changes the set for the tutor.
    /// </summary>
    public void ChangeSet()
    {
        Panel.SetActive(false);
        Panel2.SetActive(true);
    }
}
