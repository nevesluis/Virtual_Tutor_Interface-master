using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

    public UserInfo.UserData user;
    public Boolean gotCourses = false;
    public List<UserInfo.Course> courses = new List<UserInfo.Course>();
    public System.Diagnostics.Stopwatch stop = new System.Diagnostics.Stopwatch();

    // Use this for initialization
    void Start () {
        
    }

	// Update is called once per frame
	void Update () {

        int count = 0;

        if (foliosDone && topicsDone && forumsDone && !dataDone)
        {
            foreach (UserInfo.Course c in courses)
            {

                c.removeUnecessaryModules();
                if (c.newsAttempt && c.updatesFinished && !c.everyNewsDone)
                {
                    c.folios.Sort((x, y) => x.name.CompareTo(y.name));
                    c.forums.Sort((x, y) => x.name.CompareTo(y.name));
                    c.UpdateTopicsForNews();
                    Debug.Log("All Data caught for structures, seconds: " + stop.ElapsedMilliseconds / 1000f);
                    //foreach (UserInfo.Course.Forum f in c.forums)
                    //{
                    //    Debug.Log("<color="+ (f.seen==1? "black" : f.seen==0? "yellow" : "red") + ">Forum: " + f.name + " seen: " + f.seen + "</color>");
                    //}
                    //foreach (UserInfo.Course.Folio fol in c.folios)
                    //{
                    //    Debug.Log("<color=" + (fol.seen == 1 ? "black" : fol.seen == 0 ? "yellow" : "red") + ">Folio: " + fol.name + " seen: " + fol.seen + "</color>");
                    //}
                    count++;
                }
                if (!c.everyNewsDone)
                    count++;
                if (count == 0)
                    newsFinal = true;
            }
            dataDone = true;
            Debug.Log("<color=blue>DATA</color>, seconds: " + stop.ElapsedMilliseconds / 1000f);
        }
        count = 0;

        if (dataDone && !newsFinal)
        {
            foreach (UserInfo.Course c in courses)
            {

                if (c.newsAttempt && c.updatesFinished && !c.everyNewsDone)
                {
                    c.folios.Sort((x, y) => x.name.CompareTo(y.name));
                    c.forums.Sort((x, y) => x.name.CompareTo(y.name));
                    c.UpdateTopicsForNews(); 
                    Debug.Log("All Data caught for structures, seconds: " + stop.ElapsedMilliseconds / 1000f);
                    //foreach (UserInfo.Course.Forum f in c.forums)
                    //{
                    //    Debug.Log("<color=" + (f.seen == 1 ? "black" : f.seen == 0 ? "yellow" : "red") + ">Forum: " + f.name + " seen: " + f.seen + "</color>");
                    //}
                    //foreach (UserInfo.Course.Folio fol in c.folios)
                    //{
                    //    Debug.Log("<color=" + (fol.seen == 1 ? "black" : fol.seen == 0 ? "yellow" : "red") + ">Folio: " + fol.name + " seen: " + fol.seen + "</color>");
                    //}
                    count++;
                }
                if (!c.everyNewsDone)
                    count++;
                if (count == 0)
                    newsFinal = true;
            }
            
        }

        if (newsFinal && dataDone && !everyThingDone)
        {
            everyThingDone = true;
            Debug.Log("<color=blue>FINAL</color>, seconds: " + stop.ElapsedMilliseconds / 1000f);
            //foreach (UserInfo.Course c in courses)
            //{
            //    foreach (UserInfo.Course.Forum f in c.forums)
            //    {
            //        Debug.Log("<color=" + (f.seen == 1 ? "black" : f.seen == 0 ? "yellow" : "red") + ">Forum: " + f.name + " seen: " + f.seen + "</color>");
            //    }
            //    foreach (UserInfo.Course.Folio fol in c.folios)
            //    {
            //        Debug.Log("<color=" + (fol.seen == 1 ? "black" : fol.seen == 0 ? "yellow" : "red") + ">Folio: " + fol.name + " seen: " + fol.seen + "</color>");
            //    }
            //}
        }
    }

    public Boolean getURunning = false;
    public Boolean forumsDone = false;
    public Boolean foliosDone = false;
    public Boolean topicsDone = false;
    public Boolean dataDone = false;
    public Boolean updatesDone = false;
    public Boolean everyThingDone = false;
    public Boolean newsFinal = false;

    /// <summary>
    /// Gets the user.
    /// </summary>
    /// <returns>The user class.</returns>
    public UserInfo.UserData getUser()
    {
        while(getURunning);

        getURunning = true;
        if (user == null)
        {
            user = new UserInfo.UserData();
            Debug.Log("NEW USER");
        }
        getURunning = false;
        
        return user;
    }

    /// <summary>
    /// Gets the courses.
    /// </summary>
    /// <returns>List of the saved courses.</returns>
    public List<UserInfo.Course> getCourses()
    {
        if (courses == null)
        {
            courses = new List<UserInfo.Course>();
            Debug.Log("NEW Course");
        }
        return courses;
    }

    /// <summary>
    /// Gets the course by identifier.
    /// </summary>
    /// <param name="id">The identifier of the desired course.</param>
    /// <returns>The desired course.</returns>
    public UserInfo.Course getCourseById(int id)
    {
        foreach (UserInfo.Course co in courses)
        {
            if (co.id == id)
                return co;
        }
        return null;
    }

    // dados do User
    /*
      * Recebe informacao basica do utilizador EM JSON
      */
    /// <summary>
    /// Receives the user.
    /// </summary>
    /// <param name="u">The User information.</param>
    public void receiveUser(jsonValues.users u)
    {
        user.receiveUsers(u);
    }

    // DADOS DO COURSE
    /*
     * Recebe informacao basica das cadeiras que o utilizador esta inscrito EM JSON
     */
    /// <summary>
    /// Receives the courses.
    /// </summary>
    /// <param name="c">List of Courses and their characteristics.</param>
    /// <param name="multi">if set to <c>true</c> The program considers multiple courses, if set to <c>false</c> the program filters the courses and only gets the one desired.</param>
    /// <param name="courseId">The course identifier of the course desired (WebGL).</param>
    public void receiveCourses(List<jsonValues.Courses> c, Boolean multi, int courseId)
    {
        UserInfo.Course template;


        foreach (jsonValues.Courses co in c)
        {
            template = new UserInfo.Course();
            lock (template)
            {
                if (!multi) // Caso seja uma cadeira
                {
                    if (co.id == courseId)
                    {
                        
                        template.id = co.id;
                        template.idNumber = co.idnumber;
                        template.shortName = co.shortname;
                        template.fullName = co.fullname;
                        template.summary = UserInfo.Course.HtmlDecode(co.summary);
                        template.startdate = co.startdate;
                        template.visible = co.visible;
                        courses.Add(template);
                    }
                }

                else
                {
                    
                    template.id = co.id;
                    template.idNumber = co.idnumber;
                    template.shortName = co.shortname;
                    template.fullName = co.fullname;
                    template.summary = UserInfo.Course.HtmlDecode(co.summary);
                    template.startdate = co.startdate;
                    template.visible = co.visible;
                    courses.Add(template);
                }
            }
        }
        gotCourses = true;
    }

    /*
        * 
        */
    /// <summary>
    /// Receives the grades.
    /// </summary>
    /// <param name="g">List of grades of a user.</param>
    public void receiveGrades(List<jsonValues.Grades> g)
    {
        UserInfo.Course template;
        double n;
        bool isNumeric;
        foreach (jsonValues.Grades gr in g)
        {

            template = getCourseById(gr.courseid);
            if (template != null)
            {
                isNumeric = double.TryParse(gr.grade, out n);
                if (gr.grade != null && isNumeric)
                {
                    template.grade = double.Parse(gr.grade);
                }


            }
        }

    }

    /// <summary>
    /// Gets the course ids.
    /// </summary>
    /// <returns> List of IDs of the courses</returns>
    public List<int> getCourseIds()
    {
        List<int> lis = new List<int>();
        foreach (UserInfo.Course c in courses)
            lis.Add(c.id);
        lis.Sort();
        
        return lis;

    } 

}
