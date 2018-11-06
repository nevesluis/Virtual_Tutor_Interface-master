using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using UserInfo;

public class WebserviceLogin : MonoBehaviour
{
    // https://moodle.org/mod/forum/discuss.php?d=227169
    [Serializable]
    public class Values
    {
        public List<jsonValues.users> users;
        //public List<warnings> warnings;

        // course
        public List<jsonValues.Courses> courses;
        public List<jsonValues.Grades> grades;
        public List<jsonValues.dbValues> dbValues;

        public List<jsonValues.Topics> topics;
        public List<jsonValues.assignments> assignments;
        public List<jsonValues.usergrades> usergrades;
        public List<jsonValues.groups> groups;
        //quizzes
        public List<jsonValues.quizzes> quizzes; // TODO IMPLEMENT
        //update related
        public List<jsonValues.instances> instances;
        // forum related
        public List<jsonValues.forums> forums;
        public List<jsonValues.discussions> discussions;
        public List<jsonValues.posts> posts;
        // notes related
        public List<jsonValues.notes> notes;
    }
    public static List<jsonValues.dbValues> teste;

    System.Diagnostics.Stopwatch stop = new System.Diagnostics.Stopwatch();
    DateTime lastupdate;

    String tutorToken = "14ab94c8af25f6b426fc61cde1ed090b";
    int tutorID;
    public Boolean userVerified = false;
    String userToken ="14ab94c8af25f6b426fc61cde1ed090b";
    String testToken = "fff2866b7aa75ca34556714df5e2c173";

    DataManager dataM;

    public Boolean structsReady = false;

    public int countMethods = 0;
    public Boolean barrier = false;
    


    /// <summary>
    /// Increases the count of concluded methods.
    /// </summary>
    public void increaseCount()
    {
        //Debug.Log("TO INCREASE");
        //while (barrier);
        countMethods += 1;
        barrier = false;
        //Debug.Log("DONE");
    }
    //HUDSize hud; ONDE VAI BUSCAR OS VALORES INSERIDOS PELO USER

    public String moodleUrl = "http://ec2-34-240-43-90.eu-west-1.compute.amazonaws.com/moodleFCUL";

    int cycle = 0; // saber o nº de vezes que tentou ver actualizações

    // usado para separar multiplas cadeiras de so uma
    public Boolean multi = false;
    int courseId=0;

    /// <summary>
    /// Inverts the selection, if multiple courses are desired.
    /// </summary>
    public void selectionMulti()
    {
        multi = !multi;
    }

    // Use this for initialization
    void Start()
    {
        //hud = gameObject.AddComponent(typeof(HUDSize)) as HUDSize;
        //StartCoroutine("webGlRequests");
        //StartCoroutine("RetrieveTutorID");
        //Teste();
        //manager = (DataManager)Resources.Load("/WebCommunication/Prefabs/moodleLogin");
    }

    DateTime begining;

    /// <summary>
    /// Compares the time.
    /// Used to give the difference between the start and the end of the procedure.
    /// </summary>
    public void compareTime()
    {

        TimeSpan span = DateTime.UtcNow.Subtract(begining);
        StringBuilder timeString = new StringBuilder();
        if (span.Minutes > 0)
        {
            timeString.Append(span.Minutes + "minutos ");
        }
        timeString.Append(span.Seconds + "segundos");

        Debug.Log("Differença de tempo: " + timeString.ToString());

    }

    /// <summary>
    /// Finds the data manager.
    /// Used to insert the information got from Moodle.
    /// </summary>
    public void findDataManager()
    {
        GameObject moodleLogin = GameObject.Find("moodleLogin");
        dataM = moodleLogin.GetComponent(typeof(DataManager)) as DataManager;
    }
    

    int npoints = 0;
    DateTime last = DateTime.UtcNow, current, lastCheck;
    TimeSpan s;
    Boolean wasPressed = false;
    // Update is called once per frame
    void Update()
    {
        //if (countMethods > 5 && !dataM.getUser().readyForRead)
        //{
        //    dataM.getUser().doneWriting(); // marca o final da captacao de dados do user
        //}
        if(dataM != null)
        if (dataM.getUser().readyForRead && npoints !=1)
        {
            
            compareTime();
            npoints=1;
        }
        if (countMethods > 5 && stop.IsRunning)
        {
            stop.Stop();
            Debug.Log("Webservices Done, Seconds: " + stop.ElapsedMilliseconds / 1000f);
        }
        
        //if (scrollview.giveText().Contains("A carregar informação") && dataM.getUser().readyForRead)
        //{
        //    scrollview.changeText(dataM.getUser().giveData());
        //    StartCoroutine("checkNewInfo");

        //}


        //TimeSpan span = DateTime.UtcNow.Subtract(lastCheck);

        //if (span.TotalSeconds > 60 && dataM.getUser().readyForRead)
        //{
        //    StartCoroutine("hasUpdates");
        //    lastCheck = DateTime.UtcNow;
        //}

    }


