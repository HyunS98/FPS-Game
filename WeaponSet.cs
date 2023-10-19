using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WeaponSet : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject[] weapon;     // 무기 (주무기1, 주무기2, 권총)
    public Transform weaponPos;
    PlayerAll player;               // 플레이어
    AimBehavior aimBehavior;        // 조준
    PhotonView pv;

    // 플레이어 애니메이션별 총 위치 및 회전수치 조정
    Vector3 basic_pos = new Vector3(-0.083f, 0.222f, 0.013f);   // 안움직임 경우(Position)
    Vector3 basic_rot = new Vector3(78, -42.67f, 47.47f);       // 안움직임 경우(rotation)
    Vector3 change_pos = new Vector3(-0.081f, 0.232f, 0.058f);  // 움직임 경우(Position)
    Vector3 change_rot = new Vector3(80.7f, -138.7f, -47.6f);   // 움직일 경우(rotation)

    Vector3 pos;    // 변경할 좌표 값
    Vector3 rot;    // 변경할 회전 값

    void Awake()
    {
        player = GetComponent<PlayerAll>();
        aimBehavior = GetComponent<AimBehavior>();
        pv = GetComponent<PhotonView>();

        // 기본 총
        weapon[0].SetActive(true);
        player.playerSo.weaponSO = weapon[0].GetComponent<WpScript>().weaponSo;

        // weapon에 장착하여 가져온 무기 3가지를 추가
        // 상점을 우선만들어볼까??
    }


    void Update()
    {
        // 지속적으로 변경될 Transform 값
        weaponPos.localPosition = pos;
        weaponPos.localEulerAngles = rot;
        
        if (pv.IsMine)
        {
            // 행동에 따른 WeaponTransform 값
            ChangeWeaponTransform();

            // 입력값 1 -> ak
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //ChangeWep((int)KeyCode.Alpha1 - 48);
                pv.RPC("ChangeWep", RpcTarget.All, ((int)KeyCode.Alpha1 - 48));
            }

            // 입력값 2 -> kar9
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //ChangeWep((int)KeyCode.Alpha2 - 48);
                pv.RPC("ChangeWep", RpcTarget.All, ((int)KeyCode.Alpha2 - 48));
            }

            // 입력값 3 -> m4
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //ChangeWep((int)KeyCode.Alpha3 - 48);
                pv.RPC("ChangeWep", RpcTarget.All, ((int)KeyCode.Alpha3 - 48));
            }
        }
    }

    void ChangeWeaponTransform()
    {
        // 에임중 또는 움직이거나 달리기 중이 아닐때
        if ((player.Aim || player.GetMove) && !player.GetShift)
        {
            pos = change_pos;
            rot = change_rot;
        }
        else
        {
            pos = basic_pos;
            rot = basic_rot;
        }
    }

    [PunRPC]
    void ChangeWep(int index)
    {
        if (player.curWpNum != index)   // 소지중인 총 넘버 != 교체할 총 넘버
        {
            // 조준 취소
            aimBehavior.curAim = false;

            // 교체할 총 활성화
            weapon[index-1].SetActive(true);

            // 소지한 총 비활성화
            weapon[player.curWpNum-1].SetActive(false);

            // 플레이SO 인덱스 초기화
            player.playerSo.weaponSO = weapon[index-1].GetComponent<WpScript>().weaponSo; // 0:ak, 1:kar98, 2:m4
        }
    }

    // 실시간 데이터 보내거나 받음..
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(pos);
            stream.SendNext(rot);
        }
        else if (stream.IsReading)
        {
            pos = (Vector3)stream.ReceiveNext();
            rot = (Vector3)stream.ReceiveNext();
        }
    }
}

// 추상화(공통 메소드 추출)
public interface IGunMng
{
    void Shot(BulletPooling bulletPooling, Vector3 dir);
}

