using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamInfo
{
    public Team Team;
    public int KeyInChest;
    public List<PlayerInfos> Players;
    public int MaxNumberOfPlayer;
    public bool IsFull;
    public TeamInfo()
    {
        Players = new List<PlayerInfos>();
        IsFull = false;
    }
}

public class GameManager: MonoBehaviour
{
    // Must be set before the build, will determine the number of
    // player in each team
    [SerializeField] public int NbPlayerPerTeam;
    [SerializeField] uint mWinScore;
    public CustomServerNetworkManager ServerNetworkManager;
    public TeamInfo RedTeam;
    public TeamInfo BlueTeam;
    public int KeyRespawnTime;
    public bool GameStart;
    public float GameStartATime = 0;
    public float GameStartWarnTime = 5;
    public bool RedTeamWon;
    public bool BlueTeamWon;

    public int nbPlayerInRoom = 0;

	void Start ()
	{
	    ServerNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomServerNetworkManager>();
        RedTeam = new TeamInfo();
        BlueTeam = new TeamInfo();
	    RedTeam.MaxNumberOfPlayer = NbPlayerPerTeam;
	    BlueTeam.MaxNumberOfPlayer = NbPlayerPerTeam;

	    SetGameInfo();
        DontDestroyOnLoad(this);
        // Create 2 teams
	}

    public void SetGameInfo()
    {
        //Debug.Log("joueur dans team");
        BlueTeam.Players.Clear();
        RedTeam.Players.Clear();
        foreach (GameObject player in ServerNetworkManager.PlayerList)
        {
            PlayerInfos playerinfos = player.GetComponent<PlayerInfos>();
            if (playerinfos.Team == Team.blue)
            {
                BlueTeam.Players.Add(playerinfos);
                if (BlueTeam.Players.Count == BlueTeam.MaxNumberOfPlayer)
                    BlueTeam.IsFull = true;
                else
                    BlueTeam.IsFull = false;
            }
            else
            {
                RedTeam.Players.Add(playerinfos);
                if (RedTeam.Players.Count == RedTeam.MaxNumberOfPlayer)
                    RedTeam.IsFull = true;
                else
                    RedTeam.IsFull = false;
            }

        }
    }
	
    public void ScorePoint(Team pTeamId)
    {
        if (pTeamId == Team.blue)
        {
            BlueTeam.KeyInChest++;
        }
        else
        {
            RedTeam.KeyInChest++;
        }
    }

    void Update()
    {
        if (RedTeam.IsFull && BlueTeam.IsFull)
        {
            GameStart = true;
            GameStartWarnTime -= Time.deltaTime;
            ServerNetworkManager.EnableKeySpawnersContainer();
            if (GameStartWarnTime <= 0)
                StartTimer();
        }
        else
        {
            GameStart = false;
            StopTimer();
        }
        //Debug.Log(RedTeam.KeyInChest + " lol "+ BlueTeam.KeyInChest);
        if (RedTeam.KeyInChest == 3)
        {
            RedTeamWon = true;
        }
        if (BlueTeam.KeyInChest == 3)
        {
            BlueTeamWon = true;
        }
    }

    public void StartTimer()
    {
        //Debug.Log(GameStartATime);
        //Debug.Log("game commencé !!!");
        GameStartATime += Time.deltaTime;
    }

    public void StopTimer()
    {
        GameStartATime = 0;
    }

    public float GetTime()
    {
        return GameStartATime;
    }
}