    /**
     * Devido ao webgl nao suportar acesso directo as sockets que torna o System.Net invalido, foi necessario criar uma alternativa a solucao corrente
     * */

    // METODOS PARA FAZER RETRIEVE DE INFO DO MOODLE SEM O USO DE SOCKETS
    // CHAMADAS AS ESTES METODOS SAO DA FORMA: StartCoroutine("webGlRequests");

    /// <summary>
    /// Begins the connection.
    /// Receives the student data and acts upon the device
    /// </summary>
    /// <param name="values">The values given by WebManager.</param>
    /// <returns>If the user was verified.</returns>
    public Boolean beginConnection(Dictionary<String,object> values)
    {
        findDataManager();
        begining = DateTime.UtcNow;
#if UNITY_ANDROID
         StartCoroutine(getUserDataMobile(Convert.ToString(values["username"]), Convert.ToString(values["password"])));
#else
        dataM.getUser().id = Convert.ToInt32(values["userId"]);
        if (!multi)
        {
            this.courseId = Convert.ToInt32(values["courseId"]);
        }
        StartCoroutine(getUserData());
        
#endif
        return userVerified;
    }


    /// <summary>
    /// Gets the required information for the correct operation of the application, for the Android version.
    /// </summary>
    /// <param name="username">The username of the student.</param>
    /// <param name="password">The password of the student.</param>
    IEnumerator getUserDataMobile(String username, string password) // Metodo que organiza a busca de informação feita na aplicação mobile
    {
        
        yield return testLogin(username, password);
        if (userVerified)
        {
            yield return RetrieveToken(username, password);
            yield return retrieveUser(username);
            yield return RetrieveCourses(); // busca as cadeiras que o aluno esta inscrito, tem em conta se são varias cadeiras
            
            retrieveCourseGroups();
            yield return RetrieveCourseTopics();

            //yield return getCourseNotes();
            RetrieveCourseGrades();
            //RetrieveUserGrades();
            RetrieveForumData();
            lastupdate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(dataM.getUser().datelast);

            dataM.getUser().doneWriting(); // marca o final da captacao de dados do user
        }
    }

    /// <summary>
    /// Gets the required information for the correct operation of the application, for the WebGl version.
    /// </summary>
    IEnumerator getUserData() // Metodo que organiza a busca de informação feita na aplicação WebGl
    {
        stop.Start();
        dataM.stop.Start();
        StartCoroutine(retrieveUser("")); // não eh preciso autenticar ou verificar o aluno, assumindo que este metodo eh soh chamado no WebGl dentro do moodle
        
        yield return StartCoroutine(RetrieveCourses()); // busca as cadeiras que o aluno esta inscrito, tem em conta se são varias cadeiras
        structsReady = true;
        StartCoroutine(RetrieveForumData()); // busca forums
         
        StartCoroutine(retrieveCourseGroups());
        //StartCoroutine(getCourseNotes());
        StartCoroutine(RetrieveCourseGrades()); // busca e-fólios
        
        StartCoroutine(RetrieveCourseTopics());
        
        lastupdate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(dataM.getUser().datelast);
       
    }

    /**
     * Metodo que Comunica com o servidor para ir buscar o token necessario, nao utiliza sockets
     */
    /// <summary>
    /// Retrieves the token. Useful for the Android version.
    /// </summary>
    /// <param name="username">The username of the student.</param>
    /// <param name="password">The password of the student.</param>
    IEnumerator RetrieveToken(String username, String password)
    {
        WWW www = new WWW(moodleUrl + "/login/token.php" + "?service=TVService&username=" + username + "&password=" + password);
        yield return www;
        String content = www.text;
        String[] variable = content.Split(new[] { "\"token\":\"" }, StringSplitOptions.None);
        if (variable.Length > 1)
        {
            variable = variable[1].Split(new[] { "\"" }, StringSplitOptions.None);
            if (variable.Length > 0)
            {
                userVerified = true;
                userToken = variable[0];

            }
        }
    }

