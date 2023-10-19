using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public delegate void OnInputBoxFninish(string name, int maxPlayer);

public class CreateBox : MonoBehaviour
{
    public InputField inputField;
    public Dropdown dropdown;
    public OnInputBoxFninish onInputBoxFinish = null;

    //객체 활용가능//
    void OnEnable()
    {
        //버튼 객체를 찾아서..입력막기//
        Button[] buttons = FindObjectsOfType<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].transform.IsChildOf(gameObject.transform) != true)
            {
                buttons[i].interactable = false;
            }
        }      
    }

    //객체 활용 불가능//
    void OnDisable()
    {
        //버튼 객체를 찾아서..입력가능하게 하기//
        Button[] buttons = FindObjectsOfType<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].transform.IsChildOf(gameObject.transform) != true)
            {
                buttons[i].interactable = true;
            }
        }
    }

    //완료 버튼 클릭 이벤트//
    public void OnFinishClick()
    {
        string roomName = inputField.text;
        int maxPlayer = dropdown.value+2;

        if (onInputBoxFinish != null)
        {
            onInputBoxFinish(roomName, maxPlayer);
        }

        CloseBox();
    }

    public void CloseBox()
    {
        Destroy(gameObject);
    }
}
