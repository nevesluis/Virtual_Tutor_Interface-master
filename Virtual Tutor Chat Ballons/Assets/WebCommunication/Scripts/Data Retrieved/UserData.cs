using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UserInfo
{
    public class UserData : MonoBehaviour
    {


        public int id; // n de identificacao do user
        public String userName; // username disposto no moodle
        public String fullName; // nome completo do user
        public String email; // email do user
        public String lang; // languagem que o user tem no moodle?

        // informacao potencialmente util
        public String imgSmall; // link para a imagem em tamanho pequeno do user
        public String imgProf; // imagem de perfil do user
                               //Time of first access
        public DateTime datefirst = new DateTime();
        //time of last access
        public int datelast;
        //current localtime
        DateTime localTime = DateTime.Now;
        TimeSpan diff;
        public Boolean readyForRead = false;

        //informacao relativa as cadeiras do user
        //public List<Course> courses = new List<Course>();

        //public Course course = new Course();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        // METODOS PARA FILTRAR WEBSERVICE RESPONSES


        // JSON BELOW

        /*
       * Recebe informacao basica do utilizador EM JSON
       */

        /// <summary>
        /// Receives the user information.
        /// </summary>
        /// <param name="u">The user information.</param>
        public void receiveUsers(jsonValues.users u)
        {
            id = u.id;
            
            userName = u.username;

            fullName = u.fullname;
            email = u.email;
            lang = u.lang;

            
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(Convert.ToInt32(u.firstaccess));
            
            this.datefirst = date;
            date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(Convert.ToInt32(u.lastaccess));
            
            datelast = u.lastaccess;
            
        }



        /// <summary>
        /// Symbolizes the confirmation that user has all the information.
        /// </summary>
        public void doneWriting()
        {
            readyForRead = true;
        }

        /// <summary>
        /// Indicates the user has new information that is not yet integrated into the application.
        /// </summary>
        public void needsWriting()
        {
            readyForRead = false;
        }
    }
}
