using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
//using VT;

namespace UserInfo
{
    public class Course
    {
        
//        int position = 0;

        public Boolean avaliacaoContinua = false;
        public Boolean checkGradeReport = false;
        public Boolean reportViewed = false;
        public Boolean newsAttempt = false;
        public Boolean updatesFinished = false;
        public Boolean everyNewsDone = false;
        public int nPostsDone = 0;
        public Boolean postsRetrieved = false;
        //class que define tudo da cadeira

        public int visible; // se a cadeira eh visivel ao user
        public int id; // verdadeiro ID
        public int idNumber; // ID dado para identificacao
        public String shortName; // nome curto
        public String fullName; // nome completo
        public int enrolledusercount; // numero de alunos inscritos
        public String summary; // descricao da cadeira
        public double grade = -1.00; // nota dada ao user
        public String lang;
        public int startdate;



        public LinkedList<group> groups = new LinkedList<group>();
        public List<Topic> topics = new List<Topic>();
        public List<Folio> folios = new List<Folio>();
        public List<Forum> forums = new List<Forum>();
        public LinkedList<jsonValues.gradeitems> reports = new LinkedList<jsonValues.gradeitems>();
       
        public Notes notes = new Notes();
        //valores relativos a cadeira
        
        public jsonValues.dbValues parameters = new jsonValues.dbValues();
        // valores relativos ao aluno
        public Decimal averageLoginSpace = 0;
        public int currentAprov;
        public int maxCurrentAprov;

        // Dados iniciados para corresponder aos sliders
        public float like;
        public float know;
        public float importance;

        //private List<Checkpoint> checkpoints = new List<Checkpoint>();
        //public List<Checkpoint> Checkpoints
        //{
        //    get
        //    {
        //        return this.checkpoints;
        //    }
        //    set
        //    {
        //        checkpoints = value;
        //    }
        //}

        public List<int> logins = new List<int>();
        
        public List<newsUpdate> news = new List<newsUpdate>();
        public List<newsUpdate> moduleNews = new List<newsUpdate>();
        public List<newsUpdate> forumNews = new List<newsUpdate>();
        public List<newsUpdate> folioNews = new List<newsUpdate>();
        public Hashtable actFormativas = new Hashtable();

        /// <summary>
        /// Structure the component and the information related to a news associated with it.
        /// </summary>
        public class newsUpdate
        {
            public object component;
            public int cmid;
            public int time;
            public String news; 
            
            public newsUpdate()
            {
                news = "";
                cmid = 0;
                time = 0;
                component = null;
            }
        }

        /// <summary>
        /// Structure that stores information for the e-fólios (Assignments).
        /// </summary>
        public class Folio
        {
            public int seen = 0; // -1 para novo e não visto, 0 para não visto, 1 para visto
            public int id;
            public int cmid;
            public int course;
            public String name;
            public int nosubmissions; // se houve submissoes
            public DateTime duedate;
            public DateTime allowsubmissionsfromdate; // data que o aluno pode fazer submissoes
            public double grademax; // grademax
            public DateTime cutoffdate; // prazo limite
            public int categoryid; // id da categoria
            // dados do modulo
            public int visible;
            public int uservisible;
            public int visibleOnCoursePage;

            public int topicId;

            // grade data
            public List<attemptgrade> attempgrade = new List<attemptgrade>();

            // Dados usados para notificar o aluno de entregas
            public Boolean isDue = true; // se o aluno entregou algo

            public String dueInformation;
            public String feedback;

            internal int grademin;
            internal double graderaw;
            internal string gradeformatted;
            internal string weightformatted;
            internal string percentageformatted;
            internal DateTime gradedategraded;
            internal DateTime gradedatesubmitted;
        }

        /// <summary>
        /// Structure that holds and attempt made by the student for an assignment.
        /// </summary>
        public class attemptgrade
        {
            public int id;
            public int attemptnumber; // a tentativa nº
            public Double grade; // if -1 entao nao foi dada
            public DateTime timecreated; // submissao do aluno?
            public DateTime timemodified; // avaliacao do professor?
        }

        /// <summary>
        /// Structure that stores the information of various types of objects, contained in Modules.
        /// </summary>
        public class contents
        {
            // caso seja um ficheiro
            public String type;
            public String fileName;
            public String summary;
            public int fileSize;
            public String filePath; // Location in Moodle
            public String fileUrl; // Download Link
            public int isExternalFile;

            public DateTime timeCreated;
            public DateTime timeModified;

            public int userId;
            public String author;
        }

        /// <summary>
        /// Structure that containes the information of a module that is contained in a topic.
        /// </summary>
        public class modules
        {
            public int id;
            public String url;
            public String name;
            public String description;
            public int visible;
            public int uservisible;
            public int visibleOnCoursePage;
            public String modname; // maybe useful
            public int seen = 0; // -1 para novo e não visto, 0 para não visto, 1 para visto
            public LinkedList<contents> contents = new LinkedList<contents>();
            public int topicId;

        }

        /// <summary>
        /// Structure that simulates a Topic in the course page.
        /// </summary>
        public class Topic
        {
            public int visible; // check
            public int userVisible; // check
            public int id; // check
            public String name; // CHECK
            public String summary; // check
            public int hiddenByNumSections; // maybe useful
            public String url;
            public String availabilityinfo;
            public LinkedList<modules> modules = new LinkedList<modules>();
            public int seen = 1; // -1 para novo e não visto, 0 para não visto, 1 para visto
        }

