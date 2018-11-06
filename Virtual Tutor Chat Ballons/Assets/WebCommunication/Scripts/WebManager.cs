
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;

/// <summary>
/// Class that regulates the web requests made by the application with Moodle or the DB
/// </summary>
public class WebManager : MonoBehaviour
{

    [Serializable]
    public class Values
    {
        public List<jsonValues.phrases> phrases;
        public List<jsonValues.tutor> tutor;
        public List<jsonValues.dbValues> dbValues;
        public List<jsonValues.modulesViewed> modules;
        public List<jsonValues.questions> questions;
    }
    private Values v;
    //DB
    public Boolean success;
    public string tutor = null;
    public int tutorchosen = 0;
    private databaseConnections dbCons;
    private WebserviceLogin login;

    public Boolean gp2 = true; // greeting part 2

    public DataManager manager;

    public Boolean seeModules = false;

    public static WebManager Instance;
    public String assid = null, aprov = null;
    public ArrayList feedbackE = new ArrayList();
    public ArrayList feedbackf = new ArrayList();
    public ArrayList greetingspart2 = new ArrayList();
    public ArrayList folioUpdatef = new ArrayList();
    public ArrayList finalPhrases = new ArrayList();
    public int dayslastlogin; // intervalo desde o ultimo login;
    private String feedbackPhrase;
    private String greetingsPhrase;

    public int loginImportance = 65;
    public int postImportance = 35;
    public int assidThresholdmax = 75;
    public int assidThresholdmin = 50;

    private static jsonValues.dbValues teste;


    public class folioFeedback
    {
        public String frase;
        public String aprov;

        public folioFeedback(String frase, String aprov)
        {
            this.frase = frase;
            this.aprov = aprov;
        }
    }

    private List<String> requestsMade = new List<string>();

    // Use this for initialization
    void Start()
    {
        GameObject moodleLogin = GameObject.Find("moodleLogin");
        login = moodleLogin.AddComponent(typeof(WebserviceLogin)) as WebserviceLogin;
        dbCons = moodleLogin.AddComponent(typeof(databaseConnections)) as databaseConnections;
        Instance = this;
        //startConnectionWithId(3, 5);

        //Debug.Log(dbCons.databaseConnection());
        //conID();
        //makeConnection();
        connectT();

        //chama a funcao callcoroutineperformance
        //callcoroutineperformance();
    }

    public void connectT()
    {
        Get_courseId(11);
        Get_userId(21);
        Get_t(1);
        changeLocation("http://ec2-34-240-43-90.eu-west-1.compute.amazonaws.com/moodleFCUL");
        makeConnection();
    }
    int i = 0;
    DateTime last = DateTime.UtcNow;

    //void setInfo()
    //{
    //    WebserviceLogin teste = new WebserviceLogin();
    //    teste.Teste();
    //}






