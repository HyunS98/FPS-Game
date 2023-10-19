using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

class UserDataDTO
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id;

    public string userID; // 사용자 아이디
    public string userPW; // 사용자 비밀번호
    public int killCnt;   // 플레이어 킬수
    public int deathCnt;    // 플레이어 죽음수
    public bool existingUser;
    public DateTime regDate; // 사용자 회원가입날짜 
    

    public UserDataDTO(string userID, string userPW)
    {
        this.userID = userID;
        this.userPW = userPW;
        this.killCnt = 0;
        this.deathCnt = 0;
        this.existingUser = false;
        this.regDate = DateTime.Now;
    }

}

