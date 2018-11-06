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
        public DateTime datelast;
        //current localtime
        DateTime localTime = DateTime.Now;
        TimeSpan diff;
        public Boolean readyForRead = false;

        //informacao relativa as cadeiras do user
        public List<Course> courses = new List<Course>();

        //public Course course = new Course();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public String giveData()
        {
            StringBuilder filtered = new StringBuilder();
            //filtered.Append("Tempo: " + localTime.ToLocalTime() + "\n");
            diff = DateTime.UtcNow.Subtract(datelast);
            // dados do user
            if (lang.Equals("en"))
            {
                if (localTime.ToLocalTime().Hour < 12)
                    filtered.Append("Bom dia " + fullName + "!\n");
                else if (localTime.ToLocalTime().Hour >= 12 && localTime.ToLocalTime().Hour < 20)
                    filtered.Append("Boa tarde " + fullName + "!\n");
                else
                    filtered.Append("Boa noite " + fullName + "!\n");
                //filtered.Append("First time you entered in Moodle was: " + datefirst.ToLocalTime() + "\n");
                //filtered.Append("A última vez que entrou no moodle foi: " + datelast.ToLocalTime() + "\n");

                StringBuilder sb = new StringBuilder("Não vem ao Moodle da disciplina " + getCourse().fullName + " há ");

                if(diff.Hours !=0)
                {
                    sb.Append(diff.Hours + " horas ");
                }

                if(diff.Minutes != 0)
                {
                    sb.Append(diff.Minutes + " minutos");
                }

                if (diff.Seconds !=0)
                {
                    sb.Append(" e " + diff.Seconds + " segundos");
                }

                filtered.Append(sb.ToString() + "!");
                
                //filtered.Append("Não vem ao Moodle há: " + diff.Hours + " horas " + diff.Minutes + " minutos e " + diff.Seconds + " segundos"  + "\n");

                //foreach (Course c in courses)
                //{
                //    foreach (Course.Forum f in c.forums)
                //    {
                //        foreach (Course.Discussions d in f.discussions)
                //        {
                //            foreach (Course.Posts p in d.posts)
                //            {

                //                if (p.userid == id)
                //                {
                //                    DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                //                    date = date.AddSeconds(Convert.ToInt32(p.created));
                //                    filtered.Append("\nOn Forum " + f.name + " There's the Discussion " + d.name + " on which you posted: " + p.message + " at " + date.ToLocalTime());
                //                }
                //            }
                //        }
                //    }
                //}
            }
            else
            {
                if (localTime.ToLocalTime().Hour < 12)
                    filtered.Append("Bom dia " + fullName + "\n");
                else if (localTime.ToLocalTime().Hour >= 12 && localTime.ToLocalTime().Hour < 20)
                    filtered.Append("Boa tarde " + fullName + "\n");
                else
                    filtered.Append("Boa noite " + fullName + "\n");

                filtered.Append("Informação de cursos:\n");

                
            }

            //filtered.Append("efolios:\n");
            //foreach (Course c in courses)
            //{
            //    filtered.Append(c.fullName + "\n");
            //    filtered.Append(c.foliosToString());
            //    filtered.Append("---------------\n");
            //}

            return filtered.ToString();
        }

        // METODOS PARA CONVERTER EM FORMATO LEGIVEL O CONTEUDO

        public String courseDisplay(int courseId)
        {
            Course c = getCourse();

            StringBuilder s = new StringBuilder();

            if(c.visible == 1)
            {
                if (lang.Equals("en"))
                {
                    //s.Append(c.fullName + "\n");
                    //s.Append("ID:" + c.id + "\n");
                    s.Append("Sumary: " + c.summary + "\n");
                    if (c.grade == -1)
                        s.Append("No grade was yet given\n");
                    else
                        s.Append("Grade: " + c.grade + "\n");
                    //s.Append(c.toString());
                   
                }


                else
                {
                    //s.Append("ID da cadeira:" + c.id + "\n");
                    //s.Append("Nome da cadeira: " + c.fullName + "\n");
                    //s.Append("Nomenclatura: " + c.shortName + "\n");
                    //s.Append("Descricao: " + c.summary + "\n");
                    if (c.grade == -1)
                        s.Append("Avaliacao ainda nao foi feita\n");
                    else
                        s.Append("Nota: " + c.grade + "\n");
                    //s.Append(c.toString());
                }
                
                s.Append("NOTAS:\n");
                
                foreach (Course.Folio f in c.folios) {
                    
                    if (f.duedate < DateTime.UtcNow && c.getHighestGradeFolio(f.id) == -2)
                    {
                        s.AppendLine(f.name + " ja passou do prazo e não foi entregue nada :'(");
                    }
                    else if (f.duedate < DateTime.UtcNow && c.getHighestGradeFolio(f.id) ==-1)
                    {
                        s.AppendLine(f.name + " ja passou do prazo mas ainda não foi avaliada");
                    }
                    else if (f.duedate < DateTime.UtcNow)
                    {
                        s.AppendLine(f.name + " " + c.getHighestGradeFolio(f.id) + " em " + f.grademax);
                    }
                    else if (f.duedate > DateTime.UtcNow && f.attempgrade.Count > 0)
                    {
                        s.AppendLine(f.name + " ainda tem tempo antes da entrega, reve a tua entrega com calma");
                    }
                    else if (f.duedate > DateTime.UtcNow)
                    {
                        s.AppendLine(f.name + " ainda tem tempo antes da entrega, toma cuidado");
                    }

                }
                List<Course.gradeStruct> grades = c.getGradeValues();
                
                // TODO FAZER VECTORES E ANGULOS AQUI
                Vector2 v1 = new Vector2(0,0),v2 = new Vector2(0,0),vFirst = new Vector2(0,0), vLast = new Vector2(0, 0);
                LinkedList<float> declives = new LinkedList<float>();
                float sign;
                float angle;
                double bas = 0, superior = 0;
                foreach (Course.gradeStruct g in grades)
                {
                    if (g.dueDate<DateTime.UtcNow && g.isGraded) {
                        //Debug.Log("FOLIO: " + g.name + " grade:" + g.grade + " out of " + g.gradeMax + " duedate:" + g.dueDate.ToLocalTime() + " POSITION:" + g.position);
                        if (g.position == 0)
                        {
                            v1 = new Vector2(g.position++,(float)(g.grade * (20 / g.gradeMax)));
                           
                            vFirst = new Vector2(v1.x,v1.y);
                        }
                        else
                        {
                            if (g.position % 2 != 0)
                            {
                                v2 = new Vector2(g.position++, (float)(g.grade * (20 / g.gradeMax)));

                                
                                //sign = (v2.y < v1.y) ? -1.0f : 1.0f;
                                angle = Mathf.Atan2(v2.y - v1.y, v2.x - v1.x) * 180 / Mathf.PI;
                                declives.AddLast(angle);
                                vLast = new Vector2(v2.x, v2.y);
                            }
                            else
                            {
                                v1 = new Vector2(g.position++, (float)(g.grade * (20 / g.gradeMax)));
                                //sign= (v1.y < v2.y) ? -1.0f : 1.0f;
                                
                                angle = Mathf.Atan2(v1.y - v2.y, v1.x - v2.x) * 180 / Mathf.PI;
                                declives.AddLast(angle);
                                vLast = new Vector2(v1.x, v1.y);
                            }

                        }
                        bas += g.gradeMax;
                        superior += g.grade;
                    }
                    
                }

                foreach (float d in declives)
                {
                    s.Append("DECLIVE: " + d + "\n");
                }
                //sign = (vLast.y < vFirst.y) ? -1.0f : 1.0f;
                angle = Mathf.Atan2(vLast.y - vFirst.y, vLast.x - vFirst.x) * 180 / Mathf.PI;
                s.AppendLine("Performance geral: " + angle);
                s.AppendLine(superior + "/" + bas + "=" + (superior / bas));

            }
            
            return s.ToString();
        }

        public String TopicDisplay(int courseId, int topidID)
        {

            Course c = getCourse();
            if (c == null)
                return null;

            StringBuilder s = new StringBuilder();

            s.Append(c.TopicDisplay(topidID,lang));
            
            return s.ToString();
        }

        public String ModuleDisplay(int courseId, int topicId, int moduleId)
        {
            Course c = getCourse();
            if (c == null)
                return null;

            StringBuilder s = new StringBuilder();

            s.Append(c.ModuleDisplay(topicId,moduleId, lang));

            return s.ToString();
        }

        public String FolioDisplay(int courseId, int folioID)
        {

            Course c = getCourse();
            if (c == null)
                return null;

            StringBuilder s = new StringBuilder();

            s.Append(c.FolioDisplay(folioID));

            return s.ToString();
        }


        public Course getCourse()
        {
            return courses[0];
        }




        // METODOS PARA FILTRAR WEBSERVICE RESPONSES

        //filtrar dados de core_user_get_users_by_id em XML
        public void filterUserData(String content)
        {
            String[] filter = content.Split('\n');
            String[] variable;
            Boolean isPerson = true;
            //StringBuilder filtered = new StringBuilder();
            //Time of first access
            DateTime datefirst = new DateTime();
            //time of last access
            DateTime datelast;
            //current localtime
            DateTime localTime = DateTime.Now;

            for (int l = 0; l < filter.Length; l++)
            {
                if (isPerson)
                {
                    //filtrar ID
                    if (filter[l].Contains("<KEY name=\"id\">"))
                    {
                        variable = filter[l].Split(new[] { "<KEY name=\"id\"><VALUE>" }, StringSplitOptions.None);
                        variable = variable[1].Split(new[] { "</VALUE>" }, StringSplitOptions.None);
                        //filtered.Append("O ID do utilizador é: " + variable[0] + "\n");
                        id = Int32.Parse(variable[0]);
                    }

                    //filtrar o Nome Completo
                    if (filter[l].Contains("<KEY name=\"fullname\">"))
                    {
                        variable = filter[l].Split(new[] { "<KEY name=\"fullname\"><VALUE>" }, StringSplitOptions.None);
                        variable = variable[1].Split(new[] { "</VALUE>" }, StringSplitOptions.None);
                        //filtered.Append("O Nome do utilizador é: " + variable[0] + "\n");
                        fullName = variable[0];
                    }

                    //filtrar o Nome de utilizador
                    if (filter[l].Contains("<KEY name=\"username\">"))
                    {
                        variable = filter[l].Split(new[] { "<KEY name=\"username\"><VALUE>" }, StringSplitOptions.None);
                        variable = variable[1].Split(new[] { "</VALUE>" }, StringSplitOptions.None);
                        //filtered.Append("O Nome do utilizador é: " + variable[0] + "\n");
                        userName = variable[0];
                    }

                    //filtrar o email
                    if (filter[l].Contains("<KEY name=\"email\">"))
                    {
                        variable = filter[l].Split(new[] { "<KEY name=\"email\"><VALUE>" }, StringSplitOptions.None);
                        variable = variable[1].Split(new[] { "</VALUE>" }, StringSplitOptions.None);
                        //filtered.Append("O email do utilizador é: " + variable[0] + "\n");
                        email = variable[0];
                    }

                    //filtrar a linguagem
                    if (filter[l].Contains("<KEY name=\"lang\">"))
                    {
                        variable = filter[l].Split(new[] { "<KEY name=\"lang\"><VALUE>" }, StringSplitOptions.None);
                        variable = variable[1].Split(new[] { "</VALUE>" }, StringSplitOptions.None);
                        //filtered.Append("O email do utilizador é: " + variable[0] + "\n");
                        lang = variable[0];
                    }

                    //filtrar o firstaccess
                    if (filter[l].Contains("<KEY name=\"firstaccess\">"))
                    {
                        variable = filter[l].Split(new[] { "<KEY name=\"firstaccess\"><VALUE>" }, StringSplitOptions.None);
                        variable = variable[1].Split(new[] { "</VALUE>" }, StringSplitOptions.None);
                        datefirst = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        datefirst = datefirst.AddSeconds(Convert.ToInt32(variable[0]));
                        //filtered.Append("O primeiro acesso do utilizador foi: " + datefirst.ToLocalTime() + "\n");
                        this.datefirst = datefirst;
                    }

                    //filtrar o lastaccess e calcular o tempo total
                    if (filter[l].Contains("<KEY name=\"lastaccess\">"))
                    {
                        variable = filter[l].Split(new[] { "<KEY name=\"lastaccess\"><VALUE>" }, StringSplitOptions.None);
                        variable = variable[1].Split(new[] { "</VALUE>" }, StringSplitOptions.None);
                        datelast = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        datelast = datelast.AddSeconds(Convert.ToInt32(variable[0]));
                        this.datelast = datelast;
                        //filtered.Append("O ultimo acesso foi: " + datelast.ToLocalTime() + "\n");

                        //TimeSpan delta = datelast - datefirst;
                        //TODO NEEDS WORK DONE
                        //filtered.Append("Inscrito Há: " + delta.Days + " dias e " + delta.Hours + " horas" + "\n"); 

                    }

                }
                
            }

        }
   
        // JSON BELOW

        /*
       * Recebe informacao basica do utilizador EM JSON
       */
        public void receiveUsers(jsonValues.users u)
        {
            userName = u.username;
            fullName = u.fullname;
            email = u.email;
            lang = u.lang;

            
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(Convert.ToInt32(u.firstaccess));
            
            this.datefirst = date;
            date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(Convert.ToInt32(u.lastaccess));
            
            datelast = date;
            
        }

        /*
        * Recebe informacao basica das cadeiras que o utilizador esta inscrito EM JSON
        */
        public void receiveCourses(List<jsonValues.Courses> c, Boolean multi, int courseId)
        {
            Course template;


            foreach (jsonValues.Courses co in c)
            {
                if (!multi) // Caso seja uma cadeira
                {
                    if (co.id == courseId)
                    {
                        template = new Course();
                        template.id = co.id;
                        template.idNumber = co.idnumber;
                        template.shortName = co.shortname;
                        template.fullName = co.fullname;
                        template.summary = Course.HtmlDecode(co.summary);
                        template.lang = lang;
                        template.visible = co.visible;
                        courses.Add(template);
                    }
                }

                else
                {
                    template = new Course();
                    template.id = co.id;
                    template.idNumber = co.idnumber;
                    template.shortName = co.shortname;
                    template.fullName = co.fullname;
                    template.summary = Course.HtmlDecode(co.summary);
                    template.lang = lang;
                    template.visible = co.visible;
                    courses.Add(template);
                }

            }
        }

        /*
         * 
         */
        public void receiveGrades(List<jsonValues.Grades> g)
        {
            Course template;
            double n;
            bool isNumeric;
            foreach (jsonValues.Grades gr in g)
            {

                template = getCourse();
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

        public void doneWriting()
        {
            readyForRead = true;
        }

        public void needsWriting()
        {
            readyForRead = false;
        }
    }
}
