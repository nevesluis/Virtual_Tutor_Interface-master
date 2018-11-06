using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;
using UserInfo;
using System.Threading;
using System.Data;
//using Mono.Data.Sqlite;
//using System.Data.Sql;
//using System.Data.SqlClient;



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
    

    DateTime lastupdate;

    String userName = "ricardo";
    String password = "WS_TestUser1";

    //String token//MoodleSession = "d814dcb92eff420d14bd3574de0a3b3a";
    String tutorToken = "14ab94c8af25f6b426fc61cde1ed090b";
    int tutorID;
    Boolean userVerified = false;
    String userToken;
    UserData user;
    public static WebserviceLogin Instance;


    private Thread updateThread;

    //HUDSize hud; ONDE VAI BUSCAR OS VALORES INSERIDOS PELO USER

    //dataflow scrollview; // ONDE VAI FAZER DISPLAY DA INFO

    String moodleUrl = "http://ec2-52-211-160-228.eu-west-1.compute.amazonaws.com/moodleFCUL";

    int cycle = 0; // saber o nº de vezes que tentou ver actualizações

    // usado para separar multiplas cadeiras de so uma
    public Boolean multi = false;
    int courseId=0;
    bool newU = true;

    public void selectionMulti()
    {
        multi = !multi;
    }

    // Use this for initialization
    void Start()
    {
        //scrollview = gameObject.AddComponent(typeof(dataflow)) as dataflow;
        //hud = gameObject.AddComponent(typeof(HUDSize)) as HUDSize;
        //StartCoroutine("webGlRequests");
        Instance = this;
        StartCoroutine("RetrieveTutorID");
        GameObject moodleLogin = GameObject.Find("LoginMoodle");
        user = moodleLogin.GetComponent(typeof(UserData)) as UserData;
        //Debug.Log("inicial id_user: " + user.id);
        //testStuff();
    }

    public void testStuff()
    {
        Get_userId(3);
        Get_courseId(5);
        beginConnection();
    }

    public void getUserClass()
    {
        GameObject moodleLogin = GameObject.Find("LoginMoodle");
        user = moodleLogin.GetComponent(typeof(UserData)) as UserData;
        newU = !newU;
    }


    //public void compareTime()
    //{

    //    TimeSpan span = DateTime.UtcNow.Subtract(lastupdate);
    //    StringBuilder timeString = new StringBuilder();
    //    if (span.Minutes > 0)
    //    {
    //        timeString.Append(span.Minutes + "minutos ");
    //    }
    //    timeString.Append(span.Seconds + "segundos");
    //    scrollview.addText("Differença de tempo: " + timeString.ToString());

    //}

    LinkedList<string> folios = new LinkedList<string>();
    LinkedList<string> topics = new LinkedList<string>();
    LinkedList<string> foruns = new LinkedList<string>();

    int npoints = 0;
    DateTime last = DateTime.UtcNow, current, lastCheck;
    TimeSpan s;
    Boolean wasPressed = false;
    Boolean greeting = false;
    string infoUpdates = "";
    // Update is called once per frame
    void Update()
    {
        //if (!user.readyForRead && wasPressed)
        //{
        //    current = DateTime.UtcNow;
        //    s = current - last;
        //    if (s.TotalSeconds > 1) // mexe com os pontos
        //    {
        //        if (npoints < 3)
        //        {
        //            scrollview.joinText(".");
        //            npoints++;
        //        }
        //        else
        //        {
        //            scrollview.changeText(scrollview.giveText().Replace(".",""));
        //            npoints = 0;
        //        }
        //        last = current;
        //    }
        //}

        if (user.readyForRead && !greeting)
        {
            //scrollview.changeText(user.giveData());
            StartCoroutine("checkNewInfo");
            greeting = !greeting;
        }

        TimeSpan span = DateTime.UtcNow.Subtract(lastCheck);

        if (span.TotalSeconds > 60 && user.readyForRead)
        {
            StartCoroutine("hasUpdates");
            lastCheck = DateTime.UtcNow;
        }

    }

    public void press()
    {
        if (!wasPressed)
        {
            wasPressed = true;
            StartCoroutine("StartUp");
            lastupdate = user.datelast;
            // consequentes updates
            lastupdate = DateTime.UtcNow;
            lastCheck = DateTime.UtcNow;
        }
    }

    public LinkedList<string> updatesFolios()
    {
        return folios;
    }

    public LinkedList<string> updatesTopics()
    {
        return topics;
    }

    public LinkedList<string> updatesForuns()
    {
        return foruns;
    }

    // METODOS PARA FAZER RETRIEVE DE INFO DO MOODLE SEM O USO DE SOCKETS
    // CHAMADAS AS ESTES METODOS SAO DA FORMA: StartCoroutine("webGlRequests");

    public Boolean beginConnection()
    {
        
        StartCoroutine("getUserData");
        lastupdate = user.datelast;

        // consequentes updates
        lastupdate = DateTime.UtcNow;
        lastCheck = DateTime.UtcNow;
        return userVerified;
    }

    public Boolean beginConnectionWithId(int id, int courseId)
    {
        if(user == null)
        {
            getUserClass();
        }
        user.id = id;
        if (!multi)
        {
            this.courseId = courseId;
        }
        StartCoroutine("getUserData");

        return userVerified;
    }


    IEnumerator StartUp()
    {
        // A Ordem eh importante
        yield return retrieveUserWithId();
        if (userVerified)
        {
            yield return RetrieveCourses();
            
            yield return RetrieveCourseTopics();
            yield return RetrieveCourseGrades();
            yield return RetrieveUserGrades();
            yield return RetrieveForumData();
            lastupdate = user.datelast;
            user.doneWriting(); // marca o final da captacao de dados do user
        }

        //else
        //{
        //    scrollview.changeText("Login incorrecto");
        //}
    }

    IEnumerator getUserData()
    {
        yield return retrieveUserWithId();
        if (userVerified) // se o aluno foi autenticado
        {
            yield return RetrieveCourses(); // busca as cadeiras que o aluno esta inscrito, tem em conta se são varias cadeiras
            yield return retrieveCourseGroups();
            yield return RetrieveCourseTopics();

            yield return getCourseNotes();
            yield return RetrieveCourseGrades();
            yield return RetrieveUserGrades();
            yield return RetrieveForumData();
            lastupdate = user.datelast;
            user.doneWriting(); // marca o final da captacao de dados do user
        }
    }

    /**
     * Metodo que Comunica com o servidor para ir buscar o token necessario, nao utiliza sockets
     */
    IEnumerator RetrieveToken()
    {
        WWW www = new WWW(moodleUrl + "/login/token.php" + "?service=Login&username=" + userName + "&password=" + password);
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

    /*
     * Metodo que comunica com o servidor e vai buscar a informacao do user, nao utiliza sockets
     */
    IEnumerator retrieveUserWithId()
    {
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_user_get_users&criteria[0][key]=id&criteria[0][value]=" + user.id + "&moodlewsrestformat=json");
        yield return www;
        String content = www.text;
        
        Values v = UnityEngine.JsonUtility.FromJson<Values>(content);
        if (v.users.Count>0)
        {
            user.receiveUsers(v.users[0]);
             
            userVerified = true;
        }
        else
            userVerified = false;
        
    }

    /*
     * Metodo para obter o ID to tutor, usado para as notes 
     */
    IEnumerator RetrieveTutorID()
    {
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_user_get_users&criteria[0][key]=username&criteria[0][value]=" + "tutor_virtual" + "&moodlewsrestformat=json");
        yield return www;
        String content = www.text;

        Values v = UnityEngine.JsonUtility.FromJson<Values>(content);
        tutorID = v.users[0].id;
    }
    /**
     *  Metodo que comunica com o servidor e busca a informacao dos cursos a que o utilizador esta inscrito, nao utiliza sockets
     */
    IEnumerator RetrieveCourses()
    {
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_enrol_get_users_courses&userid=" + user.id + "&moodlewsrestformat=json");
        yield return www;
        String content = www.text;

        StringBuilder por = new StringBuilder();
        por.Append("{\"courses\":");
        por.Append(content + "}");

        Values v = JsonUtility.FromJson<Values>(por.ToString());
        user.receiveCourses(v.courses,multi,courseId);
    }

    IEnumerator RetrieveUserGrades() // gradereport_overview_get_course_grades, vai buscar as notas do aluno relativamente a cadeira no geral
    {
        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=gradereport_overview_get_course_grades&userid=" + user.id + "&moodlewsrestformat=json");
        yield return www;
        String content = www.text;

        Values v = UnityEngine.JsonUtility.FromJson<Values>(content);
        user.receiveGrades(v.grades);
    }

    IEnumerator RetrieveCourseTopics()
    {
        
        WWW www;
        String content;
        StringBuilder por;
        Values v;

        foreach (Course c in user.courses)
        {
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_course_get_contents&courseid=" + c.id + "&moodlewsrestformat=json"); // NAO sera utilizado de forma primaria para identificar conteudo na cadeira, eh necessario avaliar o aluno de acordo com o grupo a que pertence (avaliacao continua)
            yield return www;
            content = www.text;
            por = new StringBuilder();
            por.Append("{\"topics\":");
            por.Append(content + "}");
            
            v = JsonUtility.FromJson<Values>(por.ToString());
            c.receiveCourseTopics(v.topics);
        }
      
    }

    IEnumerator RetrieveCourseGrades() // mod_assign_get_assignments, busca as tentativas (attemptgrades)
    {
        WWW www;
        String content;
        StringBuilder sb;
        int count = 0;
        Values v;
        
        foreach (Course c in user.courses)
        {
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=mod_assign_get_assignments&courseids[0]=" + c.id + "&moodlewsrestformat=json");
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
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=mod_assign_get_grades" + sb.ToString() + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;

            v = JsonUtility.FromJson<Values>(content);
            c.receiveAssignmentsGrade(v.assignments, user.id);

        }
    }

    IEnumerator RetrieveForumData()
    {
        String content;
        StringBuilder por;
        Values v;
        WWW www;

        // report_competency_data_for_report
        foreach (Course c in user.courses)
        {
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=mod_forum_get_forums_by_courses&courseids[0]=" + c.id + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;
            por = new StringBuilder();
            por.Append("{\"forums\":");
            por.Append(content + "}");

            v = JsonUtility.FromJson<Values>(por.ToString());

            user.getCourse().receiveForums(v.forums);

            foreach (UserInfo.Course.Forum f in user.getCourse().forums)
            {
                www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=mod_forum_get_forum_discussions_paginated&forumid=" + f.id + "&moodlewsrestformat=json"); // vem organizados pelo mais recentemente alterado
                yield return www;
                content = www.text;
                v = JsonUtility.FromJson<Values>(content);
                user.getCourse().receiveDiscussions(v.discussions, f.id);


                foreach (UserInfo.Course.Discussions d in f.discussions)
                {
                    www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=mod_forum_get_forum_discussion_posts&discussionid=" + d.id + "&moodlewsrestformat=json"); // vem organizados pelo mais recentemente alterado
                    yield return www;
                    content = www.text;
                    v = JsonUtility.FromJson<Values>(content);
                    user.getCourse().receivePosts(v.posts, f.id, d.id);

                }
            }

        }
    }

    private void startUpdateCheck()
    {
        StartCoroutine("hasUpdates");
    }

    string noNewInfo = "A cadeira não tem novidades! \n\n";

    /**
     * Metodo para verificar se houve actualizações desde o ultimo login, só serve para por em texto as novidades
     * TODO core_course_check_updates pode ter que ser utilizado para validar informação que afecte o user -> Check if there is updates affecting the user for the given course and contexts.
     * */
    IEnumerator checkNewInfo()
    {
        TimeSpan s = user.datelast - new DateTime(1970, 1, 1);
        //UnityEngine.Debug.Log("Verificar updates: " + cycle);
        cycle++;
        StringBuilder debrief = new StringBuilder("");
        WWW www; String content; Values v;


            foreach (UserInfo.Course c in user.courses)
            {
                www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_course_get_updates_since&courseid=" + c.id + "&since=" + (int)s.TotalSeconds + "&moodlewsrestformat=json");
                yield return www;
                content = www.text;
                v = JsonUtility.FromJson<Values>(content);

                if (v.instances.Count > 0) // Ocurreu algo de novo
                {
                    debrief.Append("For Course " + c.fullName + " :\n");
                    foreach (jsonValues.instances i in v.instances)
                    {
                        if (i.contextlevel.ToLower().Equals("module"))
                        {
                            object m = user.getCourse().getUndefinedModule(i.id);
                            if (m == null)
                            {
                                debrief.Append("A module with the id " + i.id + " was removed\n");

                            }
                            else
                            {
                                if (m.GetType().Equals(typeof(UserInfo.Course.Folio)))
                                {
                                    UserInfo.Course.Folio f = (UserInfo.Course.Folio)m;
                                    folios.AddLast(f.name);
                                    Debug.Log("Fólio");
                                }
                                else if (m.GetType().Equals(typeof(UserInfo.Course.Forum)))
                                {
                                    UserInfo.Course.Forum fu = (UserInfo.Course.Forum)m;
                                    foruns.AddLast(fu.name);
                                    Debug.Log("Fórum");
                                }
                                else if (m.GetType().Equals(typeof(UserInfo.Course.modules)))
                                {
                                    UserInfo.Course.modules mod = (UserInfo.Course.modules)m;
                                    topics.AddLast(mod.name);
                                    Debug.Log("Module");
                                }
                            }
                        }
                        if (i.contextlevel.ToLower().Equals("topic"))
                        {
                            UserInfo.Course.Topic t = user.getCourse().GetTopic(i.id);
                            if (t == null)
                            {
                                debrief.Append("A Topic with the id " + i.id + " was removed\n");
                            }
                            else
                            {
                                debrief.Append("On Topic " + t.name + " something happened\n");
                                Debug.Log("TOPIC");
                                topics.AddLast(t.name);
                            }

                        }
                    }
                }
                else if (v.instances.Count == 0)
                {
                    debrief.Append(noNewInfo);
                }
                debrief.Append("\n");
            }
        infoUpdates = debrief.ToString();
    }

    IEnumerator hasUpdates()
    {
        TimeSpan s = lastupdate - new DateTime(1970, 1, 1);
        //UnityEngine.Debug.Log("Verificar updates: " + cycle);
        cycle++;
        Boolean happened = false;
        WWW www; String content; Values v;

        foreach (Course c in user.courses)
        {
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_course_get_updates_since&courseid=" + c.id + "&since=" + (int)s.TotalSeconds + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;
            v = JsonUtility.FromJson<Values>(content);


            if (v.instances != null)
            {
                if (v.instances.Count > 0)
                {
                    StartCoroutine(executeUpdates(v.instances, c.id));
                    happened = true;
                }
            }
        }
    }
    
    IEnumerator executeUpdates(List<jsonValues.instances> instances, int id)
    {
        user.needsWriting();
        StringBuilder sb = new StringBuilder(), modu = new StringBuilder();
        object m;
        Debug.Log("executeUpdates de " + id);
        // fazer updates aqui de acordo com o que pede, por agora manter assim
        yield return StartCoroutine("RetrieveCourseTopics");
        foreach (jsonValues.instances i in instances)
        {
            
            foreach (jsonValues.updates u in i.updates)
            {

                sb.Append(u.name + ",");
                if (u.name.ToLower().Equals("configuration"))
                {
                    m = user.getCourse().getUndefinedModule(i.id);

                    if (m != null)
                    {
                        Debug.Log("Type in execute: " + m.GetType());
                        if (m.GetType().Equals(typeof(UserInfo.Course.modules)))
                        {
                            UserInfo.Course.modules mod = (UserInfo.Course.modules)m;
                            modu.Append("TEM O NOME: " + mod.name);
                            topics.AddLast(mod.name);
                            Debug.Log(mod.name);
                        }
                        if (m.GetType().Equals(typeof(UserInfo.Course.Folio)))
                        {
                            UserInfo.Course.Folio f = (UserInfo.Course.Folio)m;
                            modu.Append("TEM O NOME: " + f.name);
                            folios.AddLast(f.name);
                            Debug.Log(f.name);
                        }
                        else if (m.GetType().Equals(typeof(UserInfo.Course.Forum)))
                        {
                            UserInfo.Course.Forum fu = (UserInfo.Course.Forum)m;
                            modu.Append("TEM O NOME: " + fu.name);
                            foruns.AddLast(fu.name);
                            Debug.Log(fu.name);
                        }

                    }
                    else
                        modu.Append("ERRO ID: " + i.id);

                }

            }

            sb = new StringBuilder();
            modu = new StringBuilder();
        }
        user.doneWriting();
    }

    IEnumerator getCourseNotes() // USADO NUM AMBIENTE DE TESTE
    {
        StringBuilder por;
        String content;
        Values v;
       
        foreach (Course c in user.courses)
        {
            WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_notes_get_course_notes&userid=" + tutorID +
                "&courseid=" + c.id + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;

            por = new StringBuilder();
            por.Append("{\"notes\":[");
            por.Append(content + "]}");

            v = JsonUtility.FromJson<Values>(por.ToString());
            c.notes.receiveNotes(v.notes,user.id);

        }
    }

    IEnumerator retrieveCourseGroups()
    {
        WWW www;
        String content;
        Values v;
        foreach (UserInfo.Course c in user.courses)
        {
            www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_group_get_course_user_groups&courseid=" +c.id+ "&userid=" + user.id + "&moodlewsrestformat=json");
            yield return www;
            content = www.text;

            v = JsonUtility.FromJson<Values>(content);
            c.receiveGroups(v.groups);
        }
    }

    /**
    * Metodo usado para testar novos webservices a parte
    * http://ec2-52-215-90-45.eu-west-1.compute.amazonaws.com/moodleFCUL/webservice/rest/server.php?wstoken=14ab94c8af25f6b426fc61cde1ed090b&wsfunction=core_group_get_course_user_groups&courseid=6&userid=3&moodlewsrestformat=json
    * http://ec2-52-215-90-45.eu-west-1.compute.amazonaws.com/moodleFCUL/webservice/rest/server.php?wstoken=14ab94c8af25f6b426fc61cde1ed090b&wsfunction=core_notes_get_course_notes&courseid=5&userid=6&moodlewsrestformat=json
    * */
    IEnumerator testNewFunction()
    {
        
        UnityEngine.Debug.Log("NEW FUNCTION");

        WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_user_get_course_user_profiles&userlist[0][userid]=" + user.id + "&userlist[0][courseid]=" + courseId);
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

        
        existe = user.courses[0].notes.existNote(user.id);// TODO TEMPORARIO

        StartCoroutine(newQuestion(duvida,texto,resolvido,existe));
        
    }

    IEnumerator newQuestion(Boolean duvida,String textoDuvida, Boolean resolvido, Boolean existe) // USADO NUM AMBIENTE DE TESTE
    {
        StringBuilder por = new StringBuilder();
       
       
        if (!existe)
        {
            por.AppendLine(user.id + " ");
            por.AppendLine("QUESTOES:");
            por.AppendLine(textoDuvida + " Duvida:" + duvida + " Resolvido:" + resolvido);
            UnityEngine.Debug.Log("a criar nota");
            UnityEngine.Debug.Log(por.ToString());
            UnityEngine.Debug.Log(tutorToken);
            UnityEngine.Debug.Log(tutorID);
            WWW www = new WWW(moodleUrl + "/webservice/rest/server.php" + "?wstoken=" + tutorToken + "&wsfunction=core_notes_create_notes&notes[0][userid]=" + tutorID + "&notes[0][publishstate]=personal"
                        + "&notes[0][courseid]=5" + "&notes[0][text]=" + por.ToString() + " " + "&notes[0][format]=2" + "&moodlewsrestformat=json");
            yield return www;
            String content = www.text;
           
        }

        else
        {
            UnityEngine.Debug.Log("Actualizar nota");
            UserInfo.Notes.Note n = user.courses[0].notes.getNote(user.id); // TODO TEMPORARIO
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

    // METODOS PARA IR BUSCAR VALORES DADOS PELO USER
    public void getUserName()
    {
        InputField[] infs = GUIElement.FindObjectsOfType<InputField>();
        foreach (InputField inf in infs)
        {
            if (inf.name.Equals("InputUser"))
                userName = inf.text;
        }
    }

    
    public void Get_userId(int id)
    {
        Debug.Log("Id enviado: " + id);
        user.id = id;
    }

    public void Get_courseId(int id)
    {
         courseId = id;
    }


    public void getPassword()
    {
        InputField[] infs = GUIElement.FindObjectsOfType<InputField>();
        foreach (InputField inf in infs)
        {
            if (inf.name.Equals("InputPassword"))
                password = inf.text;
        }
    }


    public void writeToFileExample()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string filename = Path.Combine(path, "myfile.txt");

        using (var streamWriter = new StreamWriter(filename, true))
        {
            streamWriter.WriteLine(DateTime.UtcNow);
        }

        using (var streamReader = new StreamReader(filename))
        {
            string content = streamReader.ReadToEnd();
            //System.Diagnostics.Debug.WriteLine(content);
            content.ToString();
        }
    }

}
