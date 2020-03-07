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

}

public class GameStartingMessage : MessageBase
{

}

public static class ServerMessageData
{
    
}
