using System;

class PlayerDTO
{
    public string userID;   // 플레이어 아이디
    public int killCnt;     // 플레이어 킬수
    public int deathCnt;    // 플레이어 죽음수

    public PlayerDTO()
    { }

    public PlayerDTO(string userID, int killCnt, int deathCnt)
    {
        this.userID = userID;
        this.killCnt = killCnt;
        this.deathCnt = deathCnt;
    }
}