    /// <summary>
    /// Retrieves the user information stored in the core_user_get_users webservice.
    /// </summary>
    /// <param name="username">The username of the student.</param>
    IEnumerator retrieveUser(String username)
    {
        WWWForm forms = new WWWForm();
        forms.AddField("wstoken", tutorToken);
        forms.AddField("wsfunction", "core_user_get_users");
#if UNITY_ANDROID
     
        forms.AddField("criteria[0][key]","username");
        forms.AddField("criteria[0][value]", username);
#else
        //Debug.Log(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_user_get_users&criteria[0][key]=id&criteria[0][value]=" + dataM.getUser().id + "&moodlewsrestformat=json");
       

        forms.AddField("criteria[0][key]","id");
        forms.AddField("criteria[0][value]", dataM.getUser().id);
#endif
        
        forms.AddField("moodlewsrestformat", "json");
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php", forms.data);
        yield return www;
        String content = www.text;
        
        Values v = UnityEngine.JsonUtility.FromJson<Values>(content);

        if (v.users.Count>0)
        {
            dataM.getUser().receiveUsers(v.users[0]);
             
            userVerified = true;
        }
        else
            userVerified = false;
        
        increaseCount();

    }

    /// <summary>
    /// Tests the login to verify the user.
    /// </summary>
    /// <param name="Username">The username of the student.</param>
    /// <param name="Password">The password of the student.</param>
    public IEnumerator testLogin(String Username, string Password)
    {
        string formUrl = "http://ec2-52-211-160-228.eu-west-1.compute.amazonaws.com/moodleFCUL/login/index.php";

        WWWForm loginFields = new WWWForm();
        
        loginFields.AddField("username", Username);
        loginFields.AddField("password", Password);

        WWW www = new WWW(formUrl, loginFields.data);

        yield return www;

        String pageSource = www.text;
        
        if (pageSource.Contains("<title>Dashboard</title>"))
            userVerified = true;
        else
        {
            userVerified = false;
        }
    }

    /// <summary>
    /// Retrieves the tutor identifier. Used for the Notes
    /// </summary>
    IEnumerator RetrieveTutorID()
    {
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_user_get_users&criteria[0][key]=username&criteria[0][value]=" + "tutor_virtual" + "&moodlewsrestformat=json");
        yield return www;
        String content = www.text;

        Values v = UnityEngine.JsonUtility.FromJson<Values>(content);
        tutorID = v.users[0].id;
    }

    /// <summary>
    /// Retrieves the information of the courses the student attends.
    /// core_enrol_get_users_courses
    /// </summary>
    IEnumerator RetrieveCourses()
    {
        

#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        WWWForm forms = new WWWForm();
        forms.AddField("wstoken",token);
        forms.AddField("wsfunction", "core_enrol_get_users_courses");
        forms.AddField("userid", dataM.getUser().id);
        forms.AddField("moodlewsrestformat", "json");
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php",forms.data);
            yield return www;
            String content = www.text;

            StringBuilder por = new StringBuilder();
            por.Append("{\"courses\":");
            por.Append(content + "}");
        
            Values v = JsonUtility.FromJson<Values>(por.ToString());
       
            dataM.receiveCourses(v.courses, multi, courseId);
        
        //foreach (Course c in dataM.getCourses())
        //{
        //    StartCoroutine(RetrieveUsersInCourse(c.id));
        //}

        increaseCount();
    }

    /// <summary>
    /// Retrieves the users in a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    IEnumerator RetrieveUsersInCourse(int courseId)
    {

#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_enrol_get_enrolled_users&courseid="+courseId+"&moodlewsrestformat=json");
        yield return www;
        String content = www.text;
        
        StringBuilder por = new StringBuilder();
        por.Append("{\"users\":");
        por.Append(content + "}");
        Values v = UnityEngine.JsonUtility.FromJson<Values>(por.ToString());
    }

