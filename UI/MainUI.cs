using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class MainUI : MonoBehaviour
{
    [Header("-- 로그인 관련 --")]
    public InputField inputID;  // 아이디 입력창
    public InputField inputPW;  // 비번 입력창

    public static string id;    // 접속종료를 위해 inputID를 넘겨줌

    public GameObject msgBox;   // 메세지 박스
    public GameObject lobbyBox; // 로비창


    // ■■■■■ 버튼 관련 ■■■■■

    // 오브젝트 활성화
    public void OpenBtnClick(GameObject obj)
    {
        //EventSystem.current.currentSelectedGameObject.SetActive(false);
        obj.SetActive(true);
    }

    // 오브젝트 비활성화
    public void CloseBtnClick()
    {
        // 클릭한 버튼의 오브젝트 비활성화
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }

    // 로그인
    public void Login()
    {
        UserLoginDTO dto = new UserLoginDTO(inputID.text, inputPW.text);


        if (MongoManager.Instance.Login(dto))
        {
            if (MongoManager.Instance.UserExistLogin(dto))
            {
                Debug.Log("로그인 성공");

                id = inputID.text;

                //로그인된 사용자 데이타 로드
                PlayerDTO playerData = MongoManager.Instance.LoadData(inputID.text);

                //게임방에 표시될 정보만 선택
                int killCnt = playerData.killCnt;
                int deathCnt = playerData.deathCnt;

                //포톤클라우드에..사용자 데이타 저장하기
                PhotonNetwork.LocalPlayer.CustomProperties.Add("아이디", inputID.text);
                PhotonNetwork.LocalPlayer.CustomProperties.Add("플레이어 킬수", killCnt);
                PhotonNetwork.LocalPlayer.CustomProperties.Add("플레이어 뎃수", deathCnt);

                OpenBtnClick(lobbyBox);
                CloseBtnClick();
            }
            else
            {
                msgBox.SetActive(true);
                msgBox.GetComponentInChildren<Text>().text = "이미 로그인된 상태입니다";
            }
        }
        else
        {
            msgBox.SetActive(true);
            msgBox.GetComponentInChildren<Text>().text = "아이디 또는 비밀번호가 틀립니다";
        }
    }
}
