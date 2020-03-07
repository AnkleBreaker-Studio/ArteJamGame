using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class TeamInfo
{
    public List<PlayerInfos> Players;
    public Color TeamColor;
    public TeamInfo()
    {
        Players = new List<PlayerInfos>();
    }
}

public class GameManager : NetworkBehaviour
{
    // Must be set before the build, will determine the number of
    // player in each team
    [SerializeField] uint mWinScore;
    public CustomServerNetworkManager ServerNetworkManager;

    public TeamInfo RedTeam;
    public TeamInfo BlueTeam;

    private bool teamSetted = false;
    private bool gameStarted = false;

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    private void ServerHandlerRegister()
    {
        NetworkServer.RegisterHandler<ClientReadyToStartMessage>(ClientReadyMessageRecieved);
        NetworkServer.RegisterHandler<PlayerDeadMessage>(PlayerDeadMessageRecieved);
    }
    
    void Start()
    {
        ServerNetworkManager = GetComponent<CustomServerNetworkManager>();
        RedTeam.TeamColor = Color.red;
        BlueTeam.TeamColor = Color.blue;
        ServerHandlerRegister();
    }

    private void FixedUpdate()
    {
        if (ServerNetworkManager.PlayerList.Count == ServerNetworkManager.maxConnections && !teamSetted)
        {
            int i = 0;
            foreach (GameObject o in ServerNetworkManager.PlayerList)
            {
                NetworkIdentity netID = o.GetComponent<NetworkIdentity>();
                SetPlayerTeamMessage msg = new SetPlayerTeamMessage();
                msg.Team = (i % 2 == 0) ? Team.blue : Team.red;
                msg.TeamColor = (msg.Team == Team.blue) ? Color.blue : Color.red;

                BlueTeam.Players.Add(new PlayerInfos()
                {
                    NetworkId = netID.netId,
                    Team = (msg.Team == Team.blue) ? Team.blue : Team.red,
                    IsReadyToStart = false
                });
                msg.NetworkIdentity = netID;
                NetworkServer.SendToAll(msg);
                i++;
            }

            GameReadyToStartMessage readyMsg = new GameReadyToStartMessage();
            NetworkServer.SendToAll(readyMsg);
            teamSetted = true;
        }

        if (IsTeamReady(BlueTeam) && IsTeamReady(RedTeam) && !gameStarted)
        {
            gameStarted = true;
            GameStartMessage msg = new GameStartMessage();
            NetworkServer.SendToAll(msg);
        }

    }

    private void ClientReadyMessageRecieved(NetworkConnection arg1, ClientReadyToStartMessage arg2)
    {
        foreach (GameObject o in ServerNetworkManager.PlayerList)
        {
            PlayerInfos a = GetPlayerTeam(arg1.identity);
            if (a != null)
            {
                a.IsReadyToStart = true;
            }
        }
    }
    
    private void PlayerDeadMessageRecieved(PlayerDeadMessage obj)
    {
        PlayerInfos playerInfo = GetPlayerTeam(obj.NetId);
        playerInfo.IsAlive = false;
    }


    public bool IsTeamReady(TeamInfo team)
    {
        foreach (PlayerInfos playerInfos in team.Players)
        {
            if (playerInfos.IsReadyToStart == false)
                return false;
        }
        return true;
    }

    public PlayerInfos GetPlayerTeam(NetworkIdentity NetId)
    {
        PlayerInfos a = BlueTeam.Players.SingleOrDefault(x => x.NetworkId == NetId.netId);
        if (a == null)
            a = RedTeam.Players.SingleOrDefault(x => x.NetworkId == NetId.netId);
        return a;
    }
}
