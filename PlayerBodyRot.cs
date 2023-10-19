using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerBodyRot : MonoBehaviourPunCallbacks, IPunObservable
{
    Transform upbody;       // 모델링 상체
    PlayerAll player;       // 플레이어 스크립트
    Transform cam;          // 카메라 좌표
    Transform upp;          // 가슴
    PhotonView pv;          // 포톤 뷰
    Quaternion playerBodySpine;  // 움직일 상체(가슴 위)

    void Start()
    {
        player = GetComponent<PlayerAll>();
        pv = GetComponent<PhotonView>();

        upbody = player.ani.GetBoneTransform(HumanBodyBones.UpperChest);
        upp = upbody.parent;

        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    
    void Update()
    {
        // 자신 확인
        if(pv.IsMine)
        {
            // 조준 중일때
            if (player.Aim)
            {
                // 움직임 체크
                if (!player.GetMove)
                {
                    // 수치를 조정하면 상체가 바뀌면서 총도 같이 기울어짐
                    playerBodySpine = Quaternion.Slerp(playerBodySpine, cam.transform.rotation * Quaternion.Euler(new Vector3(20, 50, 0)), 1f); // Quaternion은 애니메이션의 상체 각도 조정
                }
                else
                {
                    playerBodySpine = Quaternion.Slerp(playerBodySpine, cam.transform.rotation * Quaternion.Euler(new Vector3(0, 50, 0)), 1f);
                }
            }
            // 조준 중이 아닐때
            else
            {
                playerBodySpine = Quaternion.Slerp(playerBodySpine, upp.rotation, 1f);
            }
        }
    }

    // 본애니메이션의 움직임은 LateUpdate에서만 적용해야함
    private void LateUpdate()
    {
        upbody.rotation = playerBodySpine;
    }

    // IPunObservable 상속 받아, 데이터를 사용자간 보내거나 받음 (2명 이상의 참여자가 있어야 적용함)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*
            stream - 데이터를 주고 받는 통로
            Tip) 1. 예시로 rotation 값을 그대로 보내면 참조할 변수가 없다며 에러 발생함, 그러니 새로운 변수를 만들어서 사용하는걸 추천
                 2. Photon transform view, Photon animator view 같은 컴포넌트 등과 같이 사용하면 에러가 발생할 수 있으니 PhotonView만 사용하는걸 추천
        */

        if (stream.IsWriting) // 내가 데이터를 보냄
        {
            stream.SendNext(playerBodySpine);
        }
        else // 내가 데이터를 받음 (데이터형 작성 필수)
        {
            playerBodySpine = (Quaternion)stream.ReceiveNext();          
        }
    }
}
