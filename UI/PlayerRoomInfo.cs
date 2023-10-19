using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomInfo 
{
    string name;
    int playerCount;
    int maxPlayer;

    public PlayerRoomInfo(string name, int playerCount, int maxPlayer)
    {
        this.name = name;

        this.playerCount = playerCount;
        this.maxPlayer   = maxPlayer;
    }

    public string Name
    {
        get { return name; }
        set { name = value; }

    }

    public int PlayerCount
    {
        get { return playerCount; }
        set { playerCount = value; }

    }

    public int MaxPlayer
    {
        get { return maxPlayer; }
        set { maxPlayer = value; }

    }
}
