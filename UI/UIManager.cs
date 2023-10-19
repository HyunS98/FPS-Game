using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static UIManager instance;

    public Text hpText;         // HP 관련
    public Text remainPlayer;   // 남은 플레이어

    public int playerCnt;

    PhotonView pv;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        pv = GetComponent<PhotonView>();
        playerCnt = PhotonNetwork.PlayerList.Length;
    }

    void Start()
    {

    }

    void Update()
    {
        
        remainPlayer.text = playerCnt.ToString();
        //Debug.Log(playerCnt);
    }

    // HP 텍스트
    public void MyHPText(int hp)
    {
        hpText.text = hp.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(playerCnt);
        }
        else
        {
            playerCnt = (int)stream.ReceiveNext();
        }
    }
}
