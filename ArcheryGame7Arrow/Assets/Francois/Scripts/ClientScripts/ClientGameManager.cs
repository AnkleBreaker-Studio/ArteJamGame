using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ClientTeamInfo
{
    public NetworkIdentity NetId;
    public bool IsAlive = true;
    public bool HasArrow = true;
    public Team Team;
    public Color TeamColor;
}


public class ClientGameManager : NetworkBehaviour
{
    public List<ClientTeamInfo> playerList = new List<ClientTeamInfo>();
    
    
    
    private static ClientGameManager instance = null;

    public static ClientGameManager Instance
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

    public void RegisterHandlers()
    {
        NetworkClient.RegisterHandler<PlayerConnectedMessage>(PlayerConnectedMessageReceived);
        NetworkClient.RegisterHandler<PlayerGotKilledMessage>(PlayerGotKilledMessageReceived);
        NetworkClient.RegisterHandler<GameStartingMessage>(GameStartingMessageReceived);
        NetworkClient.RegisterHandler<SetPlayerTeamMessage>(SetPlayerTeamMessageReceived);
        NetworkClient.RegisterHandler<GameReadyToStartMessage>(GameReadyToStartMessageReceived);
        NetworkClient.RegisterHandler<GameStartMessage>(GameStartMessageReceived);
        NetworkClient.RegisterHandler<RedTeamWonMessage>(RedTeamWonMessageReceived);
        NetworkClient.RegisterHandler<BlueTeamWonMessage>(BlueTeamWonMessageReceived);
        NetworkClient.RegisterHandler<DrawGameMessage>(DrawGameMessageReceived);
    }

    private void DrawGameMessageReceived(NetworkConnection arg1, DrawGameMessage arg2)
    {
    }

    private void BlueTeamWonMessageReceived(NetworkConnection arg1, BlueTeamWonMessage arg2)
    {
    }

    private void RedTeamWonMessageReceived(NetworkConnection arg1, RedTeamWonMessage arg2)
    {
    }

    private void GameStartMessageReceived(NetworkConnection arg1, GameStartMessage arg2)
    {
    }

    private void GameReadyToStartMessageReceived(NetworkConnection arg1, GameReadyToStartMessage arg2)
    {
    }

    private void SetPlayerTeamMessageReceived(NetworkConnection arg1, SetPlayerTeamMessage arg2)
    {
    }

    private void GameStartingMessageReceived(NetworkConnection arg1, GameStartingMessage arg2)
    {
    }

    private void PlayerGotKilledMessageReceived(NetworkConnection arg1, PlayerGotKilledMessage arg2)
    {
    }

    private void PlayerConnectedMessageReceived(NetworkConnection arg1, PlayerConnectedMessage arg2)
    {
        playerList.Add(new ClientTeamInfo()
        {
            NetId = arg1.identity,
            HasArrow = true,
        });
    }

    private void Start()
    {
        RegisterHandlers();
    }

    
}