    /// <summary>
    /// Retrieves the user grade for the course using the gradereport_overview_get_course_grades webservice.
    /// </summary>
    /// <returns></returns>
//    IEnumerator RetrieveUserGrades() // gradereport_overview_get_course_grades, vai buscar as notas do aluno relativamente a cadeira no geral
//    {
//#if UNITY_ANDROID
//        String token = userToken;
//#else
//        String token = tutorToken;
//#endif

//        WWWForm forms = new WWWForm();
//        forms.AddField("wstoken", token);
//        forms.AddField("wsfunction", "gradereport_overview_get_course_grades");
//        forms.AddField("userid", dataM.getUser().id);
//        forms.AddField("moodlewsrestformat", "json");
//        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php",forms.data);
//        yield return www;
//        String content = www.text;
//        Debug.Log(content);
//        Values v = UnityEngine.JsonUtility.FromJson<Values>(content);

//        dataM.receiveGrades(v.grades);
//        //increaseCount();
//    }

    /// <summary>
    /// Retrieves the course topics using the core_course_get_contents webservices.
    /// </summary>
    IEnumerator RetrieveCourseTopics()
    {
        
    #if UNITY_ANDROID
            String token = userToken;
    #else
            String token = tutorToken;
    #endif
            WWW www;

            String content;
            StringBuilder por;
            Values v;
            WWWForm loginFields;
            DateTime now = DateTime.UtcNow;        
            foreach (Course c in dataM.getCourses())
            {
                loginFields = new WWWForm();

                loginFields.AddField("wsfunction", "core_course_get_contents");
                loginFields.AddField("wstoken", token);
                loginFields.AddField("courseid", c.id);
                loginFields.AddField("moodlewsrestformat", "json");
                www = new WWW(moodleUrl + "/webservice/rest/server.php",loginFields.data); // NAO sera utilizado de forma primaria para identificar conteudo na cadeira, eh necessario avaliar o aluno de acordo com o grupo a que pertence (avaliacao continua)

                yield return www;
                content = www.text;

                por = new StringBuilder();
                por.Append("{\"topics\":");
                por.Append(content + "}");
            
                v = JsonUtility.FromJson<Values>(por.ToString());
               
                        c.receiveCourseTopics(v.topics);

            }
            increaseCount();
       

            dataM.topicsDone = true;
            dataM.getUser().doneWriting(); // marca o final da captacao de dados do user
            Debug.Log("Topics Done, Seconds: " + stop.ElapsedMilliseconds / 1000f);
        
    }


    /// <summary>
    /// Retrieves the course grades using the following webservices:
    /// mod_assign_get_assignments gives the information about the assignments (e-fólios).
    /// mod_assign_get_grades gives the grades of the assignments.
    /// gradereport_user_get_grade_items gives the information about the grade report.
    /// </summary>
    IEnumerator RetrieveCourseGrades() // mod_assign_get_assignments, busca as tentativas (attemptgrades)
    {
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        WWW www;
        String content;
        StringBuilder sb;
        int count = 0;
        Values v;
        
        foreach (Course c in dataM.getCourses())
        {
            
            
            //Debug.Log("a usar: " + c.fullName);
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=mod_assign_get_assignments&courseids[0]=" + c.id + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;

            v = JsonUtility.FromJson<Values>(content);

            c.receiveAssignments(v.courses[0].assignments);
                
            sb = new StringBuilder();
            count = 0;
            foreach (UserInfo.Course.Folio f in c.folios)
            {
                sb.Append("&assignmentids[" + count + "]=" + f.id);
                count++;
            }
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=mod_assign_get_grades" + sb.ToString() + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;

            v = JsonUtility.FromJson<Values>(content);

            c.receiveAssignmentsGrade(v.assignments, dataM.getUser().id);


            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=gradereport_user_get_grade_items&courseid=" + c.id + "&userid=" + dataM.getUser().id + "&groupid=0&moodlewsrestformat=json");
            yield return www;

            content = www.text;
            //Debug.Log(content);
            v = JsonUtility.FromJson<Values>(content);
            foreach (jsonValues.usergrades ug in v.usergrades)
            {

                c.receiveGrades(ug);
            }

                c.currentAprov = c.currentAproveitamento();
                c.maxCurrentAprov = c.maxCurrentAproveitamento();
                Debug.Log(c.fullName + " Aproveitamento: " + c.currentAprov + " out of " + c.maxCurrentAprov + " " + (Convert.ToDouble(c.currentAprov) / Convert.ToDouble(c.maxCurrentAprov)) * 100 + "%");
            
            
        }
        //StartCoroutine(RetrieveUserGrades());
        increaseCount();
        dataM.foliosDone = true;
        Debug.Log("Folios Done, Seconds: " + stop.ElapsedMilliseconds / 1000f);
    }

