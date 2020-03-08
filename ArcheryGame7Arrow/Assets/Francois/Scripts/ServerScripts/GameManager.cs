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
        NetworkServer.RegisterHandler<ClientReadyToStartMessage>(ClientReadyMessageRecieved);
        NetworkServer.RegisterHandler<PlayerDeadMessage>(PlayerDeadMessageRecieved);
        NetworkServer.RegisterHandler<ClientReadyToEndGameMessage>(ClientReadyToEndGameMessageReceived);
        NetworkServer.RegisterHandler<ClientOutOfArrowMessage>(ClientOutOfArrowMessageReceived);
    }

    

    private void ClientReadyToEndGameMessageReceived(NetworkConnection arg1, ClientReadyToEndGameMessage arg2)
    {
        PlayerInfos a = GetPlayerTeam(arg1.identity);
        if (a != null)
            a.ReadyToStop = true;
    }

    void Start()
    {
        ServerNetworkManager = GetComponent<CustomServerNetworkManager>();
        RedTeam = new TeamInfo();
        BlueTeam = new TeamInfo();
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
        if (ServerNetworkManager.PlayerList.Count == ServerNetworkManager.maxConnections && !teamSetted)
        {
            int i = 0;
            foreach (GameObject o in ServerNetworkManager.PlayerList)
            {
                NetworkIdentity netID = o.GetComponent<NetworkIdentity>();
                SetPlayerTeamMessage msg = new SetPlayerTeamMessage();
                msg.Team = (i % 2 == 0) ? Team.blue : Team.red;
                msg.TeamColor = (i % 2 == 0) ? Color.blue : Color.red;

                if (msg.Team == Team.blue)
                    BlueTeam.Players.Add(new PlayerInfos()
                    {
                        NetworkId = netID.netId,
                        Team = Team.blue,
                        IsReadyToStart = false
                    });
                if (msg.Team == Team.red)
                    RedTeam.Players.Add(new PlayerInfos()
                    {
                        NetworkId = netID.netId,
                        Team = Team.red,
                        IsReadyToStart = false
                    });
                msg.NetworkIdentity = netID;
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
            PlayerInfos blueteam = BlueTeam.Players.SingleOrDefault(x => x.IsAlive == true);
            PlayerInfos redteam = RedTeam.Players.SingleOrDefault(x => x.IsAlive == true);
            if (blueteam == null)
            {
                RedTeamWonMessage msg = new RedTeamWonMessage();
                NetworkServer.SendToAll(msg);
            }

            if (redteam == null)
            {
                BlueTeamWonMessage msg = new BlueTeamWonMessage();
                NetworkServer.SendToAll(msg);
            }
            gameEnded = true;
        }
    }

    public void CheckIfDraw()
    {
        PlayerInfos blueTeamPlayer = BlueTeam.Players.SingleOrDefault(x=>x.HasArrow == true);
        PlayerInfos redTeamPlayer = RedTeam.Players.SingleOrDefault(x=>x.HasArrow == true);

        if (blueTeamPlayer == null && redTeamPlayer == null)
        {
            DrawGameMessage msg = new DrawGameMessage();
            NetworkServer.SendToAll(msg);
        }
    }

    public void CanDisconnectAllClient()
    {
        if (gameEnded == true)
        {
            PlayerInfos blueteam = BlueTeam.Players.SingleOrDefault(x => x.ReadyToStop == false);
            PlayerInfos redteam = RedTeam.Players.SingleOrDefault(x => x.ReadyToStop == false);
            if (blueteam == null && redteam == null)
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
        foreach (GameObject o in ServerNetworkManager.PlayerList)
        {
            PlayerInfos a = GetPlayerTeam(arg1.identity);
            if (a != null)
            {
                a.IsReadyToStart = true;
                GameObject gameobject = Instantiate(ServerNetworkManager.playerPrefab);
                NetworkServer.AddPlayerForConnection(arg1, gameobject);
            }
        }
    }
    
    private void PlayerDeadMessageRecieved(PlayerDeadMessage obj)
    {
        PlayerInfos playerInfo = GetPlayerTeam(obj.NetId);
        playerInfo.IsAlive = false;
        playerInfo.HasArrow = false;
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
    
    private void ClientOutOfArrowMessageReceived(NetworkConnection arg1, ClientOutOfArrowMessage arg2)
    {
        print("test");
        PlayerInfos pInfo = GetPlayerTeam(arg1.identity);
        pInfo.HasArrow = false;
    }
}