        /// <summary>
        /// Structure that holds the identification of a group.
        /// </summary>
        public class group
        {
            public int id;
            public String name;
        }

        /// <summary>
        /// Structure that holds the information of a post in a discussion.
        /// </summary>
        public class Posts
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
        /// Structure that holds the information of a discussion contained in a forum.
        /// </summary>
        public class Discussions
        {
            public int id;
            public string name; //Discussion name
            public int timemodified;  //Time modified X
            public String subject; //subject of message
            public String message; //message posted
            public String userfullname; //Post author full name
            public String usermodifiedfullname; //Post modifier full name
            public int created;//Creation time
            public int modified;//Time modified X
            public int userid; // id of user who created the discussion
            public List<Posts> posts;
        }

        /// <summary>
        /// Structure that simulates a Forum in the course page.
        /// </summary>
        public class Forum
        {
            public int id; //Forum id
            public int cmid;
            public String type; //The forum type
            public String name; //Forum name
            public String intro;  //The forum intro
            //public List<contents> introfiles;
            public List<Discussions> discussions;
            public int seen = 0; // -1 para novo e não visto, 0 para não visto, 1 para visto

            public int topicId;
        }



        /**
         * Devolve toda a info relevante ao curso
         * */
        /// <summary>
        /// Converts the course into a legible text.
        /// </summary>
        /// <returns>All the relevant information of the course</returns>
        public String toString()
        {
            StringBuilder response = new StringBuilder();
            foreach( Topic t in topics)
            {
               
                response.Append(TopicDisplay(t.id, lang));
                    foreach (modules m in t.modules)
                    {

                            response.Append(ModuleDisplay(t.id, m.id, lang));

                            foreach (contents c in m.contents)
                            {
                                response.Append("Content\n");
                                if (c != null)
                                {
                                    //response.Append(c.userId + "\n");
                                    response.Append("Author: " +c.author + "\n");
                                    //response.Append(c.type + "\n");
                                    response.Append("Name " +c.fileName + "\n");
                                    response.Append("URL: " +c.fileUrl + "\n");
                                    //response.Append(c.timeCreated + " \n");
                                }
                            }
                        
                    }
                
            }
            response.Append("FOLIOS\n");
            response.Append(foliosToString());
            
            return response.ToString();
        }

        // METODOS PARA CONVERTER EM FORMATO LEGIVEL O CONTEUDO

        /// <summary>
        /// Display the information about the topic.
        /// </summary>
        /// <param name="id">The identifier of the Topic.</param>
        /// <param name="lang">The language.</param>
        /// <returns>Information about the Topic.</returns>
        public String TopicDisplay(int id, String lang)
        {
            Topic t = GetTopic(id);
            
            if (t == null)
                return null;
            
            StringBuilder s = new StringBuilder();
            if(t.userVisible==1)
            {
               
                if (lang.Equals("en"))
                {
                    s.Append("Topic ID:" + t.id + "\n");
                    s.Append("Topic Name:" + t.name + "\n");
                    s.Append("Brief summary :" + t.summary + "\n");
                    s.Append("URL:" + t.url + "\n");
                }
                else
                {
                    s.Append("ID do tópico:" + t.id + "\n");
                    s.Append("nome:" + t.name + "\n");
                    s.Append("Breve sumário :" + t.summary + "\n");
                    s.Append("URL:" + t.url + "\n");
                }
                
            }
            
            return s.ToString();
        }

        /// <summary>
        /// Display the information about the module.
        /// </summary>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="id">The identifier of the module.</param>
        /// <param name="lang">The language.</param>
        /// <returns>Information about the module.</returns>
        public String ModuleDisplay(int topicId, int id, String lang)
        {
            Topic t = GetTopic(topicId);
            if (t == null)
                return null;
            modules m = getModule(topicId, id);
            if (m == null)
                return null;
           
            StringBuilder s = new StringBuilder();
            if (m.uservisible == 1)
            {
                if (lang.Equals("en"))
                {
                    s.Append("Name " + m.name + "\n");
                    s.Append("ID:" +m.id + "\n");
                    s.Append("URL: " + m.url + "\n");
                    if (m.description != null)
                        s.Append("Description: " + m.description + "\n");
                    else
                        s.Append("No description was given.\n");
                    foreach (contents c in m.contents)
                    {
                        s.Append("\nContent:\n");
                        if (c != null)
                        {
                            //response.Append(c.userId + "\n");
                            s.Append("Author: " + c.author + "\n");
                            //response.Append(c.type + "\n");
                            s.Append("Name " + c.fileName + "\n");
                            s.Append("URL: " + c.fileUrl + "\n");
                            //response.Append(c.timeCreated + " \n");
                        }
                    }

                }
                else
                {
                    s.Append("Nome" + m.name + "\n");
                    s.Append("ID:" + m.id + "\n");
                    s.Append("URL: " + m.url + "\n");
                    s.Append("Descrição: " + m.description + "\n");
                    foreach (contents c in m.contents)
                    {
                        s.Append("Conteudo:\n");
                        if (c != null)
                        {
                            //response.Append(c.userId + "\n");
                            s.Append("Autor: " + c.author + "\n");
                            //response.Append(c.type + "\n");
                            s.Append("Nome " + c.fileName + "\n");
                            s.Append("URL: " + c.fileUrl + "\n");
                            //response.Append(c.timeCreated + " \n");
                        }
                    }
                }

            }
            
            return s.ToString();
        }

