using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

[Serializable]
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
    public CustomServerNetworkManager ServerNetworkManager;

    public TeamInfo RedTeam;
    public TeamInfo BlueTeam;

    private bool teamSetted = false;
    private bool gameStarted = false;
    private bool gameEnded = false;
    
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


    public void ServerHandlerRegister()
    {
        NetworkServer.RegisterHandler<ClientReadyToStartMessage>(ClientReadyMessageRecieved, false);
        NetworkServer.RegisterHandler<ClientSpawnPlayerMessage>(ClientSpawnPlayerMessageRecieved, false);
        NetworkServer.RegisterHandler<PlayerDeadMessage>(PlayerDeadMessageRecieved, false);
        NetworkServer.RegisterHandler<ClientReadyToEndGameMessage>(ClientReadyToEndGameMessageReceived, false);
        NetworkServer.RegisterHandler<ClientOutOfArrowMessage>(ClientOutOfArrowMessageReceived, false);
    }

    private void ClientSpawnPlayerMessageRecieved(NetworkConnection arg1, ClientSpawnPlayerMessage arg2)
    {
        GameObject gameobject = Instantiate(ServerNetworkManager.playerPrefab);
        NetworkServer.AddPlayerForConnection(arg1, gameobject);
    }


    private void ClientReadyToEndGameMessageReceived(NetworkConnection arg1, ClientReadyToEndGameMessage arg2)
    {
        PlayerInfos a = GetPlayerTeam(arg1.connectionId);
        if (a != null)
            a.ReadyToStop = true;
    }

    void Start()
    {
        ServerNetworkManager = GetComponent<CustomServerNetworkManager>();
        RedTeam = new TeamInfo();
        BlueTeam = new TeamInfo();
        BlueTeam.Players = new List<PlayerInfos>();
        RedTeam.Players = new List<PlayerInfos>();
        RedTeam.TeamColor = Color.red;
        BlueTeam.TeamColor = Color.blue;
    }

    private void FixedUpdate()
    {
        CheckIfLobbyIsFull();
        CheckForGameToStart();
        CheckIfTeamWin();
        CheckIfDraw();
        CanDisconnectAllClient();
    }

    public void CheckIfLobbyIsFull()
    {
        if (ServerNetworkManager.connectedClients == ServerNetworkManager.maxConnections && !teamSetted)
        {
           
            print("GO START ASSIGN TEAM");
            int i = 0;
            foreach (PlayerData o in ServerNetworkManager.PlayerList)
            {
                NetworkConnection netID = o.conn;
                SetPlayerTeamMessage msg = new SetPlayerTeamMessage();
                msg.Team = (i % 2 == 0) ? Team.blue : Team.red;
                msg.TeamColor = (i % 2 == 0) ? Color.blue : Color.red;
                if (msg.Team == Team.blue)
                    BlueTeam.Players.Add(new PlayerInfos()
                    {
                        NetworkId = o.conn.connectionId,
                        Team = Team.blue,
                        IsReadyToStart = false
                    });
                if (msg.Team == Team.red)
                    RedTeam.Players.Add(new PlayerInfos()
                    {
                        NetworkId = o.conn.connectionId,
                        Team = Team.red,
                        IsReadyToStart = false
                    });
                msg.NetId = netID.connectionId;
                NetworkServer.SendToAll(msg);
                print("send1");
                i++;
            }

            GameReadyToStartMessage readyMsg = new GameReadyToStartMessage();
            NetworkServer.SendToAll(readyMsg);
            print("send2");
            teamSetted = true;
        }
    }
    
    public void CheckForGameToStart()
    {
        if (IsTeamReady(BlueTeam) && IsTeamReady(RedTeam) && !gameStarted)
        {
            gameStarted = true;
            GameStartMessage msg = new GameStartMessage();
            NetworkServer.SendToAll(msg);
        }
    }
    
    public void CheckIfTeamWin()
    {
        if (gameStarted && !gameEnded)
        {
            int blueteam = BlueTeam.Players.Count(x => x.IsAlive);
            int redteam = RedTeam.Players.Count(x => x.IsAlive);
            if (blueteam == 0)
            {
                RedTeamWonMessage msg = new RedTeamWonMessage();
                NetworkServer.SendToAll(msg);
            }

            if (redteam == 0)
            {
                BlueTeamWonMessage msg = new BlueTeamWonMessage();
                NetworkServer.SendToAll(msg);
            }
            gameEnded = true;
        }
    }

    public void CheckIfDraw()
    {
        if (gameStarted)
        {
            int blueTeamPlayer = BlueTeam.Players.Count(x => x.HasArrow);
            int redTeamPlayer = RedTeam.Players.Count(x => x.HasArrow);

            if (blueTeamPlayer == 0 && redTeamPlayer == 0)
            {
                DrawGameMessage msg = new DrawGameMessage();
                NetworkServer.SendToAll(msg);
            }
        }
    }

    public void CanDisconnectAllClient()
    {
        if (gameEnded == true)
        {
            int blueteam = BlueTeam.Players.Count(x => x.ReadyToStop == false);
            int redteam = RedTeam.Players.Count(x => x.ReadyToStop == false);
            if (blueteam == 0 && redteam == 0)
            {
                NetworkServer.DisconnectAllConnections();
                RedTeam.Players.Clear();
                BlueTeam.Players.Clear();
                teamSetted = false;
                gameStarted = false;
                gameEnded = false;
            }
        }
    }
    private void ClientReadyMessageRecieved(NetworkConnection arg1, ClientReadyToStartMessage arg2)
    {
        foreach (PlayerData o in ServerNetworkManager.PlayerList)
        {
            PlayerInfos a = GetPlayerTeam(arg1.connectionId);
            if (a != null)
            {
                a.IsReadyToStart = true;
            }
        }
    }
    
    private void PlayerDeadMessageRecieved(NetworkConnection arg1, PlayerDeadMessage obj)
    {
        PlayerInfos playerInfo = GetPlayerTeam(arg1.connectionId);
        playerInfo.IsAlive = false;
        playerInfo.HasArrow = false;
    }


    public bool IsTeamReady(TeamInfo team)
    {
        if (team.Players.Count == 0)
            return false;
        foreach (PlayerInfos playerInfos in team.Players)
        {
            if (playerInfos.IsReadyToStart == false)
                return false;
        }
        return true;
    }

    public PlayerInfos GetPlayerTeam(int NetId)
    {
        PlayerInfos a = BlueTeam.Players.SingleOrDefault(x => x.NetworkId == NetId);
        if (a == null)
            a = RedTeam.Players.SingleOrDefault(x => x.NetworkId == NetId);
        return a;
    }
    
    private void ClientOutOfArrowMessageReceived(NetworkConnection arg1, ClientOutOfArrowMessage arg2)
    {
        print("test");
        PlayerInfos pInfo = GetPlayerTeam(arg1.connectionId);
        pInfo.HasArrow = false;
    }
}