    // Update is called once per frame
    void Update()
    {
        //teste = WebserviceLogin.teste;

#if UNITY_WEBGL

        if (manager.gotCourses && i == 0)
        {
            i++;
            checkLoginsDB();
            getParameters();
            getTutor();
            getPosts();
            getPerformance();

        }

        //Debug.Log("I:----------"+i);
        if (manager.getUser().readyForRead && manager.dataDone && manager.getCourseById(courseId).postsRetrieved &&
            login.userVerified && manager.getCourseById(courseId).parameters != null &&
            !(requestsMade.Contains("getLogins")) && i == 1) // a escrita do aluno foi completa e foi autenticado
        {

            insertLogin();
            i++;
            UnityEngine.Debug.Log("GP2- " + gp2);
            // usado para ver as atividades formativas

            //foreach (UserInfo.Course c in manager.getCourses())
            //{
            //    foreach (String s in c.actFormativas.Keys)
            //    {
            //        Debug.Log("--------------- " + s);
            //        if (c.actFormativas[s].GetType() == typeof(UserInfo.Course.modules))
            //        {
            //            Debug.Log((c.actFormativas[s] as UserInfo.Course.modules).name);
            //        }
            //        else
            //        {
            //            List<object> obs = c.actFormativas[s] as List<object>;
            //            foreach (object o in obs)
            //            {
            //                Debug.Log((o as UserInfo.Course.modules).name);
            //            }
            //        }
            //        Debug.Log("---------------");
            //    }
            //}

            determineCoursePerformance();
            //if (manager.updatesDone)
            //{
            //    i++;
            //    seeModulesViewed();
            //}
        }

        if(i==2 && manager.updatesDone && !seeModules)
        {
            i++;
            seeModulesViewed();
        }



#else
        if (manager.getUser().readyForRead && i == 0 && login.userVerified) // a escrita do aluno foi completa e foi autenticado
        {

            getParameters();
            checkLoginsDB();
            getPerformance();
            //insertQuestion("Why?");
            seeModulesViewed();
            i++;

            foreach(UserInfo.Course c in manager.getCourses())
            {
                Debug.Log(c.fullName);
            }
        }
#endif


        if (requestsMade.Count > 0)
        {
            TimeSpan sp = DateTime.UtcNow - last;
            if (sp.TotalSeconds > 1)
            {
                last = DateTime.UtcNow;
                String[] copyR = new String[requestsMade.Count]; requestsMade.CopyTo(copyR);
                foreach (String s in copyR)
                {
                    if (dbCons.hashtable.ContainsKey(s))
                    {
                        if (s.Contains("getLogins"))
                        {

                            List<jsonValues.logins> logins = dbCons.hashtable[s] as List<jsonValues.logins>;
                            foreach (jsonValues.logins l in logins)
                            {
                                manager.getCourseById(l.course).logins.Add(l.login);
                            }


                        }
                        if (s.Contains("getPosts"))
                        {
                            if (Int32.TryParse(dbCons.hashtable[s] as String, out manager.getCourseById(courseId).nPostsDone))
                            {
                                //Debug.Log("Posts MADE SUcess: " + manager.getCourseById(courseId).nPostsDone);
                                manager.getCourseById(courseId).postsRetrieved = true;
                            }
                            //else
                            //    Debug.Log("something went wrong with posts: " + dbCons.hashtable[s] as String);
                        }

                        else if (s.Equals("getphrases")) // check error
                        {

                            Values v = JsonUtility.FromJson<Values>("{\"phrases\":" + dbCons.hashtable[s] + "}");

                            feedbackPhrase = filterText(v.phrases);
                            //Debug.Log(greetingsPhrase);
                            //Debug.Log(feedbackPhrase);
                        }


                        

                        if (s.Contains("getparameters"))
                        {
                            
                            String values = "{\"dbValues\":" + dbCons.hashtable[s] + "}";

                            Values v = JsonUtility.FromJson<Values>("{\"dbValues\":" + dbCons.hashtable[s] + "}");

                            Debug.Log("E isto: " + dbCons.hashtable[s]);

                            if (v != null)
                            {

                                foreach (jsonValues.dbValues dbv in v.dbValues)
                                {
                                    Debug.Log(dbv.toString());
                                    Debug.Log("----------------------------------------" + dbv.courseId);
                                    manager.getCourseById(dbv.courseId).parameters = dbv;
                                }
                            }
                        }
                        


                        if (s.Contains("getquestions"))
                        {
                            Values v = JsonUtility.FromJson<Values>("{\"questions\":" + dbCons.hashtable[s] + "}");
                            Debug.Log("QUESTIONS: " + v.questions.Count);

                        }
                        if (s.Contains("gettutor")) // DEDICADO AO WEBGL
                        {
                            // ver o valor escolhido, se nao houve fazer fase de escolha de tutor
                            try
                            {

                                Values v = JsonUtility.FromJson<Values>(dbCons.hashtable[s].ToString());

                                if (v.tutor[0].tutorid == 0)
                                {
                                    tutor = "";
                                    // Proceder para a escolha do tutor
                                }
                                else
                                {
                                    //load TUTOR
                                    //Debug.Log("Carregar o tutor " + ((v.tutor[0].tutorid==1)? "João" : "Maria"));
                                    tutor = ((v.tutor[0].tutorid == 1) ? "joao" : "Maria");
                                }
                            }
                            catch
                            {
                                Debug.Log("NÃO Há ESCOLHA");
                                // Proceder para a escolha do tutor
                                tutor = "";
                            }
                            tutorchosen++;
                            //Debug.Log("Final tutor: " + tutor);
                        }

                        if (s.Contains("getmodulesviewed"))
                        {

                            StringBuilder por = new StringBuilder();
                            por.Append("{\"modules\":");
                            por.Append(dbCons.hashtable[s] + "}");

                            List<jsonValues.modulesViewed> m = JsonUtility.FromJson<Values>(por.ToString()).modules;
                            DateTime time;
                            foreach (jsonValues.modulesViewed mod in m)
                            {
                                time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

                                time = time.AddSeconds(mod.timecreated).ToLocalTime();
                                //if (mod.objecttable.Equals("forum"))
                                //{
                                //    Debug.Log("As "+time.ToLocalTime()+" Foi visto o Forum: " + manager.getCourseById(mod.courseid).getForum(mod.contextinstanceid).name);
                                //}
                                //else if (mod.objecttable.Equals("resource"))
                                //{
                                //    Debug.Log("As " + time.ToLocalTime() + " Foi visto o recurso: " + manager.getCourseById(mod.courseid).getModuleById(mod.contextinstanceid).name);
                                //}
                                //else if (mod.objecttable.Equals("page"))
                                //{
                                //    Debug.Log("As " + time.ToLocalTime() + " Foi visto a pagina: " + manager.getCourseById(mod.courseid).getModuleById(mod.contextinstanceid).name);
                                //}
                                //else if (mod.objecttable.Equals("book"))
                                //{
                                //    Debug.Log("As " + time.ToLocalTime() + " Foi visto o livro: " + manager.getCourseById(mod.courseid).getModuleById(mod.contextinstanceid).name);
                                //}
                                //else if (mod.component.Equals("mod_assign"))
                                //{
                                //    Debug.Log("As " + time.ToLocalTime() + " Foi visto o fólio: " + manager.getCourseById(mod.courseid).getFolio(mod.contextinstanceid).name);
                                //}

                                manager.getCourseById(mod.courseid).compareUpdates(mod);

                            }

                            foreach (UserInfo.Course c in manager.getCourses())
                            {
                                c.newsAttempt = true;
                            }
                            
                            //foreach (UserInfo.Course c in manager.getCourses())
                            //{
                            //    foreach (UserInfo.Course.Topic t in c.topics)
                            //    {
                            //        foreach (UserInfo.Course.modules module in t.modules)
                            //        {
                            //            Debug.Log("MODULE: " + module.name + " " + ((module.seen > 0) ? "branco" : (module.seen == 0) ? "amarelo" : "vermelho"));
                            //        }
                            //    }
                            //    foreach (UserInfo.Course.Forum f in c.forums)
                            //    {
                            //        Debug.Log("Forum: " + f.name + " " + ((f.seen > 0) ? "branco" : (f.seen == 0) ? "amarelo" : "vermelho"));
                            //    }
                            //    foreach (UserInfo.Course.Folio f in c.folios)
                            //    {
                            //        Debug.Log("Folio: " + f.name + " " + ((f.seen > 0) ? "branco" : (f.seen == 0) ? "amarelo" : "vermelho"));
                            //    }
                            //}

                        }

                        requestsMade.Remove(s);
                        dbCons.hashtable.Remove(s);
                    }
                }
            }
        }
        

    }