    /// <summary>
    /// Retrieves the forum data using the following webservices:
    /// mod_forum_get_forums_by_courses gives the information of the forums.
    /// mod_forum_get_forum_discussions_paginated gives the information about discussions contained in a forum.
    /// mod_forum_get_forum_discussion_posts gives the information about posts contained in a discussion.
    /// </summary>
    IEnumerator RetrieveForumData()
    {
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        String content;
        StringBuilder por;
        Values v;
        WWW www;
        Debug.Log("Forum Start, Seconds: " + stop.ElapsedMilliseconds / 1000f);
        // report_competency_data_for_report
        foreach (Course c in dataM.getCourses())
        {
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=mod_forum_get_forums_by_courses&courseids[0]=" + c.id + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;
            por = new StringBuilder();
            por.Append("{\"forums\":");
            por.Append(content + "}");

            v = JsonUtility.FromJson<Values>(por.ToString());
                    //Debug.Log("a usar: " + c.fullName + " receiveForums");
                    dataM.getCourseById(c.id).receiveForums(v.forums);


            //foreach (UserInfo.Course.Forum f in dataM.getCourseById(c.id).forums)
            //{

            //    www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=mod_forum_get_forum_discussions_paginated&forumid=" + f.id + "&moodlewsrestformat=json"); // vem organizados pelo mais recentemente alterado
            //    yield return www;
            //    content = www.text;
            //    v = JsonUtility.FromJson<Values>(content);

            //    dataM.getCourseById(c.id).receiveDiscussions(v.discussions, f.cmid);

            //}

            //foreach(UserInfo.Course.Forum forum in dataM.getCourseById(c.id).forums)
            //{
            //    foreach (UserInfo.Course.Discussions d in forum.discussions)
            //    {
            //        www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=mod_forum_get_forum_discussion_posts&discussionid=" + d.id + "&moodlewsrestformat=json"); // vem organizados pelo mais recentemente alterado

            //        yield return www;
            //        content = www.text;
            //        v = JsonUtility.FromJson<Values>(content);
            //        dataM.getCourseById(c.id).receivePosts(v.posts, forum.cmid, d.id, dataM.getUser().id);

            //    }

            //}

        }
        increaseCount();
        dataM.forumsDone = true;
        Debug.Log("Forum Done, Seconds: " + stop.ElapsedMilliseconds / 1000f);
    }

    /// <summary>
    /// Starts the verification procedure.
    /// </summary>
    /// <param name="timestamp">The timestamp to compare the updates.</param>
    public void startUpdateCheck(int timestamp)
    {
        StartCoroutine(checkNewInfo(timestamp));
    }

    /// <summary>
    /// Checks the new information of a course regarding a timestamp using the core_course_get_updates_since webservice.
    /// </summary>
    /// <param name="timestamp">The timestamp to compare.</param>
    IEnumerator checkNewInfo(int timestamp)
    {

#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        TimeSpan s = lastupdate - new DateTime(1970, 1, 1);
        UnityEngine.Debug.Log("Verificar updates: " + cycle);
        cycle++;
        StringBuilder debrief = new StringBuilder();
        WWW www; String content; Values v;


            foreach (UserInfo.Course c in dataM.getCourses())
            {
            //UnityEngine.Debug.Log(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_course_get_updates_since&courseid=" + c.id + "&since=" + timestamp+ "&moodlewsrestformat=json");

                www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_course_get_updates_since&courseid=" + c.id + "&since=" + timestamp + "&moodlewsrestformat=json");
                yield return www;
            
                content = www.text;
                //UnityEngine.Debug.Log(www.text);
                v = JsonUtility.FromJson<Values>(content);

                if (v.instances.Count > 0) // Ocurreu algo de novo
                {
                //debrief.Append("For Course " + c.fullName + " :\n");
                //foreach (jsonValues.instances i in v.instances)
                //{
                //    if (i.contextlevel.ToLower().Equals("module"))
                //    {
                //        Course.modules m = dataM.getCourseById(c.id).getSpecificModule(i.id);
                //        if (m == null)
                //        {
                //            debrief.Append("A module with the id " + i.id + " was removed\n");
                //        }
                //        else
                //        {
                //            debrief.Append("On Module " + m.name + " something happened\n");
                //        }
                //    }
                //    if (i.contextlevel.ToLower().Equals("topic"))
                //    {
                //        UserInfo.Course.Topic t = dataM.getCourseById(c.id).GetTopic(i.id);
                //        if (t == null)
                //        {
                //            debrief.Append("A Topic with the id " + i.id + " was removed\n");
                //        }
                //        else
                //        {
                //            debrief.Append("On Topic " + t.name + " something happened\n");
                //        }

                //    }
                //}
                    c.receiveUpdates(v.instances,dataM.getUser().datelast);
                    
                }
                else if (v.instances.Count == 0)
                {
                    debrief.Append("Course " + c.fullName + " has nothing to report\n\n");
                    
                }
                debrief.Append("\n");
                c.updatesFinished = true;
            }

        dataM.updatesDone = true;

    }

