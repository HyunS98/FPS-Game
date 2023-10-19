using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SignUp : MonoBehaviour
{
    [Header("-- 회원가입 관련 --")]
    public InputField inputCreateID;    // 아이디 입력
    public InputField inputCreatePW;    // 비번 입력
    public InputField inputReCreatePW;  // 비번 재입력

    public GameObject msgBox;   // 메세지 박스

    // 회원가입
    public void SignUpBtnClick()
    {
        UserDataDTO dto = new UserDataDTO(inputCreateID.text, inputCreatePW.text);

        // 가입 가능여부
        if(MongoManager.Instance.CreateUser(dto, inputReCreatePW.text))
        {
            // 메시지 박스 띄우고 가입창 닫기
            msgBox.SetActive(true);
            msgBox.GetComponentInChildren<Text>().text = "가입완료!!";
            EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);

            // 정보 초기화
            inputCreateID.text = "";
            inputCreatePW.text = "";
            inputReCreatePW.text = "";
        }
        else
        {
            msgBox.SetActive(true);

            if (MongoManager.Instance.IsExistingID(inputCreateID.text))
            {
                msgBox.GetComponentInChildren<Text>().text = "아이디가 중복이에요!!";
            }
            else
            {
                msgBox.GetComponentInChildren<Text>().text = "비밀번호를 확인해주세요!!";
            }
            
            
        }
    }

    
}