    /// <summary>
    /// Reverses the multi value.
    /// </summary>
    public void multiCourseSelection()
    {
        login.selectionMulti();
    }
    /*
    /// <summary>
    /// Callcoroutineperformances the specified course identifier.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    public void callcoroutineperformance()
    {
        //login.Teste(courseId);
        login.Teste(courseId);
    }
    */

    public void determineCoursePerformance()
    {
        Decimal assidValue = 0;
        Decimal postValue = 0;
        if(manager.getCourseById(courseId).parameters.login_importance == 0 && manager.getCourseById(courseId).parameters.post_importance == 0)
        {
            manager.getCourseById(courseId).parameters.login_importance = 100;
            manager.getCourseById(courseId).parameters.post_importance = 0;
        }
        if (manager.getCourseById(courseId).parameters.assid_low == 0 && manager.getCourseById(courseId).parameters.assid_high == 0)
        {
            manager.getCourseById(courseId).parameters.assid_low = assidThresholdmin;
            manager.getCourseById(courseId).parameters.assid_high = assidThresholdmax;
        }
        //Debug.Log("CALCULOS");
        //foreach (int cId in manager.getCourseIds())
        //{
        // Calculo da assiduidade
        if (manager.getCourseById(courseId).logins.Count != 0) // tem de haver logins para haver
        {
            manager.getCourseById(courseId).getAverageLoginSpace();
            Decimal loginI = manager.getCourseById(courseId).averageLoginSpace; // valor de 0 a 1
                                                                                //Debug.Log(loginI);
                                                                                // fazer calculos para obter frases
            if (manager.getCourseById(courseId).averageLoginSpace > 0)
            {
                //Debug.Log("1: " + loginI);
                //Debug.Log("low:" + manager.getCourseById(courseId).parameters.login_low);
                Debug.Log("Posts: " + manager.getCourseById(courseId).nPostsDone);
                if (manager.getCourseById(courseId).parameters.login_high != 0 && manager.getCourseById(courseId).parameters.login_low != 0)
                {
                    if (loginI <= Decimal.Divide(1, manager.getCourseById(courseId).parameters.login_high)) // faz menos logins que o threshold high
                        loginI = 0; // tem valor baixo
                    else if (loginI >= Decimal.Divide(1, manager.getCourseById(courseId).parameters.login_low)) // faz mais logins que o threshold low
                        loginI = 1;
                    else
                        loginI = Math.Max(0, Decimal.Divide(loginI, Decimal.Divide(1, manager.getCourseById(courseId).parameters.login_low)));
                }
                else
                    loginI = 1;

                if (manager.getCourseById(courseId).nPostsDone >= manager.getCourseById(courseId).parameters.post_high)
                    postValue = 1;
                else
                    postValue = Decimal.Divide(manager.getCourseById(courseId).nPostsDone, Math.Max(manager.getCourseById(courseId).nPostsDone, manager.getCourseById(courseId).parameters.post_high));
                //Debug.Log(loginI);
                assidValue = ((manager.getCourseById(courseId).parameters.login_importance * loginI) + (manager.getCourseById(courseId).parameters.post_importance * postValue));
                //Debug.Log("2: " + postValue);
                Debug.Log("Assid Value:" + assidValue);
                assid = assidValue >= manager.getCourseById(courseId).parameters.assid_high ? "high" : assidValue >= manager.getCourseById(courseId).parameters.assid_low ? "middle" : "low";

                //assid = (loginI <= manager.getCourseById(courseId).parameters.login_high) ? "middle" : "low";
                //Debug.Log(assid);
                //Debug.Log(manager.getCourseById(courseId).currentAprov + "/" + manager.getCourseById(courseId).maxCurrentAprov);
            }

        }

        //Debug.Log("CALCULOS");
        // calculo do aproveitamento

        manager.getCourseById(courseId).maxCurrentAproveitamento();
        //Debug.Log("maxCurrentAprov " + manager.getCourseById(courseId).maxCurrentAprov);
        if (manager.getCourseById(courseId).maxCurrentAprov != 0)
        {
            int aprovV = (((manager.getCourseById(courseId).currentAprov * 100) / manager.getCourseById(courseId).maxCurrentAprov));
            Debug.Log("Aprov Value: " + aprovV);
            if (aprovV >= manager.getCourseById(courseId).parameters.aprov_high)
                aprov = "high";
            else
                aprov = (aprovV >= manager.getCourseById(courseId).parameters.aprov_low) ? "middle" : "low";
            //Debug.Log(aprov);
        }

        //}


        if (aprov == "")
            aprov = "middle";
        if (assid == "")
            assid = "middle";

        if (aprov != null && assid != null)
        {
            Debug.Log("aprov:" + aprov);
            Debug.Log("assid:" + assid);
            getPhrases(aprov, assid);
            putPerformance();
        }
    }


