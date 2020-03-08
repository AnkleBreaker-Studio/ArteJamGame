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
    public bool IsReadyToStart = false;
    public bool IsAlive = true;
    public bool HasArrow = true;
    public bool ReadyToStop = false;
    public Team Team;

}