    //IEnumerator executeUpdates(List<jsonValues.instances> instances, int id)
    //{
    //    dataM.getUser().needsWriting();
        
    //    StringBuilder sb = new StringBuilder(), modu = new StringBuilder();
    //    object m;

    //    foreach (jsonValues.instances i in instances)
    //    {
    //        // fazer updates aqui de acordo com o que pede, por agora manter assim
    //        yield return StartCoroutine("RetrieveCourseTopics");

    //        foreach (jsonValues.updates u in i.updates)
    //        {
                
    //            sb.Append(u.name + ",");
    //            if (u.name.ToLower().Equals("configuration"))
    //            {
    //                m = dataM.getCourseById(id).getUndefinedModule(i.id);

    //                if (m != null)
    //                {
    //                    if (m.GetType().Equals(typeof(UserInfo.Course.modules)))
    //                    {
    //                        UserInfo.Course.modules mod = (UserInfo.Course.modules)m;
    //                        modu.Append("TEM O NOME: " + mod.name);
    //                    }


    //                }
    //                else
    //                    modu.Append("ERRO ID: " + i.id);
                    
    //            }

    //        }
    //        Debug.Log("Um(a) " + i.contextlevel + " com o id: " + i.id + " sofreu uma alteracao na " + sb.Remove(sb.Length - 1, 1).ToString() + "\n" + modu.ToString());
            
            
    //        sb = new StringBuilder();
    //        modu = new StringBuilder();
    //    }
    //    dataM.getUser().doneWriting();
    //}

    IEnumerator getCourseNotes() // USADO NUM AMBIENTE DE TESTE
    {
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        StringBuilder por;
        String content;
        Values v;
        
        foreach (Course c in dataM.getCourses())
        {
            WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_notes_get_course_notes&userid=" + tutorID +
                "&courseid=" + c.id + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;

            por = new StringBuilder();
            por.Append("{\"notes\":[");
            por.Append(content + "]}");
            
            v = JsonUtility.FromJson<Values>(por.ToString());
            c.notes.receiveNotes(v.notes,dataM.getUser().id);

        }
        //increaseCount();
    }

    /// <summary>
    /// Retrieves the course groups that the student belongs to for each course created in the application.
    /// core_group_get_course_user_groups Gives the list of groups the student is a part of.
    /// </summary>
    IEnumerator retrieveCourseGroups()
    {
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        WWW www;
        String content;
        Values v;
        foreach (UserInfo.Course c in dataM.getCourses())
        {
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_group_get_course_user_groups&courseid=" +c.id+ "&userid=" + dataM.getUser().id + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;

            v = JsonUtility.FromJson<Values>(content);
            c.receiveGroups(v.groups);
        }
        increaseCount();
    }

    IEnumerator testNewFunction()
    {
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        UnityEngine.Debug.Log("NEW FUNCTION");

        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_user_get_course_user_profiles&userlist[0][userid]=" + dataM.getUser().id + "&userlist[0][courseid]=" + courseId);
        yield return www;
        String content = www.text;
        UnityEngine.Debug.Log(content);

    }

    public void EnviarNota() // USADO NUM AMBIENTE DE TESTE
    {
        Boolean duvida=false, resolvido=false, existe;
        Toggle[] infs = GUIElement.FindObjectsOfType<Toggle>();
        String texto=null;
        foreach (Toggle inf in infs)
        {
            if (inf.name.Equals("Duvida/Questao"))
            {
                if (inf.isOn)
                    duvida = true;
            }

            if (inf.name.Equals("Resolvido"))
            {
                if (inf.isOn)
                    resolvido = true;
            }

        }
        InputField[] textos = GUIElement.FindObjectsOfType<InputField>();
        foreach (InputField inf in textos)
        {
            if (inf.name.Equals("DadosPergunta"))
                texto = inf.text;
        }

        
        existe = dataM.getCourses()[0].notes.existNote(dataM.getUser().id);// TODO TEMPORARIO

        StartCoroutine(newQuestion(duvida,texto,resolvido,existe));
        
    }

