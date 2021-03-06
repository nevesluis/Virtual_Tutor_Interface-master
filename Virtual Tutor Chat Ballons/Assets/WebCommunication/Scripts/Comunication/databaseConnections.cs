﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UserInfo;
using System.Linq;

using System.Security.Cryptography;

public class databaseConnections :MonoBehaviour {

    [Serializable]
    public class Values
    {
        public List<jsonValues.logins> logins;
    }
    private Values v;
    private String location = "https://tutoria-virtual.uab.pt/webservices/teste/";
    private String sLocation = "https://tutoria-virtual.uab.pt/webservices/teste/moodlereplica/"; // secret location

    String secretKey = "mySecretKey";

    public UserData user;

    public Boolean finished = false, success = false;

    public Hashtable hashtable = new Hashtable();

    // Use this for initialization
    void Start () {
        getUserClass();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Gets the user class.
    /// Used when the Start method hasn't finished
    /// </summary>
    public void getUserClass()
    {
        GameObject moodleLogin = GameObject.Find("moodleLogin");
        DataManager dataM = moodleLogin.GetComponent(typeof(DataManager)) as DataManager;
        user = dataM.getUser();
    }

    /// <summary>
    /// Method called to start the Coroutine that gets the logins.
    /// </summary>
    /// <param name="course">The list of course ids desired.</param>
    public void getLogins(List<int> course)
    {
        StartCoroutine(getL(course));
    }

    /// <summary>
    /// Method that comunicates with the DB and requests the logins.
    /// </summary>
    /// <param name="course">The list of course ids desired.</param>
    public IEnumerator getL(List<int> course)
    {
        finished = false;
        LinkedList<String> parameters;
        if (user == null)
            getUserClass();
        List<jsonValues.logins> logins = new List<jsonValues.logins>();
        foreach (int c in course)
        {
            parameters = new LinkedList<string>();parameters.AddLast(c.ToString());
            parameters.AddLast(user.id.ToString());
            
            WWW www = new WWW(location + "login.php?function=getlogin&userid=" + user.id + "&courseid=" + c + "&hash=" + encryptHash(parameters));
            yield return www;
            String result = www.text;

            String final = "{\"logins\":" + result + "}";

            v = JsonUtility.FromJson<Values>(final);
            foreach (jsonValues.logins l in v.logins)
                logins.Add(l);
        }
        hashtable.Add("getLogins", logins);
        
    }

    /// <summary>
    /// Method that comunicates with the DB and inserts the login.
    /// </summary>
    /// <param name="course">The course identifier.</param>
    public IEnumerator insertLogin(int course)
    {
        finished = false;
        int seq = v.logins.Count + 1;
        TimeSpan s = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
        int seconds = Convert.ToInt32(s.TotalSeconds);
        LinkedList<String> parameters = new LinkedList<string>();
        parameters.AddLast(course.ToString());
        parameters.AddLast(seconds.ToString());
        parameters.AddLast(seq.ToString());
        parameters.AddLast(user.id.ToString());
        
        WWW www = new WWW(location + "login.php?function=putlogin&userid=" + user.id + "&courseid=" + course +"&login=" + seconds+ "&seqn="+ seq + "&hash=" + encryptHash(parameters));
        yield return www;
        String result = www.text;

        finished = true;
        if (result.Contains("Record updated successfully"))
            if(!hashtable.ContainsKey("insertLogin"))
                hashtable.Add("insertLogin", true);

        else
           if (!hashtable.ContainsKey("insertLogin"))
                hashtable.Add("insertLogin", false);
    }

    /// <summary>
    /// Encrypts the string composed of the parameters and the secretkey.
    /// </summary>
    /// <param name="parameters">The parameters to add to the string.</param>
    /// <returns>Hash desired to validate the request.</returns>
    public String encryptHash(LinkedList<String> parameters)
    {
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        StringBuilder sb = new StringBuilder();
        foreach (String s in parameters)
        {
            sb.Append(s);
        }
        sb.Append(secretKey);

        byte[] toBytes = System.Text.Encoding.ASCII.GetBytes(sb.ToString());


        byte[] hash = md5.ComputeHash(toBytes);
        sb.Remove(0, sb.Length);
        foreach (byte b in hash)
            sb.Append(b.ToString("X2"));

        return sb.ToString().ToLower();
    }

    /// <summary>
    /// Method called to start the Coroutine to perform a desired request.
    /// </summary>
    /// <param name="fileName">Name of the file containing the webservice.</param>
    /// <param name="parameters">The parameters to the webservice.</param>
    public void prepareRequest(String fileName, Hashtable parameters)
    {
        StartCoroutine(makeRequest(fileName, parameters));
    }

    /// <summary>
    /// Method that communicates with the DB to make the request.
    /// </summary>
    /// <param name="fileName">Name of the file containing the webservice.</param>
    /// <param name="parameters">The parameters to the webservice.</param>
    /// <returns></returns>
    public IEnumerator makeRequest(String fileName, Hashtable parameters)
    {
        Boolean secret = false;
        LinkedList<String> toHash = new LinkedList<string>();
        
        StringBuilder sb = new StringBuilder();
        //sb.Append(location + fileName + "?");
        

        foreach(String s in parameters.Keys.Cast<String>().OrderBy(c => c))
        {
            if (s.Contains("iden"))
            {
                foreach (String sin in (parameters[s] as String[]))
                {
                    sb.Append("iden[]=" + sin.ToString() + "&");
                    toHash.AddLast(sin);
                }

            }
#if UNITY_ANDROID
            else if (s.Contains("courseid[]"))
            {
                if((parameters[s] as List<int>).Count > 1)
                foreach (int sin in (parameters[s] as List<int>))
                {
                    sb.Append("courseid[]=" + sin.ToString() + "&");
                    toHash.AddLast(sin.ToString());
                }
            
            }
#endif
            else
            {
                if (s.Contains("secret"))
                {
                    secret = true;
                }
                else
                {
                    sb.Append(s + "=" + parameters[s].ToString().ToLower() + "&");
                    if (!s.Equals("function"))
                    {

                        toHash.AddLast(parameters[s].ToString());
                    }
                }
            }
        }


        
        WWW www;
        sb.Append("hash=" + encryptHash(toHash));
        if (secret)
        {
            //Debug.Log(sLocation + fileName + "?" + sb.ToString());
            www = new WWW(sLocation + fileName + "?" + sb.ToString());
        }
        else
        {
            //Debug.Log(location + fileName + "?" + sb.ToString());
            www = new WWW(location + fileName + "?" + sb.ToString());
        }
        yield return www;

        hashtable.Add(parameters["function"], www.text);
    }


    /// <summary>
    /// Method called to start the Coroutine to perform the desired requests.
    /// </summary>
    /// <param name="fileName">Name of the file containing the webservices.</param>
    /// <param name="parameters">The parameters to the webservices.</param>
    public void prepareRequests(String fileName, List<Hashtable> parameters)
    {
        StartCoroutine(requestCycle(fileName, parameters));

    }

    /// <summary>
    /// Method that prepares the requests.
    /// And adds to the list the results
    /// </summary>
    /// <param name="fileName">Name of the file containing the webservices.</param>
    /// <param name="parameters">The parameters to the webservices.</param>
    /// <returns></returns>
    public IEnumerator requestCycle(String fileName, List<Hashtable> parameters)
    {
        String function = null;
        List<String> results = new List<string>();
        foreach (Hashtable h in parameters)
        {
            if (h.ContainsKey("function"))
                function = h["function"].ToString();
            yield return makeRequests(fileName, h, results);

        }

        hashtable.Add(function, results);
    }

    /// <summary>
    /// Method that communicates with the DB to make the requests.
    /// </summary>
    /// <param name="fileName">Name of the file containing the webservices.</param>
    /// <param name="parameters">The parameters to the webservices.</param>
    /// <param name="results">List of all the collected results.</param>
    /// <returns></returns>
    public IEnumerator makeRequests(String fileName, Hashtable parameters, List<String> results)
    {
        Boolean secret = false;
        LinkedList<String> toHash = new LinkedList<string>();

        StringBuilder sb = new StringBuilder();
        //sb.Append(location + fileName + "?");


        foreach (String s in parameters.Keys.Cast<String>().OrderBy(c => c))
        {
            if (s.Contains("iden"))
            {
                foreach (String sin in (parameters[s] as String[]))
                {
                    sb.Append("iden[]=" + sin.ToString() + "&");
                    toHash.AddLast(sin);
                }

            }
#if UNITY_ANDROID
            else if (s.Contains("courseid"))
            {
                foreach (String sin in (parameters[s] as String[]))
                {
                    sb.Append("iden[]=" + sin.ToString() + "&");
                    toHash.AddLast(sin);
                }
            }
#endif
            else
            {
                if (s.Contains("secret"))
                {
                    secret = true;
                }
                else
                {
                    sb.Append(s + "=" + parameters[s] + "&");
                    if (!s.Equals("function"))
                    {

                        toHash.AddLast(parameters[s].ToString());
                    }
                }
            }
        }



        WWW www;
        sb.Append("hash=" + encryptHash(toHash));
        if (secret)
        {
            //Debug.Log(sLocation + fileName + "?" + sb.ToString());
            www = new WWW(sLocation + fileName + "?" + sb.ToString());
        }
        else
        {
            //Debug.Log(location + fileName + "?" + sb.ToString());
            www = new WWW(location + fileName + "?" + sb.ToString());
        }
        yield return www;
        results.Add(www.text);

    }


}
