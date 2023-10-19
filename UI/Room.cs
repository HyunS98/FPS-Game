using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Room : MonoBehaviourPunCallbacks
{
    public Text roomText;
    public GameObject[] playerList;
    public Button startBtn;

    void Start()
    {
        roomText.text = PhotonNetwork.CurrentRoom.Name;

        PlayerInfoListReset();
        PlayerInfoListUpdate();
    }

    private void Update()
    {
        // 방을 만든 주인이 아니면 버튼 비활성화
        if (!PhotonNetwork.IsMasterClient)
        {
            startBtn.interactable = false;
        }
        else
        {
            startBtn.interactable = true;
        }
    }
    

    // List창 업데이트
    void PlayerInfoListUpdate()
    {
        Player[] player = PhotonNetwork.PlayerList;

        for(int i=0; i<player.Length; i++)
        {
            playerList[i].SetActive(true);

            string name = (string)player[i].CustomProperties["아이디"];
            int killcnt = (int)player[i].CustomProperties["플레이어 킬수"];
            int deathcnt = (int)player[i].CustomProperties["플레이어 뎃수"];

            playerList[i].transform.Find("Number").GetComponent<Text>().text = (i + 1).ToString();
            playerList[i].transform.Find("PlayerID").GetComponent<Text>().text = name;
            playerList[i].transform.Find("PlayerKND").GetComponent<Text>().text = killcnt + " / " + deathcnt;
        }
    }

    // List창 초기화
    void PlayerInfoListReset()
    {
        for (int i=0; i<playerList.Length; i++)
        {
            playerList[i].transform.Find("RoomMaster").GetComponent<Text>().text = "";
            playerList[i].SetActive(false);
        }
    }

    // 플레이어가 들어옴
    public override void OnPlayerEnteredRoom(Player newPlayer)  //플레이어가 방으로 들어옴
    {
        //Debug.Log("새로 방에 들어온 사람 " + newPlayer.CustomProperties["아이디"]);
        PlayerInfoListUpdate();
    }

    // 플레이어가 방에서 나감
    public override void OnPlayerLeftRoom(Player otherPlayer)  
    {
        //Debug.Log("방에서 나간 사람 " + otherPlayer.CustomProperties["아이디"]);

        PlayerInfoListReset();
        PlayerInfoListUpdate();
    }

    // 마스터가 밖으로 나갔을 경우 새로운 마스터 지정 >>>>>>>>> (랜덤으로 새로운 마스터는 어려울까???)
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.SetMasterClient(newMasterClient);
        //Debug.Log("새로운 마스터 : " + newMasterClient.CustomProperties["아이디"]);
    }

    // 게임시작 버튼 클릭
    public void OnStartGameClick()
    {
        PhotonNetwork.LoadLevel(1);
    }

    // 뒤로가기
    public void BackBtnClick()
    {
        Destroy(gameObject);

        GameObject.Find("Canvas").transform.Find("Lobby").gameObject.SetActive(true);

        PhotonNetwork.LeaveRoom();
    }


}