    IEnumerator newQuestion(Boolean duvida,String textoDuvida, Boolean resolvido, Boolean existe) // USADO NUM AMBIENTE DE TESTE
    {
        StringBuilder por = new StringBuilder();
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif

        if (!existe)
        {
            por.AppendLine(dataM.getUser().id + " ");
            por.AppendLine("QUESTOES:");
            por.AppendLine(textoDuvida + " Duvida:" + duvida + " Resolvido:" + resolvido);
            UnityEngine.Debug.Log("a criar nota");
            UnityEngine.Debug.Log(por.ToString());
            UnityEngine.Debug.Log(tutorToken);
            UnityEngine.Debug.Log(tutorID);
            WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_notes_create_notes&notes[0][userid]=" + tutorID + "&notes[0][publishstate]=personal"
                        + "&notes[0][courseid]=5" + "&notes[0][text]=" + por.ToString() + " " + "&notes[0][format]=2" + "&moodlewsrestformat=json");
            yield return www;
            
            String content = www.text;
           
        }

        else
        {
            UnityEngine.Debug.Log("Actualizar nota");
            UserInfo.Notes.Note n = dataM.getCourses()[0].notes.getNote(dataM.getUser().id); // TODO TEMPORARIO
            por.Append(Course.HtmlDecode(n.content));
            por.AppendLine(textoDuvida + " Duvida:" + duvida + " Resolvido:" + resolvido);
            WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_notes_update_notes" + "&notes[0][publishstate]=personal"
                       + "&notes[0][id]=" + n.id + "&notes[0][text]=" + por.ToString() + " " + "&notes[0][format]=2" + "&moodlewsrestformat=json");
            yield return www;
            String content = www.text;
            UnityEngine.Debug.Log(content);
        }

        StartCoroutine(getCourseNotes());
    }

    //private IEnumerator retrieveInfoMoodle(System.Action<string> callback)    
    private IEnumerator retrieveInfoMoodle(int courseId)
    {
        string url = moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + testToken + "&wsfunction=get_data&moodlewsrestformat=json" + "&courseid=" + courseId;
        //Debug.Log("url:" + url);
        WWW www = new WWW(url);
        yield return www;
        String texto = www.text.Replace("courseid", "courseId");
        //Debug.Log(content.ToString());

        Values v;
        StringBuilder por;
        por = new StringBuilder();
        por.Append("{\"dbValues\":");
        por.Append("[" + texto + "]" + "}");

        Debug.Log("TESTE " + por);

        v = JsonUtility.FromJson<Values>(por.ToString());

        if (v != null)
        {

            foreach (jsonValues.dbValues dbv in v.dbValues)
            {
                /*
                Debug.Log("ENNKDSFDFSA");
                Debug.Log(dbv.toString());
                Debug.Log(dbv.aprov_low);
                Debug.Log(dbv.courseId);*/
                //teste.Add(dbv);
                Debug.Log(dataM.getCourseById(courseId).parameters.toString());
                //dataM.getCourseById(dbv.courseId).parameters = dbv;
                //dataM.getCourseById(dbv.courseId).parameters = dbv;

                //var teste = gameObject.GetComponent<WebManager>();

                //WebManager.Instance.manager.getCourseById(dbv.courseId).parameters = dbv;
                //teste.manager.getCourseById(dbv.courseId).parameters = dbv;

            }
        }

        //callback(texto);

    }

    public void Teste(int courseId)
    {
        StartCoroutine(retrieveInfoMoodle(courseId));
    }


    IEnumerator getGrades()
    {
        StringBuilder por = new StringBuilder();
#if UNITY_ANDROID
        String token = userToken;
#else
        String token = tutorToken;
#endif
        
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + token + "&wsfunction=core_notes_create_notes&notes[0][userid]=" + tutorID + "&notes[0][publishstate]=personal"
                        + "&notes[0][courseid]=5" + "&notes[0][text]=" + por.ToString() + " " + "&notes[0][format]=2" + "&moodlewsrestformat=json");
        yield return www;

    }
    
}
