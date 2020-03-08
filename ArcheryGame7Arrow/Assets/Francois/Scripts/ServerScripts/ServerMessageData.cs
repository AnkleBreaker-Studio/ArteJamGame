using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerConnectedMessage : MessageBase
{
    public int ConnectionId;
    public string Name;
}

public class PlayerGotKilledMessage : MessageBase
{
    public int PlayerId;
}


public class SetPlayerTeamMessage : MessageBase
{
    public int NetId;
    public Team Team;
    public Color TeamColor;
}

public class GameReadyToStartMessage : MessageBase
{
}

public class GameStartingMessage : MessageBase
{

}


public class GameStartMessage : MessageBase
{
    
}

public class RedTeamWonMessage : MessageBase
{
    
}

public class BlueTeamWonMessage : MessageBase
{
    
}

public class DrawGameMessage : MessageBase
{
    
}

public static class ServerMessageData
{
    
}