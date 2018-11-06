using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialScreen : MonoBehaviour
{

    UserInfo.UserData user;

    [SerializeField]
    private AvatarController controller;

    [SerializeField]
    private AvatarController controller2;

    [SerializeField]
    Text t;

    [SerializeField]
    Text b1;

    [SerializeField]
    Text b2;

    [SerializeField]
    Button bb1;

    [SerializeField]
    Button bb2;

    int ava = 0;

    DataManager dm;
    WebManager wm;
    // Use this for initialization
    void Start()
    {
        GameObject log = GameObject.Find("moodleLogin");
        user = log.GetComponent("UserData") as UserInfo.UserData;
        dm = log.GetComponent("DataManager") as DataManager;
        wm = log.GetComponent("WebManager") as WebManager;

        b1.text = "João";
        b2.text = "Maria";

        bb1.onClick.AddListener(Change1);
        bb2.onClick.AddListener(Change2);
    }

    // Update is called once per frame
    void Update()
    {
        if (dm.everyThingDone)
        {
            TutorScreen.Instance.InicialSetDesactivated();

            UserInfo.Course c = dm.getCourseById(wm.courseId);

            t.text = "Olá, " + user.fullName + "! Bem vindo à cadeira de " + c.fullName + "!\n Qual destes avatares escolhe para seu tutor nesta UC?";

            controller.SetMood(MoodState.NEUTRAL);

            Invoke("sendControl", 0.9f);
        }
    }

    /// <summary>
    /// Sends the netral state for the tutors.
    /// </summary>
    private void sendControl()
    {
        controller2.SetMood(MoodState.NEUTRAL);
        //controller2.expressEmotion(ExpressionState.HAPPY_LOW);
    }

    /// <summary>
    /// Changes the scene for the Tutor scene and shows João.
    /// </summary>
    public void Change1()
    {
        Debug.Log("Clicou João");
        Screens.Instance.ChangeSet();
        ava = 1;
        Invoke("sendNumber", 0.7f);
    }

    /// <summary>
    /// Changes the scene for the Tutor scene and shows Maria.
    /// </summary>
    public void Change2()
    {
        Debug.Log("Clicou Maria");
        Screens.Instance.ChangeSet();
        ava = 2;
        Invoke("sendNumber", 1.0f);
    }

    /// <summary>
    /// Sends the number of the choosen Tutor.
    /// </summary>
    public void sendNumber()
    {
        WebManager.Instance.chooseTutor(ava);
        TutorScreen.Instance.n_ava = ava;
        TutorScreen.Instance.defineDoll();
        TutorScreen.Instance.replying();
    }
}
