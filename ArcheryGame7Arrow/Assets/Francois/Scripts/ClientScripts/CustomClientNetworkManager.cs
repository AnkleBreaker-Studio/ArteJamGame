using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CustomClientNetworkManager : NetworkManager
{
    public Text clientsInfoText;
    public ClientHUD clientHudScript;


    [HideInInspector]
    public string serverPassword;

    //Client Side
    public override void OnStartClient()
    {
        base.OnStartClient();
        RegisterClientHandles();
        ClientGameManager.Instance.RegisterHandlers();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        clientHudScript.ConnectSuccses();
    }

    //when client recieves password information from the server.
    public void OnReceivePassword(NetworkMessage netMsg)
    {
        //read the server password.
        var msg = netMsg.ReadMessage<StringMessage>().value;
        //serverPassword = msg;
        if (msg != clientHudScript.passwordText.text)
            clientHudScript.DisConnect(true);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        clientHudScript.DisConnect(false);
    }

    //Messages that need to be Registered on Server and Client Startup.

    void RegisterClientHandles()
    {
        //NetworkClient.RegisterHandler<StringMessage>(OnReceivePassword);
    }
}