    // COMUNICACAO COM O MOODLE
    /// <summary>
    /// Starts the connection procedure. Different procedures occur when running on Android or other.
    /// Requires that the parameters have been supplied previously
    /// </summary>
    public void makeConnection()
    {

        Dictionary<String, object> values = new Dictionary<string, object>();
#if UNITY_ANDROID
  
        if(!login.multi)
            login.selectionMulti();
        values.Add("username", userName);
        values.Add("password", password); 
        login.beginConnection(values);
        
#else
        Debug.Log("OTHER");
        if (login.multi)
        {
            login.selectionMulti();
        }

        //        Debug.Log("Values: " + userId + ", " + courseId);
        values.Add("userId", userId);
        values.Add("courseId", courseId);
        if (userId != 0 && courseId != 0)
        {
            login.beginConnection(values);
        }
#endif
    }


    // COMUNICACAO COM A BD

    /// <summary>
    /// Inserts the login in the database if there wasn't a login previously in that day.
    /// </summary>
    public void insertLogin()
    {
        //DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

        //time = time.AddSeconds(mod.timecreated).ToLocalTime();
        TimeSpan s = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        int seconds = Convert.ToInt32(s.TotalSeconds);
        foreach (UserInfo.Course c in manager.getCourses())
        {

            manager.getCourseById(c.id).logins.Sort();

            if (manager.getCourseById(c.id).logins.Count > 0)
            {

                dayslastlogin = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(manager.getCourseById(c.id).logins[manager.getCourseById(c.id).logins.Count - 1]).Day) - (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).Day);
                
                
                if (dayslastlogin != 0)
                { // Caso ainda não tenha vindo hoje
                    
                    login.startUpdateCheck(manager.getCourseById(c.id).logins[Math.Max(0, (manager.getCourseById(c.id).logins.Count - 1))]);
                    manager.getCourseById(c.id).logins.Add(seconds);
                    StartCoroutine(dbCons.insertLogin(c.id));
                }
                else
                {
                    login.startUpdateCheck(manager.getCourseById(c.id).logins[Math.Max(0, (manager.getCourseById(c.id).logins.Count - 2))]);
                    gp2 = false;
                }

            }
            else
            {
                login.startUpdateCheck(0);
                StartCoroutine(dbCons.insertLogin(c.id));
                manager.getCourseById(c.id).logins.Add(seconds);
            }
            manager.getCourseById(c.id).logins.Sort();
            manager.getCourseById(c.id).getAverageLoginSpace();
            Debug.Log("CADEIRA: " + manager.getCourseById(c.id).fullName + " nlogins: " + manager.getCourseById(c.id).logins.Count);
        }
        requestsMade.Add("insertLogin");
    }

    /// <summary>
    /// Requests the logins to the database
    /// </summary>
    public void checkLoginsDB()
    {
        List<int> courseidList = new List<int>();
        foreach (UserInfo.Course c in manager.getCourses()) 
        {

            courseidList.Add(c.id);
        }
        dbCons.getLogins(courseidList);
        requestsMade.Add("getLogins");

    }

    /// <summary>
    /// Gets the phrases from the database.
    /// </summary>
    /// <param name="aproveitamento">The aproveitamento of the student.</param>
    /// <param name="assiduidade">The assiduidade of the student.</param>
    public void getPhrases(String aproveitamento, String assiduidade)
    {
        String filename = "phrases.php";
        Hashtable parameters = new Hashtable();
        parameters.Add("function", "getphrases");
        requestsMade.Add(parameters["function"].ToString());
        //parameters.Add("iden",new String[] {"f"});
        parameters.Add("aprov", aproveitamento);
        parameters.Add("assid", assiduidade);
        dbCons.prepareRequest(filename, parameters);
    }

    /// <summary>
    /// Requests the tutor defined by the student.
    /// </summary>
    public void getTutor()
    {
        String filename = "tutor.php";
        Hashtable parameters = new Hashtable();
        parameters.Add("function", "gettutor");
        requestsMade.Add(parameters["function"].ToString());

        parameters.Add("courseid", courseId);
        parameters.Add("userid", userId);

        dbCons.prepareRequest(filename, parameters);
    }

    /// <summary>
    /// Inserts in the DB the selection of the tutor.
    /// </summary>
    /// <param name="tutorid">Identification of the tutor.</param>
    public void chooseTutor(int tutorid)
    {
        String filename = "tutor.php";
        Hashtable parameters = new Hashtable();
        parameters.Add("function", "selecttutor");
        requestsMade.Add(parameters["function"].ToString());

        parameters.Add("courseid", courseId);
        parameters.Add("userid", userId);
        parameters.Add("tutorid", tutorid);
        dbCons.prepareRequest(filename, parameters);
    }

    /// <summary>
    /// Sees the modules viewed.
    /// </summary>
    public void seeModulesViewed()
    {
        seeModules = true;
        String filename = "logs.php";
        Hashtable parameters = new Hashtable();
        parameters.Add("function", "getmodulesviewed");
        requestsMade.Add(parameters["function"].ToString());
        parameters.Add("secret", true);
#if UNITY_WEBGL
        parameters.Add("courseid", courseId);
#else
        parameters.Add("courseid[]", manager.getCourseIds());
#endif
        parameters.Add("userid", userId);
        //parameters.Add("timecreated", manager.getUser().datelast);

        dbCons.prepareRequest(filename, parameters);
    }

    /// <summary>
    /// Requests the parameters defined by the Teacher in the BackOffice application
    /// </summary>
    public void getParameters()
    {
        String filename = "parameters.php";
        Hashtable parameters = new Hashtable();
        parameters.Add("function", "getparameters");
        requestsMade.Add(parameters["function"].ToString());

#if UNITY_WEBGL
        parameters.Add("courseid", courseId);
#else
        parameters.Add("courseid[]", manager.getCourseIds());
#endif

        dbCons.prepareRequest(filename, parameters);
    }

    /// <summary>
    /// Inserts the question in the DB.
    /// </summary>
    /// <param name="question">The question made by the Student.</param>
    public void insertQuestion(String question)
    {
        String filename = "questions.php";

        Hashtable parameters = new Hashtable();
        parameters.Add("function", "putquestions");
        parameters.Add("userid", userId);
        parameters.Add("courseid", courseId);
        parameters.Add("question", question);
        TimeSpan s = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        int seconds = Convert.ToInt32(s.TotalSeconds);
        parameters.Add("time", seconds);
        requestsMade.Add(parameters["function"].ToString());
        Debug.Log("---------------------------------" + parameters + "-----------------------------------");
        dbCons.prepareRequest(filename, parameters);
    }

    /// <summary>
    /// Gets the performance of the Student in the desired Courses.
    /// </summary>
    public void getPerformance()
    {
        String filename = "student_performance.php";
        Hashtable parameters = new Hashtable();
        parameters.Add("function", "getperformance");
        requestsMade.Add(parameters["function"].ToString());
#if UNITY_WEBGL 
        parameters.Add("courseid", courseId);
#else
        parameters.Add("courseid[]", manager.getCourseIds());
#endif

        parameters.Add("studentid", userId);

        dbCons.prepareRequest(filename, parameters);

    }

    public void getPosts()
    {
        String filename = "logs.php";
        Hashtable parameters = new Hashtable();
        parameters.Add("function", "getPosts");
        requestsMade.Add(parameters["function"].ToString());
        parameters.Add("secret", true);
#if UNITY_WEBGL
        parameters.Add("courseid", courseId);
#else
        parameters.Add("courseid[]", manager.getCourseIds());
#endif
        parameters.Add("userid", userId);
        //parameters.Add("timecreated", manager.getUser().datelast);

        dbCons.prepareRequest(filename, parameters);
    }

    /// <summary>
    /// Inserts new data regarding the performance of the student in the DB.
    /// </summary>
    public void putPerformance()
    {

#if UNITY_ANDROID
        TimeSpan s = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        int seconds = Convert.ToInt32(s.TotalSeconds);
        List<Hashtable> param = new List<Hashtable>();
        String filename = "student_performance.php";
        foreach (UserInfo.Course c in manager.getCourses())
        {
            manager.getCourseById(courseId).getAverageLoginSpace();
            if ((new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(manager.getCourseById(c.id).logins[manager.getCourseById(c.id).logins.Count - 1]).Day) != (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).Day))
            { // TEMPORARIO LIMITA A UM POR DIA
                
                Hashtable parameters = new Hashtable();
                parameters.Add("function", "putperformance");
                //requestsMade.Add(parameters["function"].ToString());

                parameters.Add("courseid", c.id);
                parameters.Add("studentid", userId);
                parameters.Add("aproveitamento", ((manager.getCourseById(c.id).currentAprov * 100) / manager.getCourseById(c.id).maxCurrentAprov));
                parameters.Add("assiduidade", manager.getCourseById(courseId).averageLoginSpace);
                parameters.Add("time", seconds);
                param.Add(parameters);
            }
        }
        if(param.Count>0)
        dbCons.prepareRequests(filename, param);
