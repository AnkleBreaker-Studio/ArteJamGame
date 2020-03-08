using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[Serializable]
public class PlayerData
{
    public GameObject playerObject;
    public NetworkConnection conn;
}

public class CustomServerNetworkManager : NetworkManager
{
    public Text clientsInfoText;

    [SerializeField] GameManager gamemanagerInstance;

    public int connectedClients = 0;

    [HideInInspector]
    public string serverPassword;

    public List<PlayerData> PlayerList = new List<PlayerData>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameManager.Instance.ServerHandlerRegister();
        connectedClients = 0;
        clientsInfoText.text = "Connected Clients : " + connectedClients;
    }

    //keeping track of Clients connecting.
    public override void OnServerConnect(NetworkConnection conn)
    {
        PlayerList.Add(new PlayerData()
        {
            conn = conn,
            playerObject = null
        });
        PlayerConnectedMessage PCMsg = new PlayerConnectedMessage();
        PCMsg.Name = conn.address;
        PCMsg.ConnectionId = conn.connectionId;
        base.OnServerConnect(conn);
        connectedClients += 1;
        clientsInfoText.text = "Connected Clients : " + connectedClients;
        NetworkServer.SendToAll(PCMsg);
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        PlayerData data = PlayerList.SingleOrDefault(x => x.conn == conn);
        if (data != null) data.playerObject = conn.identity.transform.gameObject;
    }


    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        PlayerList.Remove(PlayerList.SingleOrDefault(x=>x.conn == conn));
        base.OnServerRemovePlayer(conn, player);
    }

    //keeping track of Clients disconnecting.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        connectedClients -= 1;
        clientsInfoText.text = "Connected Clients : " + connectedClients;
        base.OnServerDisconnect(conn);
    }

    private void OnPlayerDisconnected(NetworkConnection player)
    {
        PlayerList.Remove(PlayerList.SingleOrDefault(x=>x.conn == player));
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
    }

    // Call a rpc on every client asking to enable the keyspawner
    public void EnableKeySpawnersContainer()
    {
        NetworkServer.SpawnObjects();
    }
}