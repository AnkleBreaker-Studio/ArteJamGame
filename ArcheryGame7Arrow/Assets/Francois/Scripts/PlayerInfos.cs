using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    red,
    blue
}

public class PlayerInfos {

    public uint NetworkId;
    public bool IsReadyToStart;
    public bool IsAlive;
    public bool HasArrow;
    public Team Team;

}
