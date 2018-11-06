using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using System.Diagnostics;
using System.IO;
using VDS.RDF.Query;
using UnityEngine.EventSystems;
using System.Linq;

public class QeAFunctions : MonoBehaviour {

    [SerializeField] Button BackB;
    [SerializeField] Button Ask;
    [SerializeField] InputField Question;

    string msgerro = "Não consigo responder à pergunta, por favor verifique se esta está bem escrita. \nA pergunta será guardada na base de dados para análise futura.";
    string msg;
    string data = 0.ToString();
    string query = "PREFIX tv: <http://www.semanticweb.org/albertosalgueiro/ontologies/2018/1/TV_fev_PUC#>" + " PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> ";
    string query2 = "PREFIX tv: <http://www.semanticweb.org/albertosalgueiro/ontologies/2018/1/TV_fev_PUC#>" + " PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> ";
    bool qualdia = false;
    bool proximoe = false;
    string mensagemfinal = "";

    DateTime sec1;
    DateTime sec2;
    DateTime sec3;

    string secs1;
    string secs2;

    private Word w;
    private Word x;
    private Word y;
    private Word z;

    public static QeAFunctions Instance;

    public string[] respostas;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        BackB.onClick.AddListener(BackFunction);
        Ask.onClick.AddListener(AskFunction);
    }

    string GetDataValue (string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if(value.Contains("|"))value = value.Remove(value.IndexOf("|"));
        return value;
    }




    /// <summary>
    /// Backs to initial menu function.
    /// </summary>
    public void BackFunction()
    {
        TutorScreen.Instance.InitialMenu();
    }

    /// <summary>
    /// Sends ask.
    /// </summary>
    public void AskFunction()
    {
        TutorScreen.Instance.option = 9;
        TutorScreen.Instance.replying();
        mensagemfinal = "";
    }


    //usar
    //dm.getCourseById(wm.courseId).fullName
    //nome do curso







    /// <summary>
    /// Saves ask and shows it.
    /// </summary>
    /// <returns></returns>
    public string GetResult()
    {
        WebManager.Instance.insertQuestion(Question.text);
        //print(Question.text);

        List<Word> phrase = new List<Word>();
        string p = Question.text;
        StringBuilder test = new StringBuilder(p);

        //confirma se acaba com "?", se não acabar adiciona
        if (!p.EndsWith("?"))
        {
            test.Append("?");
        }
        p = test.ToString();


        if (p.Contains("Fóruns"))
        {
            p = p.Replace("Fóruns", "Fórum");
        }

        if (p.Contains("p-fólios"))
        {
            p = p.Replace("p-fólios", "p-fólio");
        }

        if (p.Contains("e-fólios"))
        {
            p = p.Replace("e-fólios", "e-fólio");
        }

        string t = TestPython(p, "");
        string final;

        //confirma se o retorno do python não é uma frase vazia, se for apresenta uma mensagem de erro
        //caso seja uma frase formata a frase e separa as palavras e o deprels de forma a serem apresentados
        if (t.Contains("[]"))
        {

            final = msgerro;
        }
        else
        {
            List<List<string>> words = new List<List<string>>();
            List<string> temp = new List<string>();
            bool insert = false;
            string[] separatingChars = { "[{", "}]", "{", "}, ", ", " };
            string[] ts = t.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            //StringBuilder sb = new StringBuilder("");
            foreach (string s in ts)
            {
                if (s != "")
                {
                    insert = s.Contains("form");
                    if (insert)
                    {
                        words.Add(temp);
                        temp = new List<string>();
                    }
                    temp.Add(s);
                }
            }

            //string[] separatingChars1 = { " ", "?" };
            //string[] val = p.Split(separatingChars1, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (List<string> k in words)
            {
                phrase.Add(new Word(k));
            }

            StringBuilder form = new StringBuilder("Words:\n");
            StringBuilder dep = new StringBuilder("Deprels:\n");

            foreach (Word j in phrase)
            {
                if (j.form != null)
                {
                    //Descomentar para aparecer descriçção das palavras na consola
                    //UnityEngine.Debug.Log(j.toString());
                    form.Append(j.form + "\n");
                    dep.Append(j.deperl + "\n");
                }
            }

            //print(input.text + ";" + phrase[0]);

            //chama a função TypeOfQuestion com a frase completa e a frase formatada
            //Recebe a query criada por esta função
            final = TypeOfQuestion(Question.text, phrase);
        }

        bool quantos = false;
        bool temporal = false;
        int temporal2 = 0;
        if (Question.text.ToLower().Contains("quantos") || Question.text.ToLower().Contains("quanto"))
        {
            quantos = true;
        }
        else if (Question.text.ToLower().Contains("hoje") || Question.text.ToLower().Contains("amanhã") || Question.text.ToLower().Contains("ontem"))
        {
            temporal = true;

            if (Question.text.ToLower().Contains("hoje"))
            {
                temporal2 = 1;
            }
            else if (Question.text.ToLower().Contains("amanhã"))
            {
                temporal2 = 2;
            }
            else if (Question.text.ToLower().Contains("ontem"))
            {
                temporal2 = 3;
            }
        }

        //Caso o valor obtido de TypeOfQUestion seja a mensagem de erro apresenta esta
        //Caso não seja irá chamar a função ReadingFromARDF que irá executar a query, formatar e apresentar o resultado final

        if (final != msgerro)
        {
            ReadingFromARDF(final, quantos, temporal, temporal2);
        }
        else
        {
            mensagemfinal = msgerro;
        }

        return mensagemfinal;
    }

    public static string TestPython(string ph, string path)
    {
        string something = "\"" + Application.dataPath + "/teste_nlx.py\"";

        char[] splitter = { '\r' };
        string phrase = '"' + ph + '"';

        string py = "\"C:\\Users\\Virtual Tutoring PC1\\AppData\\Local\\Programs\\Python\\Python36\\python.exe\"";
        string server = "python " + Application.dataPath + "/teste_nlx.py " + phrase;

        Process proc;

        print("PLataforma: " + Application.platform);

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = py,
                    //FileName = server,
                    Arguments = string.Concat(something, " ", phrase),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.GetEncoding("iso-8859-1")
                }
            };
        }
        else
        {
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    //FileName = py,
                    FileName = server,
                    //Arguments = string.Concat(something, " ", phrase),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.GetEncoding("iso-8859-1")
                }
            };
        }

        // Localização do bash?? /bin/bash -- provavelmente não e preciso
        // Correr programa no servidor  "python " + Application.dataPath + "/teste_nlx.py " + phrase  

        proc.Start();
        StreamReader sReader = proc.StandardOutput;

        string teste = sReader.ReadToEnd();
        string[] output = teste.Split(splitter);

        StringBuilder sb = new StringBuilder("");

        foreach (string s in output)
        {
            sb.Append(s);
        }

        proc.WaitForExit();
        Console.ReadLine();

        return sb.ToString();
    }

    //Função para montar a query com a informação que lhe é transmitida
    public void MontarQuery(string pos1, string pos2)
    {
        query += "SELECT ?x ?y ?z" + " WHERE { ?x a tv:" + pos1 + ". ?y rdfs:domain tv:" + pos1 + ". " + "?x ?y tv:" + pos2 + ". OPTIONAL {?x rdfs:isDefinedBy ?z} }";
    }

    //Função para agrupar palavras
    public string adicionar(List<Word> words, string teste, string k)
    {
        UnityEngine.Debug.Log("ENTROU");
        Word first = getFromDeprel(words, teste);
        words.Remove(first);
        k += "_" + first.form;
        Word second = getFromDeprel(words, teste);
        if (second != null)
        {
            k += "_" + second.form;
        }
        return k;
    }

    //Dependendo do tipo de pergunta irá decidir que informação deve ser colocada na query
    public string TypeOfQuestion(string question, List<Word> words)
    {

        if (question.ToLower().Contains("qual"))
        {
            if (question.ToLower().Contains("data") || question.ToLower().Contains("dia"))
            {
                qualdia = true;
            }
            if (question.ToLower().Contains("próximo") && question.ToLower().Contains("e-fólio"))
            {
                proximoe = true;
            }


            if (getFromDeprel(words, "C") != null)
            {
                x = getFromDeprel(words, "PRD-ARG2");
                y = getFromDeprel(words, "C");
            }
            else if (getFromDeprel(words, "M-PRED") != null)
            {
                x = getFromDeprel(words, "M-PRED");
                y = getFromDeprel(words, "PRD-ARG2");
            }
            else
            {
                mensagemfinal = msgerro;
            }

            if (getFromDeprel(words, "C") != null || getFromDeprel(words, "M-PRED") != null)
            {
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, x.form);
            }
        }
        else if (question.ToLower().Contains("quais"))
        {
            if (getFromDeprel(words, "PRD-ARG2") != null)
            {
                Word x = getFromDeprel(words, "PRD-ARG2");
                Word y = getFromDeprel(words, "C");
                string k = x.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, y.form);
            }
            else
            {
                mensagemfinal = msgerro;
            }

        }
        else if (question.ToLower().StartsWith("quantos") || question.ToLower().StartsWith("quantas"))
        {
            foreach (Word a in words)
            {
                if (a.form == "Quantos" || a.form == "Quantas")
                {
                    a.deperl = "TESTE";
                }
            }

            if (getFromDeprel(words, "C-ARG2") != null)
            {

                x = getFromDeprel(words, "C-ARG2");
                y = getFromDeprel(words, "C");
            }
            else if (getFromDeprel(words, "SJ-ARG1") != null && getFromDeprel(words, "DO-ARG2") != null)
            {
                x = getFromDeprel(words, "SJ-ARG1");
                y = getFromDeprel(words, "DO-ARG2");
            }
            else if (getFromDeprel(words, "SJ-ARG1") != null)
            {
                x = getFromDeprel(words, "SJ-ARG1");
                y = getFromDeprel(words, "C");
            }
            else if (getFromDeprel(words, "PRD-ARG2") != null)
            {
                x = getFromDeprel(words, "PRD-ARG2");
                y = getFromDeprel(words, "C");
            }
            else
            {
                mensagemfinal = msgerro;
            }

            if (getFromDeprel(words, "C-ARG2") != null || (getFromDeprel(words, "SJ-ARG1") != null && getFromDeprel(words, "DO-ARG2") != null) || getFromDeprel(words, "SJ-ARG1") != null || getFromDeprel(words, "PRD-ARG2") != null)
            {
                string k = x.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, y.form);
            }
        }
        else if (question.ToLower().Contains("quanto"))
        {
            if (getFromDeprel(words, "SJ-ARG1") != null)
            {
                x = getFromDeprel(words, "SJ-ARG1");
                y = getFromDeprel(words, "C");

                string k = x.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, y.form);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().StartsWith("o que "))
        {
            if (getFromDeprel(words, "PRD-ARG2") != null)
            {
                x = getFromDeprel(words, "PRD-ARG2");
                string k = x.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                else if (k.Equals("UC"))
                {
                    k = "Unidade_Curricular";
                }
                MontarQuery(k, "definição");
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().StartsWith("quando"))
        {
            if (getFromDeprel(words, "M-PRED") != null)
            {
                x = getFromDeprel(words, "M-PRED");
                y = getFromDeprel(words, "PRD-ARG2");
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, x.form);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().StartsWith("costumam"))
        {
            if (getFromDeprel(words, "PRD-ARG2") != null && getFromDeprel(words, "DO-ARG2") != null && getFromDeprel(words, "C") != null)
            {
                x = getFromDeprel(words, "PRD-ARG2");
                y = getFromDeprel(words, "DO-ARG2");
                z = getFromDeprel(words, "C");
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }

                if (x.form.Equals("UC"))
                {
                    x.form = "Unidade_Curricular";
                }
                if (y.form.Equals("UC"))
                {
                    y.form = "Unidade_Curricular";
                }
                if (z.form.Equals("UC"))
                {
                    z.form = "Unidade_Curricular";
                }


                query2 += "SELECT ?x ?y ?z" + " WHERE { ?x a tv:" + x.form + ". ?y rdfs:domain tv:" + x.form + ". " + "?x ?y tv:" + k + ". OPTIONAL {?x rdfs:isDefinedBy ?z} }";
                IGraph g = new Graph();
                string mOntologyFilename = @"Assets\TutorVirtual_fev_2018_PUC_RDF_corrigido.owl";
                g.LoadFromFile(mOntologyFilename);
                object results2 = g.ExecuteQuery(query2);
                SparqlResultSet rset = (SparqlResultSet)results2;
                string second = "";

                foreach (SparqlResult r in rset)
                {
                    string[] separatingChars1 = { "?z =", "?y =" };
                    string[] val = r.ToString().Split(separatingChars1, System.StringSplitOptions.RemoveEmptyEntries);
                    if (val.Length > 2)
                    {
                        second = val[2];
                    }
                }

                //print("second" + second);
                string second2 = second.Replace(" ", String.Empty);

                MontarQuery(z.form, second2);

            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().Contains("em que"))
        {

            x = getFromDeprel(words, "PRD-ARG2");

            if (getFromDeprel(words, "SJ-ARG1") != null)
            {
                y = getFromDeprel(words, "SJ-ARG1");
            }
            else if (getFromDeprel(words, "C") != null)
            {
                y = getFromDeprel(words, "C");
            }
            else
            {
                mensagemfinal = msgerro;
            }

            if (getFromDeprel(words, "SJ-ARG1") != null || getFromDeprel(words, "C") != null)
            {
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(x.form, k);
            }
        }
        else if (question.ToLower().Contains("posso"))
        {
            if (getFromDeprel(words, "C-ARG1") != null)
            {
                x = getFromDeprel(words, "C-ARG1");
                y = getFromDeprel(words, "C");
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, x.form);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().Contains("posso"))
        {
            if (getFromDeprel(words, "C-ARG1") != null)
            {
                x = getFromDeprel(words, "C-ARG1");
                y = getFromDeprel(words, "C");
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, x.form);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().Contains("podemos"))
        {
            if (getFromDeprel(words, "DO-ARG2") != null)
            {
                x = getFromDeprel(words, "DO-ARG2");
                y = getFromDeprel(words, "C");
                string k = y.form;
                if (getFromDeprel(words, "M-PRED") != null)
                {
                    k = adicionar(words, "M-PRED", k);
                }
                MontarQuery(k, x.form);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().StartsWith("sai"))
        {
            if (getFromDeprel(words, "DO-ARG2") != null)
            {
                x = getFromDeprel(words, "DO-ARG2");
                y = getFromDeprel(words, "C");
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(x.form, k);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().StartsWith("tenho"))
        {
            if (getFromDeprel(words, "DO-ARG2") != null)
            {
                x = getFromDeprel(words, "DO-ARG2");
                y = getFromDeprel(words, "C");
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, x.form);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().StartsWith("preciso"))
        {
            if (getFromDeprel(words, "DO-ARG2") != null)
            {
                x = getFromDeprel(words, "DO-ARG2");
                y = getFromDeprel(words, "M-PRED");
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(x.form, k);
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        else if (question.ToLower().StartsWith("esta"))
        {
            x = getFromDeprel(words, "SJ-ARG1");

            if (getFromDeprel(words, "PRD") != null)
            {
                y = getFromDeprel(words, "PRD");
            }
            else if (getFromDeprel(words, "PRD-ARG2") != null)
            {
                y = getFromDeprel(words, "PRD-ARG2");
            }
            else
            {
                mensagemfinal = msgerro;
            }

            if (getFromDeprel(words, "PRD") != null || getFromDeprel(words, "PRD-ARG2") != null)
            {
                string k = y.form;
                if (getFromDeprel(words, "N") != null)
                {
                    k = adicionar(words, "N", k);
                }
                MontarQuery(k, x.form);
            }
        }
        else if (question.ToLower().Contains("existe") || question.ToLower().Contains("temos") || question.ToLower().Contains("há"))
        {
            if (getFromDeprel(words, "M-TMP") != null)
            {
                w = getFromDeprel(words, "M-TMP");
                if (w.form.ToLower().Equals("hoje"))
                {
                    data = System.DateTime.Now.ToString("dd_MMM_yyyy");
                }
                else if (w.form.ToLower().Equals("ontem"))
                {
                    data = System.DateTime.Now.AddDays(-1).ToString("dd_MMM_yyyy");
                }
                else if (w.form.ToLower().Equals("amanhã"))
                {
                    data = System.DateTime.Now.AddDays(1).ToString("dd_MMM_yyyy");
                }
                else
                {
                    mensagemfinal = msgerro;
                }
            }
            else if (getFromDeprel(words, "M-PRED") != null)
            {
                w = getFromDeprel(words, "M-PRED");
            }
            else
            {
                mensagemfinal = msgerro;
            }

            if (getFromDeprel(words, "DO-ARG2") != null)
            {
                Word x = getFromDeprel(words, "DO-ARG2");

                if (question.ToLower().Contains("hoje") || question.ToLower().Contains("amanhã") || question.ToLower().Contains("ontem"))
                {
                    MontarQuery(x.form, "data");
                }
                else
                {
                    MontarQuery(x.form, w.form);
                }
            }
            else if (getFromDeprel(words, "DO-ARG1") != null)
            {
                Word x = getFromDeprel(words, "DO-ARG1");

                if (question.ToLower().Contains("hoje") || question.ToLower().Contains("amanhã") || question.ToLower().Contains("ontem"))
                {
                    MontarQuery(x.form, "data");
                }
                else
                {
                    MontarQuery(x.form, w.form);
                }
            }
            else if (getFromDeprel(words, "SJ-ARG1") != null)
            {
                Word x = getFromDeprel(words, "SJ-ARG1");

                if (question.ToLower().Contains("hoje") || question.ToLower().Contains("amanhã") || question.ToLower().Contains("ontem"))
                {
                    MontarQuery(x.form, "data");
                }
                else
                {
                    MontarQuery(x.form, w.form);
                }
            }
            else
            {
                mensagemfinal = msgerro;
            }
        }
        //Adcionar Como, com devo e posso restrições para o C
        else
        {
            mensagemfinal = msgerro;
        }

        return query;
    }

    public static string convertTo(string q)
    {
        byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(q);

        // Convert utf-8 bytes to a string.
        string s_unicode2 = System.Text.Encoding.ASCII.GetString(utf8Bytes);

        return s_unicode2;
    }

    //Confirma se existem palavras com um certo deprel
    public Word getFromDeprel(List<Word> l, string key)
    {

        //UnityEngine.Debug.Log("\n" + key);
        foreach (Word w in l)
        {
            //UnityEngine.Debug.Log(w.deperl + "==" + key);
            if (w.deperl != null && w.deperl.Equals(key))
            {
                return w;

            }
        }
        return null;
    }

    private void ReadingFromARDF(string mSprqlQuery, bool quantos, bool temporal, int temporal2)
    {
        string mOntologyFilename = @"Assets\TutorVirtual_fev_2018_PUC_RDF_corrigido.owl";

        IGraph g = new Graph();
        //var dates = new List<string>();
        var dates = new List<DateTime>();

        g.LoadFromFile(mOntologyFilename);
        try
        {
            print(mSprqlQuery);
            //executa query 
            object results = g.ExecuteQuery(mSprqlQuery);
            //formatação dos resultados obitdos da query
            if (results is SparqlResultSet)
            {
                SparqlResultSet rset = (SparqlResultSet)results;
                msg = "";
                int resultados = rset.Count;
                //formatação do resultado final apresentado
                if (resultados == 0)
                {
                    if (temporal || quantos)
                    {
                        msg = "";
                    }
                    else
                    {
                        mensagemfinal = msgerro;
                    }
                }
                else
                {
                    if (temporal || quantos || qualdia)
                    {
                        msg = "";
                    }
                    else
                    {
                        msg = "Obteve-se " + resultados + " resultados:";
                    }

                }

                mensagemfinal += msg;
                UnityEngine.Debug.Log(msg);

                int cnt = 0;
                //caso se uma pergunta do tipo quantos irá devolver um valor numérico
                if (quantos)
                {
                    foreach (SparqlResult r in rset)
                    {
                        cnt++;
                    }

                    string teste = GetDataValue(mSprqlQuery, "domain tv:").Split(new char[] { '.' })[0].Replace("_", " ");

                    if (cnt == 0)
                    {
                        mensagemfinal += "Não existem " + teste + ".";
                    }
                    else if (cnt == 1)
                    {
                        mensagemfinal += "Existe " + cnt + " " + teste + ".";
                    }
                    else
                    {
                        mensagemfinal += "Existem " + cnt + " " + teste + ".";
                    }
                }
                else if (proximoe)
                {
                    foreach (SparqlResult r in rset)
                    {
                        cnt++;
                        string[] separatingChars1 = { "?z =", "?y =" };
                        string[] val = r.ToString().Split(separatingChars1, System.StringSplitOptions.RemoveEmptyEntries);
                        string second = "";
                        if (val.Length > 2)
                        {
                            second = val[2];
                        }

                        string diabd = GetDataValue(second, "dia ").Split(new char[] { '.' })[0].Replace("_", " ");
                        string diadb2 = diabd.Replace(" de ", " ").Replace("Janeiro", "Jan").Replace("Fevereiro", "Feb").Replace("Março", "Mar").Replace("Abril", "Apr").Replace("Maio", "May").Replace("Junho", "Jun").Replace("Julho", "Jul").Replace("Setembro", "Sep").Replace("Outubro", "Oct").Replace("Novembro", "Nov").Replace("Dezembro", "Dec");

                        sec2 = DateTime.Parse(diadb2);
                        dates.Add(sec2);
                    }

                    dates.Sort();
                    DateTime today = DateTime.Today;
                    var result = dates.BinarySearch(today);
                    DateTime nearest;
                    DateTime amanha = System.DateTime.Today.AddDays(1);

                    string testedb = GetDataValue(mSprqlQuery, "domain tv:").Split(new char[] { '.' })[0].Replace("_", " ");


                    if (today > dates.Last())
                    {
                        mensagemfinal = "Não existe mais " + testedb + " marcados!";
                    }
                    else
                    {
                        if (result >= 0)
                        {
                            nearest = dates[result];
                        }
                        else if (~result == dates.Count)
                        {
                            nearest = dates.Last();
                        }
                        else
                        {
                            nearest = dates[~result];
                        }


                        string[] proximo = nearest.ToString().Split(' ');
                        string proximo2 = proximo[0];
                        string proximo3 = proximo2.Replace("/11/", " de Novembro de ").Replace("/1/", " de Janeiro de ").Replace("/2/", " de Fevereiro de ").Replace("/3/", " de Março de ").Replace("/4/", " de Abril de ").Replace("/5/", " de Maio de ").Replace("/6/", " de Junho de ").Replace("/7/", " de Julho de ").Replace("/8/", " de Agosto de ").Replace("/9/", " de Setembro de ").Replace("/10/", " de Outubro de ").Replace("/12/", " de Dezembro de ");

                        if (nearest == today)
                        {
                            mensagemfinal = "O " + testedb + " é hoje!";
                        }
                        else if (nearest == amanha)
                        {
                            mensagemfinal = "O próximo " + testedb + " é amanhã!";
                        }
                        else
                        {
                            mensagemfinal = "O próximo " + testedb + " é no dia " + proximo3 + "!";
                        }
                    }
                }
                else
                {
                    foreach (SparqlResult r in rset)
                    {
                        cnt++;
                        //UnityEngine.Debug.Log(r.ToString());
                        string[] separatingChars1 = { "?z =", "?y =" };
                        string[] val = r.ToString().Split(separatingChars1, System.StringSplitOptions.RemoveEmptyEntries);
                        string second = "";
                        if (val.Length > 2)
                        {
                            second = val[2];
                        }
                        //msg = "#" + cnt + ": " + val[0].Replace("albertosalgueiro","myTVOntology") + second;


                        /*
                        string teste = val[0];
                        string limpa = teste.Replace("?x = http://www.semanticweb.org/albertosalgueiro/ontologies/2018/1/TV_fev_PUC#", "").
                            Replace("%C3%A0", "à").Replace("%C3%A1", "á").Replace("%C3%A2", "â").Replace("%C3%A3", "ã").
                            Replace("%C3%A7", "ç").
                            Replace("%C3%A8", "è").Replace("%C3%A9", "é").Replace("%C3%AA", "ê").
                            Replace("%C3%AC", "ì").Replace("%C3%AD", "í").Replace("%C3%AE", "î").
                            Replace("%C3%B2", "ò").Replace("%C3%B3", "ó").Replace("%C3%B4", "ô").Replace("%C3%B5", "õ").
                            Replace("%C3%B9", "ù").Replace("%C3%BA", "ú").Replace("%C3%BB", "û");
                            */

                        //Caso exista uma frase com data na Ontologia é formatada para o tipo Datetime de forma a poder ser comparada
                        string diabd = GetDataValue(second, "dia ").Split(new char[] { '.' })[0].Replace("_", " ");
                        string diadb2 = diabd.Replace(" de ", " ").Replace("Janeiro", "Jan").Replace("Fevereiro", "Feb").Replace("Março", "Mar").Replace("Abril", "Apr").Replace("Maio", "May").Replace("Junho", "Jun").Replace("Julho", "Jul").Replace("Setembro", "Sep").Replace("Outubro", "Oct").Replace("Novembro", "Nov").Replace("Dezembro", "Dec");
                        string testresposta;
                        string testedb = GetDataValue(mSprqlQuery, "domain tv:").Split(new char[] { '.' })[0].Replace("_", " ");
                        string data2 = data.Replace("_", " ");

                        if (temporal || qualdia)
                        {
                            sec1 = DateTime.Now;
                            sec2 = DateTime.Parse(diadb2);
                            sec3 = System.DateTime.Now.AddDays(1);

                            secs1 = sec1.ToString("dd_MMM_yyyy");
                            secs2 = sec2.ToString("dd_MMM_yyyy");
                        }

                        //Formatação da frase de saida caso seja uma pergunta temporal
                        if (qualdia)
                        {

                            if (secs1.Equals(secs2))
                            {
                                testresposta = "O " + testedb + " será hoje, dia " + diabd + ".";
                                mensagemfinal += testresposta;
                            }
                            else if (sec1.CompareTo(sec2) < 0)
                            {
                                testresposta = "O próximo " + testedb + " será no dia " + diabd + ".";
                                mensagemfinal += testresposta;
                            }
                            else
                            {
                                testresposta = "Não se encontram mais " + testedb + " marcados.";
                                mensagemfinal += testresposta;
                            }

                        }
                        else if (temporal)
                        {
                            if (temporal2 == 1)
                            {
                                if (diadb2.Equals(data2))
                                {
                                    testresposta = "Sim, tem " + testedb + " hoje, dia " + diabd + ".";
                                    mensagemfinal += testresposta;
                                }
                                else
                                {
                                    if (sec1.CompareTo(sec2) < 0)
                                    {
                                        testresposta = "Não, o " + testedb + " será no dia " + diabd + ".";
                                        mensagemfinal += testresposta;
                                    }
                                    else
                                    {
                                        testresposta = "Não, o " + testedb + " foi no dia " + diabd + ".";
                                        mensagemfinal += testresposta;
                                    }
                                }
                            }
                            else if (temporal2 == 2)
                            {
                                if (diadb2.Equals(data2))
                                {
                                    testresposta = "Sim, tem " + testedb + " amanhã, dia " + diabd + ".";
                                    mensagemfinal += testresposta;
                                }
                                else
                                {
                                    if (sec3.CompareTo(sec2) < 0)
                                    {
                                        testresposta = "Não, o " + testedb + " será no dia " + diabd + ".";
                                        mensagemfinal += testresposta;
                                    }
                                    else if (secs1.Equals(secs2))
                                    {
                                        testresposta = "Não, o " + testedb + " será hoje, dia " + diabd + ".";
                                        mensagemfinal += testresposta;
                                    }
                                    else
                                    {
                                        testresposta = "Não, o " + testedb + " foi no dia " + diabd + ".";
                                        mensagemfinal += testresposta;
                                    }
                                }
                            }
                            else if (temporal2 == 3)
                            {
                                if (diadb2.Equals(data2))
                                {
                                    testresposta = "Sim, o " + testedb + " foi ontem, dia " + diabd + ".";
                                    mensagemfinal += testresposta;
                                }
                                else
                                {
                                    testresposta = "Não, o " + testedb + " será no dia " + diabd + ".";
                                    mensagemfinal += testresposta;
                                }
                            }
                            else
                            {
                                mensagemfinal += second;
                            }
                        }
                        else
                        {
                            //msg = "#" + cnt + ": " + val[0].Replace("albertosalgueiro", "myTVOntology") + second;
                            //msg = "#" + cnt + ": " + limpa + second;
                            msg = "#" + cnt + ": " + second;
                            mensagemfinal = msg;
                        }
                    }
                }
            }
            /*else if (results is IGraph)
            {
                //CONSTRUCT/DESCRIBE queries give a IGraph
                IGraph resGraph = (IGraph)results;

                int cnt = 0;
                foreach (Triple t in resGraph.Triples)
                {
                    string msg = "Triple #" + ++cnt + ": " + t.ToString();
                    UnityEngine.Debug.Log(msg);
                    //Do whatever you want with each Triple
                }
            }*/
            else
            {
                //If you don't get a SparqlResutlSet or IGraph something went wrong 
                //but didn't throw an exception so you should handle it here 
                string msg = "ERROR, or no results";
                UnityEngine.Debug.Log(msg);
                mensagemfinal = msgerro;
            }

            query = "PREFIX tv: <http://www.semanticweb.org/albertosalgueiro/ontologies/2018/1/TV_fev_PUC#>" + " PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> ";

        }
        catch (RdfQueryException queryEx)
        {
            //There was an error executing the query so handle it here
            UnityEngine.Debug.Log(queryEx.Message);
        }
    }

    public class Word
    {
        public string form;
        public string pos;
        public string infl;
        public string ne;
        public string deperl;
        public string parent;
        public string udeprel;
        public string uparent;
        public string lemma;
        public string space;

        public Word(List<string> components)
        {
            foreach (string k in components)
            {
                string[] separatingChars = { ": " };
                string[] val = k.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
                fillTheField(val[0], val[1]);
            }
        }

        private void fillTheField(string v1, string v2)
        {
            v2 = v2.Replace(@"'", "");
            switch (v1)
            {
                case "'form'":
                    this.form = v2;
                    break;
                case "'pos'":
                    this.pos = v2;
                    break;
                case "'infl'":
                    this.infl = v2;
                    break;
                case "'ne'":
                    this.ne = v2;
                    break;
                case "'deprel'":
                    this.deperl = v2;
                    break;
                case "'parent'":
                    this.parent = v2;
                    break;
                case "'udeprel'":
                    this.udeprel = v2;
                    break;
                case "'uparent'":
                    this.uparent = v2;
                    break;
                case "'lemma'":
                    this.lemma = v2;
                    break;
                case "'space'":
                    this.space = v2;
                    break;
            }
        }

        //Quais as Competências da UC1?
        //string.Join( ",", groupIds );

        public string toString()
        {
            return "form: " + form + "; pos: " + pos + "; infl: " + infl + "; ne: " + ne + "; deprel: " + deperl + "; parent: " + parent + "; udeprel: " + udeprel + "; uparent: "
                + uparent + "; lemma: " + lemma + "; space: " + this.space + ".";
        }
    }



    // Update is called once per frame
    void Update () {
		
	}
}