        /// <summary>
        /// Display the information about the e-fólio.
        /// </summary>
        /// <param name="folioId">The folio identifier.</param>
        /// <returns>Information about the module.</returns>
        public String FolioDisplay(int folioId)
        {
            TimeSpan diff;
            StringBuilder s = new StringBuilder();
            Folio f = getFolio(folioId);
            diff = DateTime.UtcNow.Subtract(f.duedate);
            if (diff.TotalSeconds < 0) // Data ainda nao passou
            {
                diff = -diff;
                s.Append("Faltam " + diff.Days + " dias, " + diff.Hours + " horas e " + diff.Minutes + " minutos. ");
            }
            else
            {
                if (f.graderaw != -1)
                    s.AppendLine("Nota já atribuída.");
                else
                {
                    s.Append("Nota não atribuída.");
                }
            }

            //if (lang.Equals("en")) {
            //    s.Append("ID: " + f.id + "\n");
            //    s.Append("Name: " + f.itemName + "\n");
            //    s.Append("Status:" + f.status + "\n");
            //    if (f.gradedatesubmitted.Equals(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)))
            //        s.Append("YOU HAVENT SUBMITTED ANYTHING\n");
            //    else
            //        s.Append("Submitted date: " + f.gradedatesubmitted.ToLocalTime() + "\n");
            //    if (f.gradeGiven) {
            //        s.Append("Grade: " + f.graderaw + " out of " + f.grademax + "= " + f.percentageFormatted + "\n");
            //        s.Append("WAS GRADED AT: " + f.gradedategraded.ToLocalTime());
            //    }
            //    else
            //    {
            //        s.Append("Grade not given\n");
            //    }
            //}
            //else
            //{
            //    s.Append("ID: " + f.id + "\n");
            //    s.Append("Nome: " + f.itemName + "\n");
            //    if (f.gradeGiven)
            //    {
            //        s.Append("Nota: " + f.graderaw + " de " + f.grademax + "= " + f.percentageFormatted + "\n");
            //    }
            //    else
            //    {
            //        s.Append("Falta de nota\n");
            //    }
            //}

            return s.ToString();
        }

