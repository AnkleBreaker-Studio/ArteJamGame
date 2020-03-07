using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TeamInfo
{
    public Team Team;
    public List<PlayerInfos> Players;
    public int MaxNumberOfPlayer;
    public bool IsFull;
    public TeamInfo()
    {
        Players = new List<PlayerInfos>();
        IsFull = false;
    }
}

public class GameManager: NetworkBehaviour
{
    // Must be set before the build, will determine the number of
    // player in each team
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
    private int NbPlayerPerTeam;

    
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        { 
            return instance; 
        }
    }
     
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this) 
        {
            Destroy(this.gameObject);
        }
 
        instance = this;
        DontDestroyOnLoad( this.gameObject );
    }
    
	void Start ()
	{
	    ServerNetworkManager = GetComponent<CustomServerNetworkManager>();
        NbPlayerPerTeam = ServerNetworkManager.maxConnections / 2;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }
}
