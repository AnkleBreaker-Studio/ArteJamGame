using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class ClientTeamInfo
{
    public NetworkIdentity NetId;
    public bool IsAlive = true;
    public bool HasArrow = true;
    public int NumberOfArrows;
    public Team Team;
    public Color TeamColor;
}


public class ClientGameManager : MonoBehaviour
{
    public List<ClientTeamInfo> playerList = new List<ClientTeamInfo>();

    #region Singleton
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
    #endregion
    
    private void Start()
    {
        RegisterHandlers();
    }

    private void OnConnectedToServer()
    {
        print("Connected to server");
    }
    
    public void RegisterHandlers()
    {
        NetworkClient.RegisterHandler<PlayerConnectedMessage>(PlayerConnectedMessageReceived);
        NetworkClient.RegisterHandler<PlayerGotKilledMessage>(PlayerGotKilledMessageReceived);
        NetworkClient.RegisterHandler<GameStartingMessage>(GameStartingMessageReceived);
        NetworkClient.RegisterHandler<GameReadyToStartMessage>(GameReadyToStartMessageReceived);
        NetworkClient.RegisterHandler<SetPlayerTeamMessage>(SetPlayerTeamMessageReceived);
        NetworkClient.RegisterHandler<GameStartMessage>(GameStartMessageReceived);
        NetworkClient.RegisterHandler<RedTeamWonMessage>(RedTeamWonMessageReceived);
        NetworkClient.RegisterHandler<BlueTeamWonMessage>(BlueTeamWonMessageReceived);
        NetworkClient.RegisterHandler<DrawGameMessage>(DrawGameMessageReceived);
        NetworkClient.RegisterHandler<DisconnectMessage>(DisconnectMessageReceived);
    }

    private void DisconnectMessageReceived(NetworkConnection arg1, DisconnectMessage arg2)
    {
        playerList.Remove(playerList.SingleOrDefault(x => x.NetId == arg1.identity));
    }

    public IEnumerator RedTeamWon()
    {
        yield return null;
    }
    
    public IEnumerator BlueTeamWon()
    {
        yield return null;
    }
    
    public IEnumerator Draw()
    {
        yield return null;
    }
    
    public IEnumerator StartGame()
    {
        yield return null;
        ClientReadyToStartMessage msg = new ClientReadyToStartMessage();
        NetworkClient.Send(msg);
    }
    
    private void DrawGameMessageReceived(NetworkConnection arg1, DrawGameMessage arg2)
    {
        print("Draw");
        StartCoroutine(Draw());
        /*
         *
         *activate enum of end of game message, play sound and animations for X seconds (duration of the anim / sound)
         * Then call ClientReadyToEndGameMessage to make the server disconnect all the client.
         */
    }

    private void BlueTeamWonMessageReceived(NetworkConnection arg1, BlueTeamWonMessage arg2)
    {
        print("blueTeamWon");
        StartCoroutine(BlueTeamWon());

        /*
         *
         *activate enum of end of game message, play sound and animations for X seconds (duration of the anim / sound)
         * Then call ClientReadyToEndGameMessage to make the server disconnect all the client.
         */
    }

    private void RedTeamWonMessageReceived(NetworkConnection arg1, RedTeamWonMessage arg2)
    {
        print("redTeamWon");
        StartCoroutine(RedTeamWon());
        /*
         *
         *activate enum of end of game message, play sound and animations for X seconds (duration of the anim / sound)
         * Then call ClientReadyToEndGameMessage to make the server disconnect all the client.
         */
    }
    
    private void GameReadyToStartMessageReceived(NetworkConnection arg1, GameReadyToStartMessage arg2)
    {
        print("GameReadyToStart");
        if (NetworkClient.connection == arg1)
            StartCoroutine(StartGame());
        /*
        *
        *activate enum of start of game message, play sound and animations for X seconds (duration of the anim / sound)
        * Then call ClientReadyToStartMessage to make the server make all the client to start
        */
    }

    private void GameStartMessageReceived(NetworkConnection arg1, GameStartMessage arg2)
    {
        print("startGame");
        /*Starting the round*/
    }

    private void SetPlayerTeamMessageReceived(NetworkConnection arg1, SetPlayerTeamMessage arg2)
    {
        ClientTeamInfo clientInfo = playerList.SingleOrDefault(x => x.NetId == arg1.identity);
        if (clientInfo != null)
        {
            clientInfo.Team = arg2.Team;
            clientInfo.TeamColor = arg2.TeamColor;
            clientInfo.NumberOfArrows = 7;
            clientInfo.IsAlive = true;
            clientInfo.HasArrow = true;
        }
    }

    private void GameStartingMessageReceived(NetworkConnection arg1, GameStartingMessage arg2)
    {
        /*not implemented*/
    }

    private void PlayerGotKilledMessageReceived(NetworkConnection arg1, PlayerGotKilledMessage arg2)
    {
        ClientTeamInfo clientInfo = playerList.SingleOrDefault(x => x.NetId == arg1.identity);
        if (clientInfo != null)
        {
            clientInfo.IsAlive = false;
            clientInfo.HasArrow = false;
        }
    }

    private void PlayerConnectedMessageReceived(NetworkConnection arg1, PlayerConnectedMessage arg2)
    {
        playerList.Add(new ClientTeamInfo()
        {
            NetId = arg1.identity,
            HasArrow = true,
            IsAlive = true,
            NumberOfArrows = 7
        });
    }
    

}