#else
        TimeSpan s = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        int seconds = Convert.ToInt32(s.TotalSeconds);
        foreach (UserInfo.Course c in manager.getCourses())
        {
            manager.getCourseById(courseId).getAverageLoginSpace();
            if (manager.getCourseById(c.id).logins.Count > 0)
            {
                if (dayslastlogin != 0)
                { // TEMPORARIO LIMITA A UM POR DIA
                    String filename = "student_performance.php";
                    Hashtable parameters = new Hashtable();
                    parameters.Add("function", "putperformance");
                    //requestsMade.Add(parameters["function"].ToString());

                    parameters.Add("courseid", c.id);
                    parameters.Add("studentid", userId);
                    if (manager.getCourseById(c.id).maxCurrentAprov > 0)
                        parameters.Add("aproveitamento", ((manager.getCourseById(c.id).currentAprov * 100) / manager.getCourseById(c.id).maxCurrentAprov));
                    else
                        parameters.Add("aproveitamento", 0);
                    parameters.Add("assiduidade", manager.getCourseById(courseId).averageLoginSpace);
                    parameters.Add("time", seconds);

                    dbCons.prepareRequest(filename, parameters);
                }
            }
            else
            {
                String filename = "student_performance.php";
                Hashtable parameters = new Hashtable();
                parameters.Add("function", "putperformance");
                //requestsMade.Add(parameters["function"].ToString());

                parameters.Add("courseid", c.id);
                parameters.Add("studentid", userId);
                if (manager.getCourseById(c.id).maxCurrentAprov > 0)
                    parameters.Add("aproveitamento", ((manager.getCourseById(c.id).currentAprov * 100) / manager.getCourseById(c.id).maxCurrentAprov));
                else
                    parameters.Add("aproveitamento", 0);
                parameters.Add("assiduidade", manager.getCourseById(courseId).averageLoginSpace);
                parameters.Add("time", seconds);

                dbCons.prepareRequest(filename, parameters);
            }
        }