        /// <summary>
        /// Calculates the current maximum value of aproveitamento.
        /// </summary>
        /// <returns>The maximum value the aproveitamento can take at the point in time.</returns>
        public int maxCurrentAproveitamento()
        {
            int result = 0;
            foreach(Folio f in folios)
            {
                if (f.graderaw != -1)
                {
                    result += (Convert.ToInt32(f.grademax) * (100 / Convert.ToInt32(f.grademax))); // pode nao ter chegado a data limite mas o professor ter avaliado
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the current value of aproveitamento.
        /// </summary>
        /// <returns>The value of aproveitamento the student has at the point in time.</returns>
        public int currentAproveitamento()
        {
            int result = 0;
            foreach (Folio f in folios)
            {
                if (f.graderaw != -1)
                {
                    result += Convert.ToInt32(f.graderaw * (100 / Convert.ToInt32(f.grademax))); // pode nao ter chegado a data limite mas o professor ter avaliado
                }
            }

            return result;
        }

        public class gradeStruct
        {
            public String name;
            public double grade;// -2 se nao entregou nada ainda, -1 se entregou mas nao foi avaliado
            public double gradeMax;
            public int position;
            public DateTime dueDate;
            public Boolean isGraded = true;

            public gradeStruct(String name, double grade, double gradeMax, int position, DateTime dueDate)
            {
                this.name = name;
                this.grade = grade;
                if (grade == -1)
                    isGraded = false;
                this.gradeMax = gradeMax;
                this.position = position;
                this.dueDate = dueDate;
            }
        }

        /// <summary>
        /// Gets the information about the grades.
        /// </summary>
        /// <returns>List of structures that contain the grades.</returns>
        public List<gradeStruct> getGradeValues()
        {
            List<gradeStruct> grades = new List<gradeStruct>();
            gradeStruct template;
            foreach(Folio f in folios)
            {
                template = new gradeStruct(f.name,getHighestGradeFolio(f.id),f.grademax,0,f.duedate);

                foreach(gradeStruct g in grades)
                {

                    if (template.dueDate < g.dueDate && (template.position > g.position || template.position == 0))
                    {
                        template.position = g.position;
                        g.position++;
                    }
                    else if (template.dueDate < g.dueDate && template.position < g.position)
                        g.position++;
                }
                if (template.position == 0)
                    template.position = grades.Count;
                grades.Add(template);
            }
            grades.Sort((s1,s2) => s1.position.CompareTo(s2.position));
            return grades;
        }


        /// <summary>
        /// Gets the highest grade of the e-fólio.
        /// </summary>
        /// <param name="folioId">The folio identifier.</param>
        /// <returns>the highest grade of the e-fólio.</returns>
        public double getHighestGradeFolio(int folioId)
        {
            Folio f = getFolio(folioId);
            double grade = -1; 
            if(f!= null)
            {
                if (f.attempgrade.Count == 0)
                return -2; // NAO HOUVE ENTREGAS

                foreach(attemptgrade a in f.attempgrade)
                {
                    if (a.grade > grade)
                        grade = a.grade;
                }
            }
            

            return grade;
        }

        // Da toda a info dos folios em string
        /// <summary>
        /// Display the information about the e-fólios.
        /// </summary>
        /// <returns>Information about the e-fólios.</returns>
        public String foliosToString()
        {
            StringBuilder response = new StringBuilder();
            //foreach (Folio f in folios)
            //{
            //    response.Append("NAME: " + f.itemName + "\n");
            //    response.Append("MINIMO: " + f.grademin + "\n");
            //    response.Append("Maximo: " + f.grademax + "\n");
                
            //    if (f.gradeGiven)
            //    {
            //        response.Append("DATE: " + f.gradedategraded.ToLocalTime() + " value " + f.gradeformatted + "\n");
            //        response.Append(f.percentageFormatted + "\n");
            //    }
            //    else
            //        response.Append("Grade wasn't given yet.\n");
            //    response.Append("----------------\n");
            //}

            foreach(Folio f in folios)
            {
                if (f.visible>0)
                    response.AppendLine(f.dueInformation);
            }

            return response.ToString();
        }

        // Metodos para buscar valores

        /// <summary>
        /// Gets the topic.
        /// </summary>
        /// <param name="id">The Topic identifier.</param>
        /// <returns>The structure that contains the information about the topic.</returns>
        public Topic GetTopic(int id)
        {
            foreach (Topic to in topics)
            {

                if (to.id == id)
                   return to;
            }
            return null;
        }

        /// <summary>
        /// Gets the module.
        /// </summary>
        /// <param name="tId">The topic identifier.</param>
        /// <param name="id">The module identifier.</param>
        /// <returns>The structure that contains the information about the module.</returns>
        public modules getModule(int tId,int id)
        {
            Topic t = GetTopic(tId);
            
            foreach (modules mo in t.modules)
            {
                if (mo.id == id)
                {
                    return mo;
                }
            }
            return null;
            
        }

        /// <summary>
        /// Gets the module by identifier.
        /// </summary>
        /// <param name="id">The module identifier.</param>
        /// <returns>The structure that contains the information about the module.</returns>
        public modules getModuleById(int id)
        {
            modules placeholder;
            foreach(Topic t in topics)
            {
               placeholder= getModule(t.id, id);
                if (placeholder != null)
                    return placeholder;
            }
            return null;
        }

        /// <summary>
        /// Removes the module.
        /// </summary>
        /// <param name="id">The module identifier.</param>
        public void removeModule(int id)
        {
            
            modules m = getModuleById(id);
            
            foreach(Topic t in topics)
            {     
                if (t.modules.Contains(m))
                    t.modules.Remove(m);
            }

        }

        /// <summary>
        /// Gets the e-fólio.
        /// </summary>
        /// <param name="fId">The e-fólio identifier.</param>
        /// <returns>The structure that contains the information about the e-fólio.</returns>
        public Folio getFolio(int fId)
        {
            
            foreach(Folio fo in folios)
            {
                if (fo.cmid == fId)
                    return fo;
            }

            return null;
        }

        /// <summary>
        /// Gets the forum.
        /// </summary>
        /// <param name="fId">The forum identifier.</param>
        /// <returns>The structure that contains the information about the forum.</returns>
        public Forum getForum(int fId)
        {
            foreach (Forum fo in forums)
            {
                if (fo.cmid == fId)
                    return fo;
            }

            return null;
        }

        /// <summary>
        /// Gets the undefined module. (can be a e-fólio,forum or module). Used in updates.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Structure of the object desired.</returns>
        public object getUndefinedModule(int id)
        {

            Folio f = getFolio(id);
            if (f != null)
                return f;
            Forum fo = getForum(id);
            if (fo != null)
                return fo;

            modules m = getModuleById(id);
            if (m != null)
                return m;

            return null;
        }

        /// <summary>
        /// Gets the discussion.
        /// </summary>
        /// <param name="fId">The forum identifier.</param>
        /// <param name="discId">The discussion identifier.</param>
        /// <returns>Structure of the discussion.</returns>
        public Discussions GetDiscussion(int fId, int discId)
        {
            Forum f = getForum(fId);
            if(f != null)
            {
                
                foreach(Discussions d in f.discussions)
                {
                    if (d.id == discId)
                        return d;
                }
            }
            //Debug.Log("NO FORUM");
            return null;
        }

        // METODOS PARA RECEBER WEBSERVICE RESPONSES JSON

        /// <summary>
        /// Receives the course topics obtained by webservices.
        /// </summary>
        /// <param name="topics">The topics.</param>
        public void receiveCourseTopics(List<jsonValues.Topics> topics)
        {
            //Debug.Log("a usar: " + fullName + " receiveCourseTopics");
            Topic placeHolder = null;
            modules moduleP = null;
            contents contentP = null;
            this.topics = new List<Topic>();
            foreach (jsonValues.Topics t in topics)
            {
                if (t.visible > 0)
                {
                    placeHolder = new Topic();
                    placeHolder.id = t.id;
                    placeHolder.name = t.name;
                    placeHolder.summary = HtmlDecode(t.summary);
                    placeHolder.visible = t.visible;
                    placeHolder.userVisible = (t.uservisible.Equals("true")) ? 1 : 0;
                    placeHolder.availabilityinfo = t.availabilityinfo;
                    foreach (jsonValues.Modules m in t.modules)
                    {
                        if (m.visibleoncoursepage > 0 & m.visible>0)
                        {

                            moduleP = new modules();
                            moduleP.id = m.id;
                            moduleP.name = m.name;
                            if (m.description != null)
                                moduleP.description = HtmlDecode(m.description);
                            moduleP.visible = m.visible;
                            moduleP.url = m.url;
                            moduleP.uservisible = (m.uservisible.Equals("true")) ? 1 : 0;
                            moduleP.visibleOnCoursePage = m.visibleoncoursepage;
                            moduleP.topicId = placeHolder.id;


                            foreach (jsonValues.Contents c in m.contents)
                            {
                                contentP = new contents();
                                contentP.fileUrl = c.fileurl;
                                contentP.fileSize = c.filesize;
                                contentP.filePath = c.filepath;
                                contentP.fileName = c.filename;
                                contentP.isExternalFile = c.isexternalfile;
                                contentP.type = c.type;
                                contentP.author = c.author;
                                contentP.userId = c.userid;
                                if (c.summary != null)
                                    contentP.summary = HtmlDecode(c.summary);

                                //TODO Times to be ADDED
                                moduleP.contents.AddLast(contentP);
                            }
                            placeHolder.modules.AddLast(moduleP);

                            if (moduleP.name.Contains("AF") || moduleP.name.ToLower().Contains("atividade formativa"))
                            {

                                //Debug.Log("adicionar AF " + moduleP.name);
                                string[] numbers = Regex.Split(moduleP.name, @"\D+");
                                if (numbers.Length > 0)
                                {
                                    int i = 0;
                                    while (i < numbers.Length && string.IsNullOrEmpty(numbers[i]))
                                        i++;
                                    if (i == numbers.Length) { }

                                    else
                                    {
                                        if (actFormativas.ContainsKey(numbers[i]))
                                        {
                                            if (actFormativas[numbers[i]].GetType() == typeof(modules))
                                            {
                                                List<object> obs = new List<object>();

                                                obs.Add(actFormativas[numbers[i]]);
                                                obs.Add(moduleP);
                                                actFormativas.Remove(numbers[i]);
                                                actFormativas.Add(numbers[i], obs);
                                            }
                                            else
                                            {
                                                List<object> obs = actFormativas[numbers[i]] as List<object>;

                                                obs.Add(moduleP);
                                                actFormativas.Remove(numbers[i]);
                                                actFormativas.Add(numbers[i], obs);
                                            }

                                        }
                                        else
                                            actFormativas.Add(numbers[i], moduleP);
                                    }
                                }
                                else
                                    Debug.Log("adicionar numero " + moduleP.name);
                            }
                            
                        }
                    }
                    this.topics.Add(placeHolder);
                    
                }
            }

        }

        public void removeUnecessaryModules()
        {
            Folio fol = null;
            Forum foru = null;
            List<int> nMod = new List<int>();
            foreach (Topic t in topics)
            {
                foreach(modules m in t.modules)
                {
                    if ((fol = getFolio(m.id)) != null)
                    {
                        fol.visible = m.visible;
                        fol.uservisible = m.uservisible;
                        fol.visibleOnCoursePage = m.visibleOnCoursePage;
                        fol.topicId = t.id;
                        fol.cmid = m.id;
                        nMod.Add(m.id);
                    }

                    else if ((foru = getForum(m.id)) != null)
                    {

                        foru.topicId = t.id;
                        foru.cmid = m.id;
                        nMod.Add(m.id);
                    }
                }
            }
            foreach(int i in nMod)
            {
                removeModule(i);
            }
        }


        /// <summary>
        /// Receives the grades obtained by webservices.
        /// </summary>
        /// <param name="grades">The grades.</param>
        public void receiveGrades(jsonValues.usergrades grades)
        {
            lock (folios)
            {
                Folio template;
                //folios = new LinkedList<Folio>();
                DateTime date;
                foreach (jsonValues.gradeitems f in grades.gradeitems)
                {
                    if (f.itemmodule != null)
                    {
                        if (f.itemmodule.Equals("assign"))
                        {
                            template = getFolio(f.cmid);

                            if (template != null)
                            {

                                template.grademin = f.grademin;
                                template.grademax = f.grademax;
                                template.categoryid = f.categoryid;
                                if (f.graderaw == null)
                                    template.graderaw = -1; // ainda nao recebeu avaliacao
                                else if (f.graderaw.Equals(""))
                                    template.graderaw = -1; // ainda nao recebeu avaliacao
                                else
                                    template.graderaw = Convert.ToDouble(f.graderaw);

                                //Debug.Log(template.name + " grade: " + template.graderaw);
                                template.gradeformatted = f.gradeformatted;
                                template.weightformatted = f.weightformatted;
                                template.percentageformatted = f.percentageformatted;

                                if (f.gradedategraded != 0)
                                {
                                    date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                    date = date.AddSeconds(Convert.ToInt32(f.gradedategraded));
                                    template.gradedategraded = date;
                                }

                                if (f.gradedatesubmitted != 0)
                                {
                                    date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                    date = date.AddSeconds(Convert.ToInt32(f.gradedategraded));
                                    template.gradedatesubmitted = date;
                                }

                                if (!f.feedback.Equals(""))
                                {
                                    template.feedback = f.feedback;
                                }

                                reports.AddLast(f);
                            }

                        }
                        else
                        {
                            if (f.itemname != null)
                            {
                                if (f.itemname.Equals("E-fólios")
                                    && avaliacaoContinua)
                                {
                                    checkGradeReport = true;
                                    foreach (jsonValues.gradeitems fol in reports)
                                    {
                                        if (fol.itemmodule.Equals("assign") && fol.categoryid == f.iteminstance)
                                        {
                                            if (getFolio(fol.cmid).graderaw == -1)
                                            {
                                                checkGradeReport = false;
                                            }
                                        }
                                    }

                                }
                            }
                            reports.AddLast(f);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Receives the assignments obtained by webservices.
        /// </summary>
        /// <param name="assignments">The assignments.</param>
        public void receiveAssignments(List<jsonValues.assignments> assignments)
        {
            lock (folios)
            {
                Folio template;
                modules m;
                DateTime date;
                TimeSpan diff;
                foreach (jsonValues.assignments a in assignments)
                {
                    
                    //Debug.Log("a receber folio: " + m.name);
                    template = new Folio();
                       
                    template.id = a.id;
                    template.cmid = a.cmid;
                    template.course = a.course;
                    template.name = a.name;
                    template.nosubmissions = a.nosubmissions;
                    template.grademax = a.grade;
                        

                    date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    date = date.AddSeconds(Convert.ToInt32(a.duedate));
                    template.duedate = date;

                    date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    date = date.AddSeconds(Convert.ToInt32(a.allowsubmissionsfromdate));
                    template.allowsubmissionsfromdate = date;

                    date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    date = date.AddSeconds(Convert.ToInt32(a.cutoffdate));
                    template.cutoffdate = date;

                    diff = DateTime.UtcNow.Subtract(template.duedate);

                    if (diff.TotalSeconds < 0) // Data ainda nao passou 
                    {
                        diff = -diff;
                        template.dueInformation = "";
                        if (diff.Days == 0)
                            template.dueInformation = "A entrega do efolio " + template.name + " é daqui a " + diff.Hours + " horas\n";
                        else
                            template.dueInformation = "Faltam " + diff.Days + " dias para a entrega do efolio " + template.name;

                    }
                    else
                    {
                        template.dueInformation = "O prazo de entrega do efolio " + template.name + " já passou\n";
                    }


                       
                    folios.Add(template);
                   

                }
            }
        }

        /// <summary>
        /// Receives the assignments grade obtained by webservices.
        /// </summary>
        /// <param name="assignments">The assignments.</param>
        /// <param name="userid">The user identifier.</param>
        public void receiveAssignmentsGrade(List<jsonValues.assignments> assignments, int userid)
        {
            lock (folios)
            {
                attemptgrade template;
                Folio f;
                DateTime date;
                foreach (jsonValues.assignments a in assignments) // em todos os assignments
                {
                    f = getFolio(a.assignmentid); // encontrar folio/assignment mencionado
                    if (f != null) // caso tenha encontrado
                    {
                        foreach (jsonValues.grades g in a.grades) // todas as notas desse assignment
                        {
                            if (g.userid == userid) // so notas que pertencem ao aluno
                            {
                                template = new attemptgrade();
                                template.attemptnumber = g.attemptnumber;
                                template.id = g.id;

                                if (Double.TryParse(g.grade, out template.grade)) // se verdade entao conseguiu fazer parse
                                    template.grade = Double.Parse(g.grade); //NOTE: Nao eh necessario, ja foi parsed
                                else
                                    template.grade = -1;// se for -1 entao nota nao foi dada
                                date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                date = date.AddSeconds(Convert.ToInt32(g.timecreated));
                                template.timecreated = date;

                                date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                date = date.AddSeconds(Convert.ToInt32(g.timemodified));
                                template.timemodified = date;

                                f.isDue = false; // houve alguma entrega

                                f.attempgrade.Add(template);
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Receives the forums obtained by webservices.
        /// </summary>
        /// <param name="forums">The forums.</param>
        public void receiveForums(List<jsonValues.forums> forums)
        {
            Forum template;
            modules placeholder;
            this.forums = new List<Forum>();
            foreach(jsonValues.forums f in forums)
            {
                template = new Forum();

                template.id = f.id;
                template.cmid = f.cmid;
                
                template.intro = f.intro;
                template.name = f.name;
                
                template.type = f.type;
                template.discussions = new List<Discussions>();
                this.forums.Add(template);
                
            }
        }

        /// <summary>
        /// Receives the discussions obtained by webservices.
        /// </summary>
        /// <param name="discussions">The discussions.</param>
        /// <param name="forumcmid">The forum identifier.</param>
        public void receiveDiscussions(List<jsonValues.discussions> discussions, int forumcmid)
        {
            
            Forum f = getForum(forumcmid);
            Discussions template;
            foreach(jsonValues.discussions d in discussions)
            {
                template = new Discussions();
                template.id = d.discussion;
                template.created = d.created;
                template.message = HtmlDecode(d.message);
                template.modified = d.modified;
                template.name = d.name;

                template.subject = HtmlDecode(d.subject);
                template.timemodified = d.timemodified;
                template.userfullname = d.userfullname;
                template.userid = d.userid;
                template.usermodifiedfullname = d.usermodifiedfullname;
                template.posts = new List<Posts>();
                f.discussions.Add(template);
            }
        }

        /// <summary>
        /// Receives the posts.
        /// </summary>
        /// <param name="posts">The posts.</param>
        /// <param name="forumid">The forum identifier.</param>
        /// <param name="discID">The discussion identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        public void receivePosts(List<jsonValues.posts> posts, int forumid,int discID, int studentId)
        {
            Discussions d = GetDiscussion(forumid, discID);

            if (d != null)
            {

                Posts template;

                foreach(jsonValues.posts p in posts)
                {
                    template = new Posts();
                    template.created = p.created;
                    template.discussion = p.discussion;
                    template.id = p.id;
                    template.message = HtmlDecode( p.message);
                    template.modified = p.modified;
                    template.parent = p.parent;
                    template.subject = p.subject;
                    template.userid = p.userid;
                    

                    if (template.userid == studentId)
                        nPostsDone++;
                    d.posts.Add(template);
                }
            }
        }

        /// <summary>
        /// Receives the groups obtained by webservices.
        /// </summary>
        /// <param name="groups">The groups.</param>
        public void receiveGroups(List<jsonValues.groups> groups)
        {
            group template;
            foreach (jsonValues.groups g in groups)
            {
                template = new group();
                template.id = g.id;
                template.name = HtmlDecode(g.name);
                this.groups.AddLast(template);
                if (g.name.Equals("Avaliação Contínua"))
                {
                    avaliacaoContinua = true;
                }
            }
        }

        /// <summary>
        /// Receives the updates.
        /// </summary>
        /// <param name="instances">The instances of updates.</param>
        /// <param name="lastLogin">The last login done by the user.</param>
        public void receiveUpdates(List<jsonValues.instances> instances, int lastLogin)
        {

            newsUpdate template;
            foreach(jsonValues.instances i in instances)
            {

                template = new newsUpdate();
                lock (template) {
                    
                    template.time = 0;
                    if (i.contextlevel.Equals("module"))
                    {
                        template.cmid = i.id;
                        template.component = getUndefinedModule(i.id);
                    }

                    if (template.component != null)
                    {
                        foreach (jsonValues.updates u in i.updates)
                        {
                            if (u.name.Equals("configuration"))
                                template.time = u.timeupdated;
                            if (template.component != null && template.news != null)
                            {
                                if (u.name.Equals("usergrades"))
                                {
                                    if (template.component != null)
                                        template.news = "O e-fólio " + ((template.component) as Folio).name + " foi avaliado";
                                    else
                                        template.news = "O e-fólio foi removido";
                                }



                                if (u.name.Equals("contentfiles"))
                                {
                                    template.news = "Houve adições ao Módulo " + template.cmid;
                                }

                                if (u.name.Equals("introattachmentfiles"))
                                {
                                    template.news = "Houve adições na introdução do Módulo " + template.cmid;
                                }

                                if (u.name.Equals("discussions"))
                                {

                                    template.news = "O Forum " + ((template.component) as Forum).name + " teve alguma adição";
                                }
                            }
                        }
                    }

                    if (template.component == null)
                        template.news = "Foi removido";
                    else if (template.component != null && template.news == null)
                        template.news = "Houve adições ao Módulo " + template.cmid;
                    if (template.time == 0)
                        template.time = lastLogin;

                    news.Add(template);

                    if (template.component != null)
                    {
                        if (template.component.GetType().ToString().ToLower().Contains("forum"))
                        {
                            (template.component as Forum).seen = -1;
                            forumNews.Add(template);

                        }

                        if (template.component.GetType().ToString().ToLower().Contains("folio"))
                        {
                            (template.component as Folio).seen = -1;
                            folioNews.Add(template);

                        }

                        if (template.component.GetType().ToString().ToLower().Contains("module"))
                        {
                            (template.component as modules).seen = -1;
                            moduleNews.Add(template);

                        }
                    }

                }
            }
            
            updatesFinished = true;
            
        }
        
        /// <summary>
        /// Compares the updates with the interactions made by the student, used to identify different situations.
        /// </summary>
        /// <param name="mod">The module.</param>
        public void compareUpdates(jsonValues.modulesViewed mod)
        {
            lock (news)
            {
                foreach (newsUpdate n in news.ToArray())
                {
                    if (n.cmid == mod.contextinstanceid) // 
                    {
                        if (n.time < mod.timecreated) // caso tenha sido visto depois do update
                        {
                            if (moduleNews.Contains(n))
                            {
                                lock (moduleNews[moduleNews.IndexOf(n)])
                                {
                                    //Debug.Log("APAGAR O TEMPLATE: " + n.cmid);
                                    moduleNews.Remove(n);
                                }
                                (n.component as modules).seen = 1;
                            }
                            if (folioNews.Contains(n))
                            {
                                lock (folioNews[folioNews.IndexOf(n)])
                                {
                                    //Debug.Log("APAGAR O TEMPLATE: " + n.cmid);
                                    folioNews.Remove(n);
                                }
                                (n.component as Folio).seen = 1;
                            }
                            if (forumNews.Contains(n))
                            {
                                lock (forumNews[forumNews.IndexOf(n)])
                                {
                                    //Debug.Log("APAGAR O TEMPLATE: " + n.cmid);
                                    forumNews.Remove(n);
                                }
                                (n.component as Forum).seen = 1;
                            }
                            news.Remove(n);
                        }
                    }
                }
            }
            if (mod.component.Equals("mod_forum")) // FORUM
            {
                Forum temp = getForum(mod.contextinstanceid);
                if (temp != null)
                {
                    temp.seen = 1;

                }
            }
            else if (mod.component.Equals("mod_assign")) // Folio
            {
                Folio temp = getFolio(mod.contextinstanceid);
                if (temp != null)
                {
                    temp.seen = 1;

                }
            }
            else
            {
                modules temp = getModuleById(mod.contextinstanceid);
                if (temp != null)
                {
                    temp.seen = 1;

                }
            }

            if (mod.component.Equals("gradereport_user") && logins[logins.Count - 1] < mod.timecreated) // if the student views since the last login
            {
                reportViewed = true;
            }

        }

        public void UpdateTopicsForNews()
        {   // -1 para novo e não visto, 0 para não visto, 1 para visto
            foreach (Forum foe in forums)
            {
                if(GetTopic(foe.topicId) != null)
                if (foe.seen < GetTopic(foe.topicId).seen)
                    GetTopic(foe.topicId).seen = foe.seen;
            }

            foreach(Folio fol in folios)
            {
                if (GetTopic(fol.topicId) != null)
                    if (fol.seen < GetTopic(fol.topicId).seen)
                    GetTopic(fol.topicId).seen = fol.seen;
            }
            foreach (Topic t in topics)
            {

                foreach(modules m in t.modules)
                {
                    if (m.seen < t.seen)
                        t.seen = m.seen;
                }
                everyNewsDone = true;
            }
        }


        /// <summary>
        /// Gets the average of login intervals.
        /// </summary>
        /// <returns>The average of login intervals.</returns>
        public Decimal getAverageLoginSpace()
        {
            if (logins.Count == 0)
                return 0;
            
            Decimal average=0;
            //Debug.Log(parameters.startCourseDate);
            if (parameters.startCourseDate != 0) // tem de estar definido o inicio
            {
                
                int avgStart = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddSeconds(parameters.startCourseDate)).TotalDays)+1;
                if (avgStart > 0) // Não é o inicio
                {
                    ////Debug.Log(logins.Count);
                    ////Debug.Log(avgStart);
                    //Debug.Log(logins.Count + "/" + avgStart);
                    average = Decimal.Divide(logins.Count,avgStart); // entre 0 e 1
                    //Debug.Log(Decimal.Divide(logins.Count, avgStart)); 
                }
            }
            averageLoginSpace = average;

            return average;
        }

        /// <summary>
        /// Gets the foruns that have news.
        /// </summary>
        /// <returns>List of forums.</returns>
        public List<newsUpdate> GetUpdatedForuns()
        {
            return forumNews;
        }

        /// <summary>
        /// Gets the folios that have news.
        /// </summary>
        /// <returns>List of e-fólios.</returns>
        public List<newsUpdate> GetUpdatedFolios()
        {
            return folioNews;
        }

        /// <summary>
        /// Gets the modules that have news.
        /// </summary>
        /// <returns>List of modules</returns>
        public List<newsUpdate> GetUpdatedModules()
        {
            return moduleNews;
        }

        /// <summary>
        /// Decodes the HTML strings.
        /// </summary>
        /// <param name="value">The string that needs to be decoded.</param>
        /// <returns>String decoded.</returns>
        public static string HtmlDecode(string value)
        {
            String v;
            String[] s = value.Split('\n');
            StringBuilder sb = new StringBuilder();
            foreach (String st in s)
            {
                v = st.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'").Replace("<br />", "").Replace("<hr />", "").Replace("<h3>", "").Replace("</h3>", "").Replace("<li>", "").Replace("</li>", "")
                    .Replace("<ol>", "").Replace("</ol>", "").Replace("<span>", "").Replace("<b>", "").Replace("</b>", "").Replace("</p>", "").Replace("</span>", "").Replace("<u>", "").Replace("</u>", "").Replace("<ul>", "").Replace("</ul>", "").Replace("</div>", "")
                    .Replace("</a>", "").Replace("<strong>", "").Replace("</strong>", "").Replace("<hr>", "").Replace("<br>", "").Replace("</font>", "").Replace("\u00ed", "í").Replace("\u00fa", "ú");

                v = Regex.Replace(v, @"\<span(.*?)\>", "");
                v = Regex.Replace(v, @"\<p(.*?)\>", "");
                v = Regex.Replace(v, @"\<a(.*?)\>", "");
                v = Regex.Replace(v, @"\<div(.*?)\>", "");
                v = Regex.Replace(v, @"\<img(.*?)\>", "");
                v = Regex.Replace(v, @"\<font(.*?)\>", "");

                sb.Append(v);
            }
            

            return sb.ToString();

        }
        public string FolioFeedback(int folid)
        {
            StringBuilder sb = new StringBuilder("");
            Folio f = getFolio(folid);

            if (f != null)
            {
                if (f.graderaw <= f.grademin)
                {
                    sb.Append("LOW");
                }
                else if (f.grademax > f.graderaw && f.graderaw > f.grademin)
                {
                    sb.Append("MIDDLE");
                }
                else
                {
                    sb.Append("HIGH");
                }
            }
            return sb.ToString();
        }
    }
    
}
