using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubMember : MonoBehaviour
{
    public string PlayerId;
    public string PlayerName;
    public string RoleId;
    public ClubMember(string playerId, string playerName, string roleId)
    {
        PlayerId = playerId;
        PlayerName = playerName;
        RoleId = roleId;
    }
}