#endif
    }


    /// <summary>
    /// Generic method that requests a certain webservice in the file using the parameters as definers
    /// </summary>
    /// <param name="filename">The name of the file that contains the webservice.</param>
    /// <param name="parameters">The parameters supplied to the webservice.</param>
    public void requestDB(String filename, Hashtable parameters)
    {
        requestsMade.Add(parameters["function"].ToString());
        dbCons.prepareRequest(filename, parameters);
    }


    /// <summary>
    /// Filters the phrases supplied by the DB.
    /// </summary>
    /// <param name="p">Struture that contains the phrases, for more information check jsonValues.</param>
    /// <returns>The constructed feedback phrase.</returns>
    public String filterText(List<jsonValues.phrases> p)
    {
        System.Random n = new System.Random();
        feedbackE = new ArrayList();
        feedbackf = new ArrayList();
        greetingspart2 = new ArrayList();
        String np;
        foreach (jsonValues.phrases i in p)
        {
            np = Regex.Unescape(@i.frase);
            //Debug.Log(np); 
            if (i.identifier.Contains("fe"))
            {
                feedbackE.Add(np);
            }

            if (i.identifier.Contains("ff"))
                feedbackf.Add(np);

            if (i.identifier.Contains("pf"))
            {
                folioUpdatef.Add(new folioFeedback(np, i.aproveitamento.ToString()));
            }

            if (i.identifier.Contains("gp2"))
                greetingspart2.Add(np);

            if (i.identifier.Contains("fin"))
                finalPhrases.Add(np);
        }
        StringBuilder sb = new StringBuilder();
        if (manager.getCourseById(courseId).maxCurrentAprov != 0)
        {
            if (feedbackE.Count > 0)
                sb.Append(feedbackE[n.Next(0, feedbackE.Count)]);
            sb.Append(" ");
        }
        String fed = feedbackf[n.Next(0, feedbackf.Count)] as String;


        if (feedbackf.Count > 0)
            sb.Append(fed);
        if (greetingspart2.Count > 0)
            greetingsPhrase = greetingspart2[n.Next(0, greetingspart2.Count)] as String;

        //greetingsPhrase = Regex.Replace(greetingsPhrase, "@dayslastlogin", dayslastlogin.ToString());
        String si = sb.ToString();
        if (manager.getUser().fullName != null)
            if (!manager.getUser().fullName.Equals(""))
                si = Regex.Replace(sb.ToString(), "@username", manager.getUser().fullName);
        else if (manager.getUser().userName != null)
            if(!manager.getUser().userName.Equals(""))
                si = Regex.Replace(sb.ToString(), "@username", manager.getUser().userName);
       
        // adicionar filtros adicionais se necessários

        return si;
    }

    // METODOS PARA IR BUSCAR VALORES DADOS PELO USER

    public int userId = 0;
    public String userName = "";
    public int courseId = 0;
    public String password = "";
    public int teacher = 0;

    /// <summary>
    /// Gets the name of the Student. (Android version)
    /// </summary>
    /// <param name="name">The name of the Student.</param>
    public void getUserName(String name)
    {
        userName = name;
    }


    /// <summary>
    /// Gets the user identifier. (WebGl version)
    /// </summary>
    /// <param name="id">The identifier of the Student.</param>
    public void Get_userId(int id)
    {
        userId = id;
    }

    /// <summary>
    /// Gets the course identifier. (WebGl version)
    /// </summary>
    /// <param name="id">The identifier of the Course.</param>
    public void Get_courseId(int id)
    {
        courseId = id;
    }

    /// <summary>
    /// Gets the course identifier. (WebGl version)
    /// </summary>
    /// <param name="id">The identifier of the Course.</param>
    public void Get_t(int id)
    {
        teacher = id;
    }

    /// <summary>
    /// Gets the password. (Android version)
    /// </summary>
    /// <param name="Password">The password of the Student.</param>
    public void getPassword(String Password)
    {
        password = Password;
    }

    /// <summary>
    /// Gets the phrase.
    /// </summary>
    /// <returns>The feedback defined phrase.</returns>
    public String GetPhrase()
    {
        return feedbackPhrase;
    }

    /// <summary>
    /// Gets the greeting phrase.
    /// </summary>
    /// <returns>The defined greetings phrase.</returns>
    public String getGreetingPhrase()
    {
        return greetingsPhrase;
    }

    public string GetFolioPhrase(string state)
    {
        int n = 0;
        if (state.Equals("LOW"))
        {
            n = UnityEngine.Random.Range(0, 1);
        }
        else if (state.Equals("MIDDLE"))
        {
            n = UnityEngine.Random.Range(2, 3);
        }
        else
        {
            n = UnityEngine.Random.Range(4, 5);
        }

        int i = 0;
        foreach (folioFeedback f in folioUpdatef)
        {
            if (f.aprov.Equals(state) && i == n)
            {
                return f.frase;
            }
            i++;
        }
        return "";
    }

    public string getFeedback2()
    {
        int sizef = feedbackE.Count;
        int sizef1 = feedbackf.Count;
        string feede = getFromList(feedbackE, UnityEngine.Random.Range(0, sizef));
        string feedf = getFromList(feedbackf, UnityEngine.Random.Range(0, sizef1));
        if (manager.getCourseById(courseId).maxCurrentAprov == 0)
        {
            feede = "";
        }
        if (manager.getCourseById(courseId).checkGradeReport)
        {
            feedf = "Caso tenha alguma questão contacte o professor desta UC.";
        }

        return feede + " " + feedf;
    }

    private string getFromList(ArrayList l, int place)
    {
        int i = 0;

        foreach (string f in l)
        {
            if (i == place)
            {
                return f;
            }
            i++;
        }
        return "";
    }

    public void changeLocation(String url)
    {
        login.moodleUrl = url;
        //login.manager = (DataManager)Resources.Load("/WebCommunication/Prefabs/moodleLogin");
    }
}
