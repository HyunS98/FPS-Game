using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{
    public GameObject messageBox;       // 경고창 
    public GameObject roomBox;          // 로비UI
    public GameObject createBox;        // 방 만들기
    public GameObject[] playerRoomList; // 생성한 방을 보여줄 리스트
    
    private List<PlayerRoomInfo> playerRoomInfoList;    // 생성된 방 리스트

    private void Awake()
    {
        // 네트워크 씬 자동 로드 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // 포톤 클라우드 접속하기
        PhotonNetwork.ConnectUsingSettings();  
        
        ResetPlayerRoomList();                          // UI 게임방목록 초기화(리셋) 하기
        playerRoomInfoList = new List<PlayerRoomInfo>();// 플레이어 룸 정보 리스트 객체 생성
    }

    // 포톤클라우드 콜백함수(이벤트 처리)함수
    public override void OnConnected()  // 포톤클라우드 접속됨
    {
        Debug.Log("포톤클라우드 접속됨");
    }

    // 포톤클라우드 --> 마스터로 접속됨
    public override void OnConnectedToMaster() 
    {
        Debug.Log("포톤클라우드 마스터에 접속됨");

        //게임로비에 들어가기//
        PhotonNetwork.JoinLobby();
    }

    // 포톤클라우드 연결 안됨
    public override void OnDisconnected(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.ExceptionOnConnect:
                Debug.Log("서버에 접속할 수 없습니다");
                break;

            case DisconnectCause.DisconnectByClientLogic:
                Debug.Log("사용자가 접속을 종료했습니다"); 
                break;

            default:
                Debug.Log("서버 접속 오류가 발생했습니다");
                break;
        }
    }

    // 게임 로비에 참여 성공
    public override void OnJoinedLobby()  
    {
        Debug.Log("게임로비에 들어감");   
    }

    // 게임방을 생성 성공
    public override void OnCreatedRoom()  
    {
        Debug.Log("게임방을 생성함");
    }

    // 게임방에 들어감
    public override void OnJoinedRoom() 
    {
        Debug.Log("게임방에 들어감");

        // 방 입장 및 
        Instantiate(roomBox);
        gameObject.SetActive(false);
    }

    // 게임방 생성 실패
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // 똑같은 방제가 있을 때
        if (CheckDoubleRoomName(name))
        {
            messageBox.SetActive(true);
            messageBox.GetComponentInChildren<Text>().text = "똑같은 방명이 존재합니다.";
        }

        Debug.Log("[게임방 만들기 실패] 에러코드 :" + returnCode + ", 에러메시지 : " + message);
    }

    // 게임방 입장 실패
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("[게임방 들어가기 실패] 에러코드 :" + returnCode + ", 에러메시지 : " + message);
    }

    // 게임방 생성 버튼 클릭 이벤트 함수
    public void OnCreateRoomClick()
    {
        // 리스트 갯수가 5개 이상이면..
        if (playerRoomInfoList.Count >= 5)
        {
            messageBox.SetActive(true);
            messageBox.GetComponentInChildren<Text>().text = "생성할 수 있는 최대치를 만들었습니다.";
        }
        else
        {
            // delegate로 함수 전달
            GameObject obj = Instantiate(createBox);
            obj.GetComponent<CreateBox>().onInputBoxFinish = OnCreateRoomFunc;
        }
    }

    // 게임방 만들기 함수
    public void OnCreateRoomFunc(string name, int maxPlayer)
    {
        RoomOptions options = new RoomOptions();  // 게임룸 생성 옵션
        options.MaxPlayers  = (byte)maxPlayer;    // 최대참여인원 수

        PhotonNetwork.CreateRoom(name, options);  // 방 생성
    }

    // 게임방 목록...전달 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList != true)   // 새로운 방 생성 / 기존방에 정보가 변경될때
            {
                Debug.Log("업데이트 될 게임방 이름 :" + info.Name);

                int i = GetPlyaerRoomInfoIndex(info.Name);

                if(i!=-1)   // 기존방 수정
                {
                    playerRoomInfoList[i].MaxPlayer = info.MaxPlayers;
                    playerRoomInfoList[i].PlayerCount = info.PlayerCount;
                }
                else    // 새로운방 추가
                {
                    // 게임방 목록 추가
                    string name = info.Name;
                    int playerCount = info.PlayerCount;
                    int maxPlayer = info.MaxPlayers;

                    // playerRoomInfoList.Count번째 리스트 활성화
                    playerRoomList[playerRoomInfoList.Count].SetActive(true);

                    // 방 생성
                    PlayerRoomInfo room = new PlayerRoomInfo(name, playerCount, maxPlayer);
                    playerRoomInfoList.Add(room);
                }
            }
            else
            {
                Debug.Log("제거된 게임방 이름 :" + info.Name);

                int i = GetPlyaerRoomInfoIndex(info.Name);

                //리스트에서 삭제방 이름 검색하기//
                if (i != -1)
                { 
                    playerRoomInfoList.RemoveAt(i);
                    playerRoomList[playerRoomInfoList.Count].SetActive(false);
                }
                

            }
        }

        // UI 전체 방목록 제거하기
        ResetPlayerRoomList();

        // PlayerRoomInfo 리스트의 내용을  UI로 표시하기//
        for (int i = 0; i < playerRoomInfoList.Count; i++)
        {
            // i 번째 playerRoomInfoList ===> i 번째 playerRoomList UI로 출력
            string num   = (i+1).ToString();
            string name  = playerRoomInfoList[i].Name;
            string count = playerRoomInfoList[i].PlayerCount + "/" + playerRoomInfoList[i].MaxPlayer;

            // ui 출력
            playerRoomList[i].transform.Find("Number").GetComponent<Text>().text = num;
            playerRoomList[i].transform.Find("RoomTitle").GetComponent<Text>().text = name;
            playerRoomList[i].transform.Find("RoomPeople").GetComponent<Text>().text = count;
            playerRoomList[i].SetActive(true);
        }
    }

    // 게임방 목록에서 이름으로 인덱스 찾아오기
    int GetPlyaerRoomInfoIndex(string name)
    {
        for(int i=0; i<playerRoomInfoList.Count; i++)
        {
            if(playerRoomInfoList[i].Name == name)
            {
                return i;
            }
        }

        return -1;
    }

    // UI플레이어게임방 목록 리셋 함수
    public void ResetPlayerRoomList()
    {
        for (int i = 0; i < playerRoomList.Length; i++)
        {
            //playerRoomList[i].transform.Find("Number").GetComponent<Text>().text = "";
            //playerRoomList[i].transform.Find("RoomTitle").GetComponent<Text>().text = "";
            //playerRoomList[i].transform.Find("RoomPeople").GetComponent<Text>().text = "";

            playerRoomList[i].SetActive(false);
        }
    }

    // 참여 버튼 클릭 이벤트 처리함수
    public void OnJoinRoomClick(int i)
    {
        string name = playerRoomInfoList[i].Name;   // i번째 게임방이름

        // Room 접속 가능 인원 제한
        if(playerRoomInfoList[i].MaxPlayer > playerRoomInfoList[i].PlayerCount)
        {
            // 포톤클라우드에서 게임방으로 들어가기
            PhotonNetwork.JoinRoom(name);
        }
        else
        {
            // 다 찼을때 경고문
            messageBox.SetActive(true);
            messageBox.GetComponentInChildren<Text>().text = "자리가 없으니 다른방을 찾아주세요!";
        }
    }

    // 똑같은 방제가 있을 경우
    bool CheckDoubleRoomName(string roomName)
    {
        for(int i=0; i<playerRoomInfoList.Count; i++)
        {
            if(playerRoomInfoList[i].Name == roomName.Trim())
            {
                return true;
            }
        }

        return false;
    }
}
