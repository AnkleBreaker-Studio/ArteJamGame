using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CustomClientNetworkManager : NetworkManager
{
    public Text clientsInfoText;
    public ClientHUD clientHudScript;
    public GameObject ClientHud;
    public GameObject GameStartingUI;
    public GameObject WaitingForPlayerUI;
    public GameObject LoosePannelUI;
    public GameObject WinPannelUI;
    public GameObject DrawPannelUI;
    public UILabel WaitingForPlayerUILabel;
    public UILabel GameStartingUiLabel;

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
        ClientHud.SetActive(false);
        WaitingForPlayerUI.SetActive(true);
        WaitingForPlayerUILabel.text = "en attente des joueurs";
    }

    //when client recieves password information from the server.
    public void OnReceivePassword(NetworkMessage netMsg)
    {
        //read the server password.
        var msg = netMsg.ReadMessage<StringMessage>().value;
        //serverPassword = msg;
        clientHudScript.DisConnect(true);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientHud.SetActive(true);
        clientHudScript.DisConnect(false);
    }

    //Messages that need to be Registered on Server and Client Startup.
    void RegisterClientHandles()
    {
        //NetworkClient.RegisterHandler<StringMessage>(OnReceivePassword);
    }
}