using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    red,
    blue
}

public class PlayerInfos : MonoBehaviour {

    public int id = -1;
    public uint NetworkId;
    public Team Team;

    public Team GetTeam()
    {
        return Team;
    }
}
