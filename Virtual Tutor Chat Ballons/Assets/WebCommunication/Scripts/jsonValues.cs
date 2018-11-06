using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class jsonValues : MonoBehaviour
    {
        /// <summary>
        /// User information given by Moodle
        /// </summary>
        [Serializable]
        public class users
        {
            public int id;
            public string username;
            public string fullname;
            public string email;
            public string lang;
            public string auth;
            public int firstaccess;
            public int lastaccess;
            //public List<preferences> pref ;
        }

        /// <summary>
        /// Course information given by Moodle
        /// </summary>
        [Serializable]
        public class Courses
        {
            public int id;
            public string shortname;
            public string fullname;
            public int idnumber;
            public int visible;
            public String summary;
            public int startdate;
            public List<assignments> assignments;
            
            //public int startdate;
            //public int enddata;

        }

        /// <summary>
        /// Grade in a Course given by Moodle
        /// </summary>
        [Serializable]
        public class Grades
        {
            public int courseid;
            public string grade;
            public string graderaw;
        }

        /// <summary>
        /// Course Topic structure given by Moodle
        /// </summary>
        [Serializable]
        public class Topics
        {
            public int id;
            public string name;
            public int visible;
            public String summary;
            public String uservisible;
            public List<Modules> modules;
            public String availabilityinfo;
            //public int startdate;
            //public int enddata;

            public int majorNews;

        }

        /// <summary>
        /// Course Module structure given by Moodle
        /// </summary>
        [Serializable]
        public class Modules
        {
            public int id;
            public string url;
            public string name;
            public int visible;
            public String description;
            public int uservisible;
            public List<Contents> contents;
            public int visibleoncoursepage;
            //public int startdate;
            //public int enddata;

        }

        /// <summary>
        /// Course content information given by Moodle
        /// </summary>
        [Serializable]
        public class Contents
        {
            public int filesize;
            public int timecreated;
            public int timemodified;
            public string type;
            public string filename;
            public string filepath;
            public string fileurl;
            public string author;
            public int userid;
            public int isexternalfile;
            public String summary;
            public String uservisible;
            
            //public int startdate;
            //public int enddata;

        }

        /// <summary>
        /// Course groups information given by Moodle
        /// </summary>
        [Serializable]
        public class groups
        {
            public int id;
            public String name;
        }

        /// <summary>
        /// User grade structure information given by Moodle
        /// </summary>
        [Serializable]
        public class usergrades
        {
            public int courseid;
            public int userid;
            public String userfullname;
            public List<gradeitems> gradeitems;
        }

        /// <summary>
        /// Information about each grade item in the report given by Moodle
        /// </summary>
        [Serializable]
        public class gradeitems
        {
            public int id;
            public int cmid;
            public String itemmodule;
            public string itemtype;
            public String itemname;
            public String graderaw;
            public int categoryid;
            public int iteminstance;
            // in case the value is null, this happens when the grade wasn't given
            public int gradedatesubmitted;
            public int gradedategraded;
            public string gradeformatted;
            public int grademin;
            public int grademax;
            public String weightformatted;
            public String percentageformatted;
            public String feedback;
            public String status; // optional
            public string rangeformatted;
            
            public String toString()
            {
                if(!string.IsNullOrEmpty(graderaw))
                return "Cmid: " + cmid + " name: " + itemname + " type: " + itemtype + " module: " + itemmodule + " Grade: " + graderaw + " from " + grademin + "-" +grademax+ " = " + percentageformatted;
                else
                return "Cmid: " + cmid + " name: " + itemname + " type: " + itemtype + " module: " + itemmodule + " "+ grademin + "-" + grademax ;
            }
        }

        /// <summary>
        /// Assignment(e-folio) information given by Moodle
        /// </summary>
        [Serializable]
        public class assignments
        {
            // UTILIZADO NO mod_assign_get_assignments
            public int id;
            public int cmid;
            public int course;
            public String name;
            public int nosubmissions; // se houve submissoes
            public int duedate;
            public int allowsubmissionsfromdate; // data que o aluno pode fazer submissoes
            public double grade; // grademax
            public int cutoffdate; // prazo limite

            // Utilizado no mod_assign_get_grades
            public int assignmentid;
            public List<grades> grades;
        }

        /// <summary>
        /// Information about each attempt on a assignment given by Moodle
        /// </summary>
        [Serializable]
        public class grades
        {
            public int id;
            public int userid; // verificar se eh o do aluno
            public int attemptnumber; // a tentativa nº
            public String grade; // if -1 entao nao foi dada
            public int timecreated; // submissao do aluno?
            public int timemodified; // avaliacao do professor?
        }


        // usado no mod_quiz_get_quizzes_by_courses
        /// <summary>
        /// Quizz information given by Moodle (Not Used)
        /// </summary>
        [Serializable]
        public class quizzes
        {
            public int id;
            public int course;
            public int coursemodule;
            public String name;
            public String intro;
            public List<introfiles> introfiles;

            public int timeopen; // Optional The time when this quiz opens. (0 = no restriction.)
            public int timeclose; //Optional The time when this quiz closes. (0 = no restriction.)
            public int timelimit; // Optional The time limit for quiz attempts, in seconds.
            public String overduehandling; //Optional //The method used to handle overdue attempts.'autosubmit', 'graceperiod' or 'autoabandon'.
            public int attempts;
            public double grade;
        }

        /// <summary>
        /// Information about introduced files given by Moodle
        /// </summary>
        [Serializable]
        public class introfiles
        {
            public String filename;
            public String filepath;
            public int filesize;
            public String fileurl;
            public int timemodified;
            public int isexternalfile;
        }

        // usado no core_course_get_updates_since
        /// <summary>
        ///Instance of updates given by Moodle
        /// </summary>
        [Serializable]
        public class instances
        {
            public String contextlevel; //the context level
            public int id;  //Instance id
            public List<updates> updates;
        }

        /// <summary>
        /// Update information given by Moodle
        /// </summary>
        [Serializable]
        public class updates
        {
            public String name; //Name of the area updated.
            public int timeupdated; //Optional Last time was updated
            public List<int> itemids; // Optional The ids of the items updated
        }

        /// <summary>
        /// Forum structure information given by Moodle
        /// (Forum contains discussions that contains posts)
        /// </summary>
        [Serializable]
        public class forums
        {
            public int cmid;
            public int id; //Forum id
            public String type; //The forum type
            public String name; //Forum name
            public String intro;  //The forum intro
            public List<introfiles> introfiles;

        }

        /// <summary>
        /// Discussion structure information given by Moodle
        /// </summary>
        [Serializable]
        public class discussions
        {
            public int id; //Post id -> temporariamente não tem utilidade
            public string name; //Discussion name
            public int timemodified;  //Time modified
            public int discussion; // discussion id
            public String subject; //subject of message
            public String message; //message posted
            public String userfullname; //Post author full name
            public String usermodifiedfullname; //Post modifier full name
            public int created;//Creation time
            public int modified;//Time modified
            public int userid; // id of user who created the discussion

        }

        /// <summary>
        /// Post information given by Moodle
        /// </summary>
        [Serializable]
        public class posts
        {
            public int id;//Post id
            public int discussion;//Discussion id
            public int parent;//Parent id
            public int userid;//User id
            public int created;//Creation time
            public int modified;//Time modified
            public String subject;//The post subject
            public String message;//The post message
        }

        /// <summary>
        /// Warning information given by Moodle (not used)
        /// </summary>
        [Serializable]
        public class warnings
        {
            public int itemid;
            public String warningcode;
            public String message;
        }


        //usado no core_notes_get_course_notes
        /// <summary>
        /// List of Note information given by Moodle (test only)
        /// </summary>
        [Serializable]
        public class notes
        {
            public List<LNotes> sitenotes;
            public List<LNotes> coursenotes;
            public List<LNotes> personalnotes;

            public int id;
            public double aproveitamento;
            public double assiduidade;
            public String tutorname;
        }

        /// <summary>
        /// Note information given by Moodle
        /// </summary>
        [Serializable]
        public class LNotes
        {
            public int id;
            public int courseid;
            public int userid;
            public String content;
            public int created;
            public int lastmodified;
            public String publishstate;
        }


        // USADO NA DB
        /// <summary>
        /// Login information given by DB
        /// </summary>
        [Serializable]
        public class logins
        {
            public int aluno;//id aluno
            public int course; // id da cadeira
            public int login; // timestamp do login
            public int seq; // n sequencia de login
        }

        /// <summary>
        /// Threshold information given by DB
        /// </summary>
        [Serializable]
        public class phrases
        {
            public String identifier;
            public String assiduidade;
            public String aproveitamento;
            public String frase;
        }

        /// <summary>
        /// Tutor information given by DB
        /// </summary>
        [Serializable]
        public class tutor
        {
            public int userid;
            public int courseid;
            public int tutorid;
        }

        /// <summary>
        /// Module viewed by the user given by DB, using the replication of the log table of Moodle DB
        /// </summary>
        [Serializable]
        public class modulesViewed
        {
            public String component;
            public String objecttable;
            public int contextinstanceid;
            public int userid;
            public int courseid;
            public int timecreated;
        }

        /// <summary>
        /// Question information given by DB
        /// </summary>
        [Serializable]
        public class questions
        {
            public int userid;
            public int courseid;
            public int time;
            public String question;
        }

    /// <summary>
    /// Values regarding student performance given by DB
    /// </summary>
    [Serializable]
    public class dbValues
    {
        public int courseId;

        public int post_high;
        public int post_low;
        public int login_high;
        public int login_low;
        public int aprov_high;
        public int aprov_low;

        public int startCourseDate;

        public int login_importance;
        public int post_importance;

        public int assid_low;
        public int assid_high;
        //public dbValues()
        //{
        //    post_high = 0;
        //    post_low = 0;
        //    login_high = 0;
        //    login_low = 0;
        //    aprov_high = 0;
        //    aprov_low = 0;
        //}

        public String toString()
        {
            return "post_high " + post_high + "post_low " + post_low + "\nlogin_high " + login_high + "login_low " + login_low + "\naprov_high " + aprov_high + "aprov_low " + aprov_low + "\n"
                + "loginImp " + login_importance + "postImp " + post_importance + "\n" + "startCourseDate " + startCourseDate;
        }

        public void copyValues(dbValues other)
        {
            other.post_high = this.post_high;
            other.post_low = post_low;
            other.aprov_low = this.aprov_low;
            other.aprov_high = this.aprov_high;
            other.login_high = this.login_high;
            other.login_low = this.login_low;
            other.startCourseDate = this.startCourseDate;
            other.login_importance = this.login_importance;
            other.post_importance = this.post_importance;
            other.assid_low = this.assid_low;
            other.assid_high = this.assid_high;
        }
    }

    // Use this for initialization
    void Start()
        {

        }
    
}