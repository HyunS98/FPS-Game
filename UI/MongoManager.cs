using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MongoDB.Driver;
using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;
using System;

class MongoManager : MonoBehaviour
{
    public string ip;   
    public int port;

    MongoClient    client;
    IMongoDatabase database;    // 데이터베이스

    //싱글톤 만들기//
    static MongoManager instance = null;

    public static MongoManager Instance 
    { 
        get { return instance; } 
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            Connect();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //[1]접속하기
    public void Connect()
    {
        string uri = "mongodb://" + ip + ":" + port;

        client   = new MongoClient(uri);
        database = client.GetDatabase("KHS_FPS");   // KHS_FPS의 명칭에
    }

    //[2]회원가입(사용자 생성)
    public bool CreateUser(UserDataDTO dto, string userPW2)
    {
        IMongoCollection<UserDataDTO> col = database.GetCollection<UserDataDTO>("UserData");

        if(ID_Check(dto.userID, col) && PW_Check(dto.userPW, userPW2) && !IsExistingID(dto.userID))
        {
            //비밀번호 암호화
            dto.userPW = MD5Encrypt(dto.userPW);

            col.InsertOne(dto);

            return true;
        }

        return false;
    }

    //[3]아이디 중복검사
    public bool IsExistingID(string userID)
    {
        IMongoCollection<UserIDDTO> col = database.GetCollection<UserIDDTO>("UserData");

        string filter = "{userID:'" + userID + "' }";
        string proj   = "{userID:true}";

        List<BsonDocument> list = col.Find(filter).Project(proj).ToList();

        if (list.Count == 0) //데이타베이스에 기존사용 아이디 없음
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //[4]로그인하기
    public bool Login(UserLoginDTO dto)
    {
        IMongoCollection<UserLoginDTO> col = database.GetCollection<UserLoginDTO>("UserData");

        dto.userPW = MD5Encrypt(dto.userPW);

        string filter = "{userID:'" + dto.userID + "'  , userPW:'" + dto.userPW + "' }";
        string proj = "{userID:true }";

        List<BsonDocument> list = col.Find(filter).Project(proj).ToList();

        // 아이디가 없음
        if (list.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // 이미 로그인했는지 확인
    public bool UserExistLogin(UserLoginDTO dto)
    {
        IMongoCollection<UserDataDTO> col = database.GetCollection<UserDataDTO>("UserData");

        string filter = "{userID:'" + dto.userID + "', existingUser:false}";

        var exist = col.Find(filter).FirstOrDefault();

        // 사용중임
        if(exist == null)
        {
            return false;
        }
        else
        {
            exist.existingUser = true;
            col.ReplaceOne(filter, exist);
            return true;
        }
    }

    //[5]게임데이타 저장
    public void SaveData(string userID, PlayerDTO dto)
    {
        // PlayerDTO에서 정보를 업뎃하다가 원할때 SaveData를 부르자

        IMongoCollection<UserIDDTO> col = database.GetCollection<UserIDDTO>("UserData");

        string filter = "{userID:'" + userID + "'}";
        string update = "{$set:{ killCnt:" + dto.killCnt + " , deathCnt:" + dto.deathCnt + ", existingUser:false}}";

        col.UpdateOne(filter, update);
    }

    public void ExitLogin(string userID)
    {
        IMongoCollection<UserDataDTO> col = database.GetCollection<UserDataDTO>("UserData");

        string filter = "{userID:'" + userID + "', existingUser:true}";

        var exist = col.Find(filter).FirstOrDefault();

        exist.existingUser = false;
        col.ReplaceOne(filter, exist);
    }

    //[6]게임데이타 로드
    public PlayerDTO LoadData(string userID)
    {
        IMongoCollection<UserIDDTO> col = database.GetCollection<UserIDDTO>("UserData");

        string filter = "{userID:'" + userID + "'}";
        string proj   = "{killCnt:true, deathCnt:true, modDate:true}";

        List<BsonDocument> list = col.Find(filter).Project(proj).ToList();
        PlayerDTO dto         = new PlayerDTO();

        dto.killCnt  = list[0].GetValue("killCnt").AsInt32;
        dto.deathCnt  = list[0].GetValue("deathCnt").AsInt32;

        return dto;
    }

    //[7]비밀번호 암호화
    public string MD5Encrypt(string text)
    {
        MD5 md5 = MD5.Create();

        byte[] inBytes  = Encoding.UTF8.GetBytes(text);
        byte[] outBytes = md5.ComputeHash(inBytes);

        string res = Convert.ToBase64String(outBytes);

        return res;
    }

    // 중복 아이디 확인
    bool ID_Check(string userID, IMongoCollection<UserDataDTO> collection)
    {
        // 문자열 길이 제한
        if (!(4 <= userID.Length))
        {
            return false;
        }

        // 동일 아이디 확인 (위에 IsExistingID()랑 겹침)
        List<UserDataDTO> list = collection.Find("{userID : '" + userID + "'}").ToList();
        if (list.Count != 0)
        {
            return false;
        }

        return true;
    }

    // PW 제한
    bool PW_Check(string userPW, string pw2)
    {
        // 문자열 길이 제한
        if (!(1 <= userPW.Length))
        {
            return false;
        }

        // 재입력PW 확인
        if (!(pw2 == userPW))    // PW2와 PW가 같다면 false
        {
            return false;
        }

        return true;
    }

    private void OnApplicationQuit()
    {
        if(MainUI.id != null)
        {
            ExitLogin(MainUI.id);
        }
        
    }
}
