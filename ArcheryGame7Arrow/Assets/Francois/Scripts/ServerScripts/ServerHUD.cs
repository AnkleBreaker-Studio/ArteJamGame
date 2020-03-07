using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerHUD : MonoBehaviour {

    public GameObject stopServer, startServer, resetSettings, getIP, checking, clientsInfo;
    public Text serverInfoText, portPlaceholderText, paswPlaceholderText, clientsInfoText;
    public InputField portText, passwordText, maxConnText;

    private NetworkManager manager;
    private TelepathyTransport _telepathyTransport;
    private bool noConnection, setText, checkIP;
    private string externalip="?", localIP="?";
    private int maximumConnections;

    // Use this for initialization
    void Start () {
        if (!manager)
            manager = GetComponent<NetworkManager>();
        if (!_telepathyTransport)
            _telepathyTransport = GetComponent<TelepathyTransport>();
        
        //Checking if we have saved Server Infomation and filling the text fields.
        if (PlayerPrefs.HasKey("nwPortS"))
        {
            _telepathyTransport.port = (ushort) Convert.ToInt32(PlayerPrefs.GetString("nwPortS"));
            portPlaceholderText.text = _telepathyTransport.port.ToString();
        }
        if (PlayerPrefs.HasKey("IPAddressS"))
        {
            externalip = PlayerPrefs.GetString("IPAddressS");
            localIP = PlayerPrefs.GetString("LocalIP");
            getIP.GetComponentInChildren<Text>().text = "Server IP Address\nExternal :" + externalip + "\nLocal :" + localIP;
        }
        if (PlayerPrefs.HasKey("Password"))
        {
            passwordText.text = PlayerPrefs.GetString("Password");
            if (passwordText.text == "")
                paswPlaceholderText.text = "(not needed)";
        }
        if (PlayerPrefs.HasKey("MaxConnections"))
        {
            maxConnText.text = PlayerPrefs.GetString("MaxConnections");
        }

        clientsInfoText = clientsInfo.GetComponentInChildren<Text>();
        setText = true;       
    } 
	
	// Update is called once per frame
	void Update () {
        
        noConnection = (NetworkClient.connection == null || NetworkClient.connection.connectionId != -1);

        //Showing and hiding the appropriate buttons and text depending on if the server is running or not.
        if (!manager.IsClientConnected() && !NetworkServer.active)
        {
            if (noConnection)
            {
                stopServer.SetActive(false);
                clientsInfo.SetActive(false);

                if (setText)
                {
                    serverInfoText.color = Color.red;
                    serverInfoText.text = "Server Not Running !";
                    setText = false;
                }
            }
        }
        else
        {
            if (setText)
            {
                serverInfoText.color = new Color(0.2f, 0.6f, 0.2f, 1f);
                string pw="";               
                if (passwordText.text == "")               
                    pw = "(no password)";                
                else pw = passwordText.text;

                string maxConn = "";
                if (maxConnText.text == "")
                    maxConn = "8";
                else maxConn = maxConnText.text;

                serverInfoText.text = "Server Is Running !\n" +"\nIP Address\nExternal : " +externalip+"\nLocal : "+localIP+"\n\nServer Port : " + _telepathyTransport.port +"\nPassword : "+pw+"\nMax Connections : "+ maxConn;
                setText = false;
            }
        }
    }

    //shutdown the server.
    public void StopHostCustom()
    {
        startServer.SetActive(true);
        portText.transform.parent.gameObject.SetActive(true);
        resetSettings.SetActive(true);
        getIP.SetActive(true);
        setText = true;
        manager.StopHost();
    }

    public void StartServerCustom()
    {
        //setting the network managers port to use.
        if (portText.text == "")//did we not set a port number ?
        {
            if (PlayerPrefs.HasKey("nwPortS"))//did we have a previous one saved.
            {
                _telepathyTransport.port = (ushort) Convert.ToInt32(PlayerPrefs.GetString("nwPortS"));
            }
            else//if not, use the default port.
            {
                _telepathyTransport.port = 31029;
                portPlaceholderText.text = _telepathyTransport.port.ToString()+"(Default)";
            }
        }
        else
        {
            PlayerPrefs.SetString("nwPortS", portText.text);//save the port we are using.         
            _telepathyTransport.port = (ushort) Convert.ToInt32(portText.text);
            portPlaceholderText.text = _telepathyTransport.port.ToString();
        }

        PlayerPrefs.SetString("Password", passwordText.text);//save the servers pasword.  
        PlayerPrefs.SetString("MaxConnections", maxConnText.text.ToString());
        //Showing and hiding the appropriate buttons and text.  
        resetSettings.SetActive(false);
        portText.transform.parent.gameObject.SetActive(false);
        getIP.SetActive(false);
        startServer.SetActive(false);
        stopServer.SetActive(true);
        clientsInfo.SetActive(true);
        setText = true;

        if (maxConnText.text != "")
        {
            maximumConnections = Convert.ToInt32(maxConnText.text);
        }
        else maximumConnections = 8;
        manager.maxConnections = maximumConnections;

        manager.StartServer();
        SceneManager.LoadScene(manager.onlineScene);


        //var config = new ConnectionConfig();
        //config.AddChannel(QosType.Reliable);
        //config.AddChannel(QosType.Unreliable);

        //manager.StartServer(config, maximumConnections);
    }

    public void ResetToDefault()
    {
        //deleting all saved info and resetting to use the default ones.
        PlayerPrefs.DeleteKey("IPAddressS");
        getIP.GetComponentInChildren<Text>().text = "Find Server IP Address.";
        externalip = "?";
        PlayerPrefs.DeleteKey("nwPortS");
        portPlaceholderText.text = "7777(Default)";
        portText.text = "";
        PlayerPrefs.DeleteKey("LocalIP");
        localIP = "?";
        PlayerPrefs.DeleteKey("Password");
        paswPlaceholderText.text = "(not needed)";
        passwordText.text = "";
        PlayerPrefs.DeleteKey("MaxConnections");
        maxConnText.text = "";
    }

    //Finding the servers ip addresses.
    public void GetIP()
    {
        getIP.GetComponentInChildren<Text>().text = "If this takes too long\nClick again.";
        StartCoroutine(GetPublicIP());//start the actual checkking.
        checking.SetActive(true);
    }

    IEnumerator GetPublicIP()
    {
        WWW www = new WWW("http://checkip.dyndns.org");//the website to use to find your external ip, use any "find my ip" site you want.
        yield return www;//wait till we get a response.
        if (www.error==null)
        {
            //filter the response message for the ip address.
            string response = www.text;
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            externalip = a4;//TADAA..!!   your external ip addres :)

            //getting the ip from the pc the server is running on. (a local Lan address) 
            //onely used to connect from inside your house/network.
            //localIP = Network.player.ipAddress;
            localIP = "127.0.0.1";

            getIP.GetComponentInChildren<Text>().text = "Server IP Address\nExternal :" + externalip+"\nLocal :"+localIP;
            //saving the ip addresses.
            PlayerPrefs.SetString("IPAddressS", externalip);
            PlayerPrefs.SetString("LocalIP", localIP);
            checking.SetActive(false);
        }
        else
        {
            getIP.GetComponentInChildren<Text>().text = "Someting went wrong\nPlease try again";
            checking.SetActive(false);
        }
    }
}