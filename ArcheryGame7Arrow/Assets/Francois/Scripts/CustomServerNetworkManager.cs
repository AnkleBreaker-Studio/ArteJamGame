using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CustomServerNetworkManager : NetworkManager
{
    public Text clientsInfoText;

    [SerializeField] GameManager gamemanagerInstance;

    private int connectedClients = 0;

    [HideInInspector]
    public string serverPassword;

    public List<GameObject> PlayerList = new List<GameObject>();

    //Server Side

    public override void OnStartServer()
    {
        base.OnStartServer();
        connectedClients = 0;
        clientsInfoText.text = "Connected Clients : " + connectedClients;
    }

    //keeping track of Clients connecting.
    public override void OnServerConnect(NetworkConnection conn)
    {

        Debug.Log(networkAddress);
        
        base.OnServerConnect(conn);
        connectedClients += 1;
        Debug.Log("Player joined the server :"  + connectedClients);
        clientsInfoText.text = "Connected Clients : " + connectedClients;
        //clientsInfoText.text = "Connected Clients : " + connectedClients;

        //Sending password information to client.
        StringMessage msg = new StringMessage(serverPassword);
        conn.Send(msg);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("PlayerSpawned");
        PlayerList.Clear();
        IEnumerable<GameObject> players = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        foreach (GameObject player in players)
        {

            PlayerInfos info = player.GetComponent<PlayerInfos>();
            info.id = i;
            info.NetworkId = player.GetComponent<NetworkIdentity>().netId;
            info.Team = (info.id < gamemanagerInstance.NbPlayerPerTeam) ? Team.red : Team.blue;
            i++;
            PlayerList.Add((player));
        }
    }

    //keeping track of Clients disconnecting.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        connectedClients -= 1;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        base.OnServerRemovePlayer(conn, player);
    }
    
    private void OnPlayerDisconnected(NetworkConnection player)
    {
        PlayerList.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            PlayerList.Add((players[i]));
        }
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