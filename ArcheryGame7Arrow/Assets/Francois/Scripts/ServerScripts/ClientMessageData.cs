
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;


public class ClientReadyToStartMessage : MessageBase
{
    public NetworkIdentity NetId;
}

public class PlayerDeadMessage : MessageBase
{
    public int NetId;
}

public class ClientReadyToEndGameMessage : MessageBase
{
}

public class ClientOutOfArrowMessage : MessageBase
{
}


public static class ClientMessageData
{
    
}
