using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UserInfo;

public class TutorScreen : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern bool ItExists(string str);

    [DllImport("__Internal")]
    private static extern void GetAF(string str);

    /*avatars and ballon fields*/
    [SerializeField] Button option7;
    [SerializeField] Text text7;
    [SerializeField] Button option8;
    [SerializeField] Text text8;
    [SerializeField] Button option9;
    [SerializeField] Text text9;
    [SerializeField] Button option10;
    [SerializeField] Text text10;
    [SerializeField] Button optionAsk;
    [SerializeField] Text textAsk;
    [SerializeField] Button optionAF;
    [SerializeField] Text textAF;
    [SerializeField] Button optionPUC;
    [SerializeField] Text textPUC;
    [SerializeField] GameObject table;
    [SerializeField] GameObject table2;
    [SerializeField] Scrollbar scb1;
    [SerializeField] Scrollbar scb2;
    [SerializeField] GameObject ballon;
    Scrollbar scb;
    [SerializeField] Text BallonText;
    [SerializeField] Text InfoText;
    [SerializeField] GameObject AskBox;
    [SerializeField] GameObject InfoBox;
    [SerializeField] GameObject AllButtons;


    /*Avatar components*/
    [SerializeField] private AvatarController doll;
    [SerializeField] private AvatarController doll2;
    [SerializeField] private GameObject dollh;
    [SerializeField] private GameObject dollh2;
    private AvatarController dolly = null;

    [SerializeField] private GameObject screen;

    //Fólios content
    [SerializeField] ContentSizeFitter contentSizeFitter;
    [SerializeField] Transform folioParentPanel;
    [SerializeField] GameObject newFolioPrefab;

    //topics content
    [SerializeField] ContentSizeFitter contentSizeFitterInfo;
    [SerializeField] Transform infoParentPanel;
    [SerializeField] GameObject newInfoPrefab;

    bool greeting = false;

    //names for buttons
    private string efolios = "e-fólios";
    private string docs = "Tópicos";
    private string foruns = "Fóruns";
    private string back = "Voltar";

    //replys for the baloons
    private string giveHelp = "Eu posso ajudar: use os botões em baixo para obter informações sobre os e-folios, os Documentos e as actividades nos Fóruns";

    string mood = "Neutral_mood";
    string message = "";
    public int option = 0;
    public int n_ava = 0;
    private bool hasInfo = false;
    UserInfo.UserData user;
    public static TutorScreen Instance;
    List<UserInfo.Course.Folio> lf;
    List<UserInfo.Course.Topic> tf;
    List<UserInfo.Course.Forum> fo;
    List<UserInfo.Course.Folio> foliosActivities = new List<UserInfo.Course.Folio>();
    List<UserInfo.Course.Topic> topicActivities = new List<UserInfo.Course.Topic>();
    List<UserInfo.Course.Forum> forumActivities = new List<UserInfo.Course.Forum>();
    List<UserInfo.Course.modules> mActivities = new List<UserInfo.Course.modules>();
    static string assiduidade;
    static string avaliacao;
    bool defined = false;
    bool greet = false;
    bool happy = false;
    int count = 0;
    string infoEf = "Por favor, carregue nos botões abaixo para obter informação sobre cada e-fólio.";
    string infoDoc = "Estes são os tópicos disponíveis neste momento. Pode carregar para obter mais informação.";
    string infoForum = "Estes são os foruns ativos neste momento. Pode carregar para obter mais informação.";
    string showEf = "Use o link para o e-fólio escolhido.";
    string showDoc = "O Tópico seleccionado é o que está destacado na página.";
    string showForum = "O Fórum que escolheu é o que está destacado na página.";
    string questionEanswer = "Esta funcionalidade de Pergunta-Resposta (em Inglês, Question & Answering) está ainda em desenvolvimento. Deixe aqui uma pergunta que gostasse de ver respondida.";
    //string questionEanswer = "Escreva em baixo a pergunta que gostaria que respondesse. A funcionalidade ainda está em desenvolvimento podendo não conseguir responder a todas as perguntas.";
    string UC = "Aqui está informação sobre as atividades formativas desta UC.";
    string close = "Obrigado! Até uma próxima!";
    string open = "Bem vindo de volta!";
    string efoliosDone = "Os e-fólios estão todos avaliados! Por favor veja o seu cartão de aprendizagem!";
    string PUC = "Aqui está o Plano da U.C.! Aqui, poderá saber qual o plano da disciplina durante o semestre!";
    string AF = "Aqui está a informação sobre a Actividade Formativa escolhida!";
    public string folioFeedbacK = "";

    /// <summary>
    /// Sets the n ava.
    /// </summary>
    /// <param name="n_ava">The n ava.</param>
    public void SetNAva(int n_ava)
    {
        this.n_ava = n_ava;
    }

    DataManager dm;
    //public WebManager wm;
    private WebManager wm;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        GameObject log = GameObject.Find("moodleLogin");
        user = log.GetComponent("UserData") as UserInfo.UserData;
        dm = log.GetComponent("DataManager") as DataManager;
        wm = log.GetComponent("WebManager") as WebManager;

        InicialSet();

        dollh.SetActive(false);
        dollh2.SetActive(false);
        ballon.SetActive(false);
        hideButtons();

        AllTheMenu();
        BackMenu();

        Instance = this;


        ///tentativa de scale do model
        ///
        ///float height = Camera.main.orthographicSize * 2;
        ///float width = height * Screen.width / Screen.height;
        ///Debug.Log("height "+ height);
        ///Debug.Log("width " + width);
        ///Debug.Log("scale1 " + dollh.transform.localScale);
        ///dollh.transform.localScale = Vector3.one * height / 145f;
        ///Debug.Log("scale2 " + dollh.transform.localScale);
        ///

        //var width = Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height;

        Debug.Log("width: " + Screen.width);
        Debug.Log("height: " + Screen.height);

        //Debug.Log("width " + width);
        Debug.Log("Scale: " + InfoBox.gameObject.transform.localScale);
        Debug.Log("Position: " + InfoBox.gameObject.transform.position);

        ///InfoBox.gameObject.transform.localScale += new Vector3(teste/3000, teste/3000, teste/3000);
        //Debug.Log("Scale2 " + InfoBox.gameObject.transform.localScale);


        /*
         * 429/432
         * 
        */



        if ((Screen.width>1800) && (Screen.height>850))
        {
            BallonText.gameObject.transform.position += new Vector3(705, 400, 0);
            BallonText.gameObject.transform.localScale += new Vector3(0.012f, 0.012f, 0);

            InfoText.gameObject.transform.position += new Vector3(660, 200, 0);
            InfoText.gameObject.transform.localScale += new Vector3(0.012f, 0.012f, 0);

            AllButtons.gameObject.transform.position += new Vector3(750, 50, -1);
            AllButtons.gameObject.transform.localScale += new Vector3(1, 1, 1);
        }
        /*else if((Screen.width < 370) && (Screen.height < 370))
        {
            Debug.Log("EGFSFD");
            BallonText.gameObject.transform.position += new Vector3(150, 400, 0);
            BallonText.gameObject.transform.localScale += new Vector3(0.008f, 0.008f, 0);

            InfoText.gameObject.transform.position += new Vector3(0, -25, 0);
            InfoText.gameObject.transform.localScale += new Vector3(0.0000000000000000000000000001f, 0.0000000000000000000000001f, 0);

            AllButtons.gameObject.transform.position += new Vector3(-25, 0, 0);
            AllButtons.gameObject.transform.localScale += new Vector3(-0.2f, -0.2f, 1);
        }*/



    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    public void Update()
    {
        if (!defined && count == 0 && !greeting && dm.everyThingDone)
        {
            Invoke("replying", 4.0f);
            count = 1;
            option = 1;
        }
        else if (count == 0 && dm.everyThingDone)
        {
            Invoke("replying", 4.0f);
            count = 1;
            option = 1;
        } 
    }

    /// <summary>
    /// Makes the tutor less happy.
    /// </summary>
    public void makeTutorLessHappy()
    {
        string oldMood = mood;
        Debug.Log("here");
        mood = "neutrala";
        invokeMood();
        mood = oldMood;
        invokeMood();
    }

    /// <summary>
    /// Inicials the set.
    /// </summary>
    public void InicialSet()
    {
        screen.SetActive(true);
    }

    /// <summary>
    /// Inicials the set desactivated.
    /// </summary>
    public void InicialSetDesactivated()
    {
        screen.SetActive(false);
    }

    /// <summary>
    /// Gets the activiies.
    /// </summary>
    public void GetActiviies()
    {
        // usado para ver as atividades formativas
        Debug.Log("GetActivities");
        foreach (UserInfo.Course c in dm.getCourses())
        {

            foreach (String s in c.actFormativas.Keys)
            {
                if (c.actFormativas[s].GetType() == typeof(UserInfo.Course.modules))
                {
                    mActivities.Add((UserInfo.Course.modules)c.actFormativas[s]);
                } else if(c.actFormativas[s].GetType() == typeof(UserInfo.Course.Forum))
                {
                    forumActivities.Add((UserInfo.Course.Forum)c.actFormativas[s]);
                } else if (c.actFormativas[s].GetType() == typeof(UserInfo.Course.Folio))
                {
                    foliosActivities.Add((UserInfo.Course.Folio)c.actFormativas[s]);
                }
                else if (c.actFormativas[s].GetType() == typeof(UserInfo.Course.Topic))
                {
                    topicActivities.Add((UserInfo.Course.Topic)c.actFormativas[s]);
                }
                else
                {
                    List<object> obs = c.actFormativas[s] as List<object>;
                    foreach (object o in obs)
                    {
                        mActivities.Add((UserInfo.Course.modules)o);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Defines the doll.
    /// </summary>
    public void defineDoll()
    {
        if (n_ava == 1)
        {
            dolly = doll;
            dollh.SetActive(true);
            dollh2.SetActive(false);
            Debug.Log("João");
        }
        else
        {
            dolly = doll2;
            dollh2.SetActive(true);
            dollh.SetActive(false);
            Debug.Log("Maria");
        }

        defined = !defined;
    }

    /// <summary>
    /// Shows the message.
    /// </summary>
    public void ShowMessage()
    {
        if (message != "")
        {
            ShowBalloon(message);
        }
    }

    /// <summary>
    /// Fills the information lists.
    /// </summary>
    public void fillInfoLists()
    {
        UserInfo.Course c = dm.getCourseById(wm.courseId);
        lf = c.folios;
        tf = c.topics;
        fo = c.forums;
    }

    /// <summary>
    /// Informations the about behaviour.
    /// </summary>
    public void infoAboutBehaviour()
    {
        assiduidade = WebManager.Instance.assid;
        avaliacao = WebManager.Instance.aprov;
        if (avaliacao == "")
        {
            avaliacao = "middle";
        }
        
        defineMood();
        invokeMood();
    }

    /// <summary>
    /// Sets the reply of the tutor.
    /// </summary>
    public void replying()
    {
        Debug.Log("replying to " + option);
        InicialSetDesactivated();
        switch (option)
        {
            case 1:
                //greetings parte 1 e 2
                Debug.Log("Teacher: " + wm.teacher + "; gp2: " + wm.gp2);
                if (wm.gp2)
                {
                    greetingsPart1();
                    if(wm.teacher != 0)
                    {
                        wm.gp2 = !wm.gp2;
                    }
                }
                else //greetings parte 2
                {
                    if (wm.teacher != 0)
                    {
                        greetingsPart2();
                    }
                    else
                    {
                        greetingsPart1();
                    }
                }
                Debug.Log("Primeiro Menu");
                break;
            case 2: //balão e-fólios
                UserInfo.Course co = dm.getCourseById(wm.courseId);
                if (!co.checkGradeReport)
                {
                    responseMessage(infoEf);
                }
                else
                {
                    responseMessage(efoliosDone);
                }
                break;
            case 3: //balão documentos
                responseMessage(infoDoc);
                break;
            case 4: //balão fóruns
                responseMessage(infoForum);
                break;
            case 5: //balão mostrar conteúdos de documentos
                responseMessage(showDoc);
                break;
            case 6: //balão mostrar conteúdos de fóruns
                responseMessage(showForum);
                break;
            case 7: //balão mostrar conteúdos de e-fólios
                responseMessage(folioFeedbacK);
                break;
            case 8: //balão mostrar conteúdos de Q&A
                responseMessage(questionEanswer);
                break;
            case 9:
                responseMessage(QeAFunctions.Instance.GetResult());
                break;
            case 10:
                responseMessage(UC);
                break;
            case 11:
                responseMessage(close);
                break;
            case 12:
                responseMessage(open);
                break;
            case 13:
                responseMessage(PUC);
                break;
            case 14:
                responseMessage(AF);
                break;
            default:
                responseMessage(giveHelp);
                break;
        }
    }

    /// <summary>
    /// Receives all information.
    /// </summary>
    public void receiveAllInformation()
    {
        ballon.SetActive(true);
        Debug.Log("Ballon:  " + ballon.activeInHierarchy);
        defineDoll();
        fillInfoLists();
        GetActiviies();
        infoAboutBehaviour();
    }

    /// <summary>
    /// Greetingses the part1.
    /// </summary>
    public void greetingsPart1()
    {
        Debug.Log("part 1");
        defineDoll();
        responseMessage2("Olá " + user.fullName + "!\n Bem-vind@ a " + dm.getCourseById(wm.courseId).fullName + "!");
        if(wm.teacher != 0)
        {
            Invoke("greetingsPart2", 6.0f);
        }
    }

    /// <summary>
    /// Greetingses the part2.
    /// </summary>
    public void greetingsPart2()
    {
        Debug.Log("part 2");
        if (!hasInfo)
        {
            Debug.Log("retrieve info");
            receiveAllInformation();
            hasInfo = !hasInfo;
        }
        if (!greeting)
        {
            Debug.Log("normal login");
            responseMessage(WebManager.Instance.getFeedback2());
            greeting = !greeting;
        }
        else
        {
            Debug.Log("hello");
            responseMessage(WebManager.Instance.getFeedback2());
        }
    }

    /// <summary>
    /// Shows the mens.
    /// </summary>
    /// <param name="mens">The mens.</param>
    public void responseMessage(string mens)
    {
        message = mens;
        ShowMessage();
        CreateButtons();
    }

    /// <summary>
    /// Shows the mens.
    /// </summary>
    /// <param name="mens">The mens.</param>
    public void responseMessage2(string mens)
    {
        ballon.SetActive(true);
        message = mens;
        ShowMessage();
    }
    
    /// <summary>
    /// Hides the buttons.
    /// </summary>
    public void hideButtons()
    {
        option7.gameObject.SetActive(false);
        option8.gameObject.SetActive(false);
        option9.gameObject.SetActive(false);
        option10.gameObject.SetActive(false);
        optionAsk.gameObject.SetActive(false);
        optionAF.gameObject.SetActive(false);
        optionPUC.gameObject.SetActive(false);
        table.SetActive(false);
        table2.SetActive(false);
        InfoText.gameObject.SetActive(false);
        AskBox.SetActive(false);
        InfoBox.SetActive(false);
    }

    /// <summary>
    /// Updates the news about this instance.
    /// </summary>
    /// <returns>the news about the instance</returns>
    public string updates()
    {
        StringBuilder sb = new StringBuilder("");
        int countFolios = countListNewsF(lf);
        int countForuns = countListNewsFo(fo);
        int countTopic = countListNewsT(tf);

        if (countFolios == 0 && countTopic == 0 && countForuns == 0)
        {
            sb.Append("Não existem novidades na cadeira!\n");
        }
        else
        {
            sb.Append("Tem novidades em:\n");
            if (countForuns != 0)
            {
                sb.Append("- Foruns.\n");
            }
            if (countTopic != 0)
            {
                sb.Append("- Tópicos.\n");
            }
            if (countFolios != 0)
            {
                sb.Append("- e-fólios.\n");
            }
        }
        sb.Append("\nCarregue nos botões para mais informação.");
        return sb.ToString();
    }

    /// <summary>
    /// Counts the news in topics.
    /// </summary>
    /// <param name="tf">Topics.</param>
    /// <returns></returns>
    private int countListNewsT(List<Course.Topic> tf)
    {
        int n = 0; 

        foreach (Course.Topic f in tf)
        {
            if (f.seen == -1)
            {
                n++;
            }
        }

        return n;
    }

    /// <summary>
    /// Counts the news in Foruns.
    /// </summary>
    /// <param name="fo">Foruns.</param>
    /// <returns></returns>
    private int countListNewsFo(List<Course.Forum> fo)
    {
        int n = 0;

        foreach (Course.Forum f in fo)
        {
            if (f.seen == -1)
            {
                n++;
            }
        }

        return n;
    }

    /// <summary>
    /// Counts the news in e-fólios.
    /// </summary>
    /// <param name="lf">e-fólios.</param>
    /// <returns></returns>
    private int countListNewsF(List<Course.Folio> lf)
    {
        int n = 0;

        foreach(Course.Folio f in lf)
        {
            if (f.seen == -1)
            {
                n++;
            }
        }

        return n;
    }

    /// <summary>
    /// Shows the buttons to the user.
    /// </summary>
    public void CreateButtons()
    {
        /*Hide all the buttons*/
        hideButtons();
        //clean all the panels
        foreach (Transform child in infoParentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in folioParentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (dm.everyThingDone) { 
            switch (option)
            {
                case 1: //menu inicial
                    Debug.Log(efolios);
                    InfoText.gameObject.SetActive(true);
                    InfoBox.SetActive(true);
                    InfoText.text = updates();
                    SetAllTheMenuTrue();
                    break;
                case 2: //menu e-fólios
                    Debug.Log(efolios);
                    int fol = 0;
                    table.SetActive(true);
                    UserInfo.Course co = dm.getCourseById(wm.courseId);
                    if (!co.checkGradeReport)
                    {
                        foreach (UserInfo.Course.Folio f in lf)
                        {
                            if (ItExists("module-" + f.cmid))
                            {
                                GameObject response = (GameObject)Instantiate(newFolioPrefab);
                                response.transform.SetParent(folioParentPanel);
                                response.transform.SetSiblingIndex(folioParentPanel.childCount - 2);
                                string displayMessage = co.FolioDisplay(f.cmid);
                                string stateOfE = co.FolioFeedback(f.cmid);
                                string feed = "";
                                if (f.graderaw != -1)
                                {
                                    feed = wm.GetFolioPhrase(stateOfE);
                                }
                                else
                                {
                                    feed = "O link para o e-fólio que escolheu está agora destacado na página da UC.";
                                }
                                response.GetComponent<MessageFuntions>().ShowMessage(f.name, displayMessage);
                                response.GetComponent<MessageFuntions>().GetDataToShow(feed);
                                response.GetComponent<MessageFuntions>().AddFunctionToButton(7, "module-" + f.cmid);
                                string c = (f.seen > 0) ? "branco" : (f.seen == 0) ? "amarelo" : "vermelho";
                                if (c == "vermelho")
                                {
                                    response.GetComponent<MessageFuntions>().itIsNotSeen();
                                }
                                else if (c == "amarelo")
                                {
                                    response.GetComponent<MessageFuntions>().itIsANew();
                                }

                                fol++;
                            }
                        }
                    }
                    if (co.checkGradeReport || fol == 0)
                    {
                        GameObject response = (GameObject)Instantiate(newFolioPrefab);
                        response.transform.SetParent(folioParentPanel);
                        response.transform.SetSiblingIndex(folioParentPanel.childCount - 2);
                        response.GetComponent<MessageFuntions>().ShowMessageM("Cartão de Aprendizagem");
                        response.GetComponent<MessageFuntions>().AddFunctionToButtonCA(7);

                    }

                    scb = scb1;
                    itsDown();
                    SetTheBackMenuTrue();
                    break;
                case 3: //menu documentos
                    //Debug.Log(docs + " : " + tf.Count);
                    table2.SetActive(true);
                    foreach (UserInfo.Course.Topic t in tf)
                    {
                        if (ItExists(t.name))
                        {
                            GameObject response = (GameObject)Instantiate(newInfoPrefab);
                            response.transform.SetParent(infoParentPanel);
                            response.transform.SetSiblingIndex(infoParentPanel.childCount - 2);
                            response.GetComponent<TopicFunctions>().ShowInfo(t.name);
                            response.GetComponent<TopicFunctions>().AddFunctionToButton(5, t.name);
                            //Debug.Log(name);
                            if (t.seen == -1)
                            {
                                response.GetComponent<TopicFunctions>().itIsNotSeen();
                            }
                            else if (t.seen == 0)
                            {
                                response.GetComponent<TopicFunctions>().itIsANew();
                            }
                        }
                    }
                    scb = scb2;
                    itsDown();
                    SetTheBackMenuTrue();
                    break;
                case 4: //menu fóruns
                    //Debug.Log(foruns + " : " + fo.Count);
                    table2.SetActive(true);
                    foreach (UserInfo.Course.Forum f in fo)
                    {
                        if (ItExists("module-" + f.cmid))
                        {
                            GameObject response = (GameObject)Instantiate(newInfoPrefab);
                            response.transform.SetParent(infoParentPanel);
                            response.transform.SetSiblingIndex(infoParentPanel.childCount - 2);
                            response.GetComponent<TopicFunctions>().ShowInfo(f.name);
                            response.GetComponent<TopicFunctions>().AddFunctionToButton(6, "module-" + f.cmid);
                            if (f.seen == -1)
                            {
                                response.GetComponent<TopicFunctions>().itIsNotSeen();
                            }
                            else if (f.seen == 0)
                            {
                                response.GetComponent<TopicFunctions>().itIsANew();
                            }
                        }
                    }
                    scb = scb2;
                    itsDown();
                    SetTheBackMenuTrue();
                    break;
                case 5: //mostrar conteúdos de documentos
                    SetTheBackMenuTrue();
                    break;
                case 6: //mostrar conteúdos de fóruns
                    SetTheBackMenuTrue();
                    break;
                case 7: //mostrar conteúdos de e-fólios
                    SetTheBackMenuTrue();
                    break;
                case 8: //mostrar conteúdos de Q&A
                    //SetTheBackMenuTrue();
                    SetAskMenu();
                    break;
                case 10: //mostrar conteúdos de Actividades Formativas
                    Debug.Log("activities " + mActivities.Count);
                    table.SetActive(true);
                    if (topicActivities.Count != 0 || foliosActivities.Count != 0 || forumActivities.Count != 0 || mActivities.Count != 0)
                    {
                        //topics
                        if (topicActivities.Count != 0)
                        {
                            foreach (UserInfo.Course.Topic t in topicActivities)
                            {
                                GameObject response = (GameObject)Instantiate(newFolioPrefab);
                                response.transform.SetParent(folioParentPanel);
                                response.transform.SetSiblingIndex(folioParentPanel.childCount - 2);
                                response.GetComponent<MessageFuntions>().ShowMessage(" ---- " + t.name + " ---- ");
                            }
                        }
                        //folios
                        if (foliosActivities.Count != 0)
                        {
                            foreach (UserInfo.Course.Folio f in foliosActivities)
                            {
                                GameObject response = (GameObject)Instantiate(newFolioPrefab);
                                response.transform.SetParent(folioParentPanel);
                                response.transform.SetSiblingIndex(folioParentPanel.childCount - 2);
                                response.GetComponent<MessageFuntions>().ShowMessageM(f.name);
                                response.GetComponent<MessageFuntions>().AddFunctionToButton2(14, "module-" + f.cmid);
                            }
                        }
                        //foruns
                        if (forumActivities.Count != 0)
                        {
                            foreach (UserInfo.Course.Forum f in forumActivities)
                            {
                                GameObject response = (GameObject)Instantiate(newFolioPrefab);
                                response.transform.SetParent(folioParentPanel);
                                response.transform.SetSiblingIndex(folioParentPanel.childCount - 2);
                                response.GetComponent<MessageFuntions>().ShowMessageM(f.name);
                                response.GetComponent<MessageFuntions>().AddFunctionToButton2(14, "module-" + f.cmid);
                            }
                        }
                        //modules
                        if (mActivities.Count != 0)
                        {
                            foreach (UserInfo.Course.modules f in mActivities)
                            {
                                GameObject response = (GameObject)Instantiate(newFolioPrefab);
                                response.transform.SetParent(folioParentPanel);
                                response.transform.SetSiblingIndex(folioParentPanel.childCount - 2);
                                response.GetComponent<MessageFuntions>().ShowMessageM(f.name);
                                response.GetComponent<MessageFuntions>().AddFunctionToButton2(14, f.name);
                            }
                        }
                    } else
                    {
                        GameObject response = (GameObject)Instantiate(newFolioPrefab);
                        response.transform.SetParent(folioParentPanel);
                        response.transform.SetSiblingIndex(folioParentPanel.childCount - 2);
                        response.GetComponent<MessageFuntions>().ShowMessage("Não existem Actividades.");
                    }
                    scb = scb1;
                    itsDown();
                    SetTheBackMenuTrue();
                    break;
                case 11:

                    break;
                case 12:
                    Debug.Log("Default");
                    SetAllTheMenuTrue();
                    break;
                case 13:
                    SetTheBackMenuTrue();
                    break;
                case 14:
                    SetTheBackMenuTrue();
                    break;
                default:
                    Debug.Log("Default");
                    SetAllTheMenuTrue();
                    break;
            }
        } else
        {
            Debug.Log("User não pronto para leitura");
        }
    }

    /// <summary>
    /// Sets the feedback for folio.
    /// </summary>
    /// <param name="feedback">The feedback.</param>
    public void setFeedbackForFolio(string feedback)
    {
        folioFeedbacK = feedback;
    }

    /// <summary>
    /// Sets the menu.
    /// </summary>
    public void AllTheMenu()
    {
        text8.text = efolios;
        option8.onClick.AddListener(replyingF);
        text9.text = docs;
        option9.onClick.AddListener(replyingD);
        text10.text = foruns;
        option10.onClick.AddListener(replyingFu);
        textAsk.text = "Q&A";
        optionAsk.onClick.AddListener(replyingQ);
        textAF.text = "Act. Formativas";
        optionAF.onClick.AddListener(replyingAF);
        textPUC.text = "Plano da U.C.";
        optionPUC.onClick.AddListener(replyingPUC);
    }

    /// <summary>
    /// Sets the back menu.
    /// </summary>
    public void BackMenu()
    {
        text7.text = back;
        option7.onClick.AddListener(InitialMenu);
    }

    /// <summary>
    /// Sets the ask menu.
    /// </summary>
    public void SetAskMenu()
    {
        AskBox.SetActive(true);
    }

    /// <summary>
    /// Sets the back menu true.
    /// </summary>
    public void SetTheBackMenuTrue()
    {
        option7.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets all the menu true.
    /// </summary>
    public void SetAllTheMenuTrue()
    {
        option8.gameObject.SetActive(true);
        option9.gameObject.SetActive(true);
        option10.gameObject.SetActive(true);
        optionAsk.gameObject.SetActive(true);
        optionAF.gameObject.SetActive(true);
        optionPUC.gameObject.SetActive(true);
    }

    /// <summary>
    /// Makes the tutor talk.
    /// </summary>
    public void talk()
    {
        string oldMood = mood;
        mood = "talk";
        invokeMood();
        mood = oldMood;
    }

    /// <summary>
    /// Shows the initial menu.
    /// </summary>
    public void InitialMenu()
    {
        option = 1;
        replying();
    }

    /// <summary>
    /// Shows the second menu.
    /// </summary>
    public void replyingF()
    {
        option = 2;
        replying();
    }

    /// <summary>
    /// Shows the third menu.
    /// </summary>
    public void replyingD()
    {
        option = 3;
        replying();
    }

    /// <summary>
    /// Shows the fourth menu.
    /// </summary>
    public void replyingFu()
    {
        option = 4;
        replying();
    }

    /// <summary>
    /// Shows the fifth menu.
    /// </summary>
    public void replyingDW()
    {
        option = 5;
        replying();
    }

    /// <summary>
    /// Shows the six menu.
    /// </summary>
    public void replyingFuW()
    {
        option = 6;
        replying();
    }

    /// <summary>
    /// Shows the eight menu.
    /// </summary>
    public void replyingQ()
    {
        option = 8;
        replying();
    }

    /// <summary>
    /// Shows the ten menu.
    /// </summary>
    public void replyingAF()
    {
        option = 10;
        replying();
    }

    /// <summary>
    /// Shows the PUC.
    /// </summary>
    public void replyingPUC()
    {
        option = 13;
        GetAF("Plano da Unidade Curricular");
        replying();
    }

    /// <summary>
    /// Closes the tutor and shows goodbyes.
    /// </summary>
    public void closing()
    {
        if(wm.teacher == 1)
        {
            option = 11;
            replying();
        } else
        {
            greetingsPart1();
        }
    }

    /// <summary>
    /// Opens the tutor and shows hello.
    /// </summary>
    public void Opening()
    {
        if(wm.teacher == 1) { 
            option = 12;
            replying();
        } else
        {
            greetingsPart1();
        }
    }

    /// <summary>
    /// Shows the balloon message.
    /// </summary>
    /// <param name="menssage">The menssage.</param>
    public void ShowBalloon(string menssage)
    {
        talk();
        BallonText.text = menssage;
    }

    /// <summary>
    /// Scrolls down.
    /// </summary>
    public void itsDown()
    {
        scb.value = 1;
    }

    /// <summary>
    /// Defines the mood of the tutor.
    /// </summary>
    public void defineMood()
    {
        bool qua1 = assiduidade == "low" && avaliacao == "low";
        bool qua2 = assiduidade == "middle" && avaliacao == "low";
        bool qua3 = assiduidade == "high" && avaliacao == "low";
        bool qua4 = assiduidade == "low" && avaliacao == "middle";
        bool qua5 = assiduidade == "middle" && avaliacao == "middle";
        bool qua6 = assiduidade == "high" && avaliacao == "middle";
        bool qua7 = assiduidade == "low" && avaliacao == "high";
        bool qua8 = assiduidade == "middle" && avaliacao == "high";
        bool qua9 = assiduidade == "high" && avaliacao == "high";

        if (qua1)
        {
            mood = "Neutral_mood";
        } else if (qua2)
        {
            mood = "Neutral_mood";
        }
        else if (qua3)
        {
            mood = "Neutral_mood";
        }
        else if (qua4)
        {
            mood = "Neutral_mood";
        }
        else if (qua5)
        {
            mood = "Happy_low";
        }
        else if (qua6)
        {
            mood = "Happy_low";
        }
        else if (qua7)
        {
            mood = "Happy_low";
        }
        else if (qua8)
        {
            mood = "Happy_high";
        }
        else if (qua9)
        {
            mood = "Happy_high";
        }
    }

    /// <summary>
    /// Invokes the mood of the tutor.
    /// </summary>
    public void invokeMood()
    {
        //Debug.Log(mood);
        if (!dolly.hasAnimator())
        {
            dolly.getAnimator();
        }

        if (mood == "Neutral_mood")
        {
            dolly.SetMood(MoodState.NEUTRAL);
        }
        if (mood == "Happy_low")
        {
            dolly.SetMood(MoodState.HAPPY_LOW);
        }
        if (mood == "Happy_high")
        {
            dolly.SetMood(MoodState.HAPPY_HIGH);
        }
        if (mood == "sad_low")
        {
            dolly.SetMood(MoodState.SAD_LOW);
        }
        if (mood == "sad_high")
        {
            dolly.SetMood(MoodState.SAD_HIGH);
        }

        if (mood == "neutral1")
        {
            dolly.expressEmotion(ExpressionState.NEUTRAL);
        }
        if (mood == "neutrala")
        {
            dolly.expressEmotion(ExpressionState.HAPPY_LOW);
        }
        if (mood == "neutralb")
        {
            dolly.expressEmotion(ExpressionState.HAPPY_HIGH);
        }
        if (mood == "neutralc")
        {
            dolly.expressEmotion(ExpressionState.SAD_LOW);
        }
        if (mood == "neutrald")
        {
            dolly.expressEmotion(ExpressionState.SAD_HIGH);
        }
        if (mood == "neutrale")
        {
            dolly.expressEmotion(ExpressionState.ANGER_LOW);
        }
        if (mood == "neutralf")
        {
            dolly.expressEmotion(ExpressionState.ANGER_HIGH);
        }
        if (mood == "neutralg")
        {
            dolly.expressEmotion(ExpressionState.FEAR_LOW);
        }
        if (mood == "neutralh")
        {
            dolly.expressEmotion(ExpressionState.FEAR_HIGH);
        }
        if (mood == "neutrali")
        {
            dolly.expressEmotion(ExpressionState.DISGUST_LOW);
        }
        if (mood == "neutralj")
        {
            dolly.expressEmotion(ExpressionState.DISGUST_HIGH);
        }
        if (mood == "neutralk")
        {
            dolly.expressEmotion(ExpressionState.SURPRISE_LOW);
        }
        if (mood == "neutrall")
        {
            dolly.expressEmotion(ExpressionState.SURPRISE_HIGH);
        }
        if (mood == "agree")
        {
            dolly.expressEmotion(ExpressionState.HEAD_NOD);
        }
        if (mood == "talk")
        {
            dolly.expressEmotion(ExpressionState.VISEMES);
        }

        if (mood == "Happy_high" && !happy)
        {
            InvokeRepeating("makeTutorLessHappy", 18f, 12f);
            happy = !happy;
        }
    }

    /// <summary>
    /// Changes the mood of the tutor.
    /// </summary>
    /// <param name="reply">The reply.</param>
    public void changeMood(string reply)
    {
        this.mood = reply;
    }
}