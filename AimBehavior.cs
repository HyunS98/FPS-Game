using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBehavior : MostPlayer
{
    public GameObject zoomStandObj;     // 줌 상태 기준 오브젝트
    //public WeaponSet wea;
    Camera3rd cam3rd;

    public bool curAim = false;
    PhotonView pv;


    void Start()
    {
        pv = GetComponent<PhotonView>();
        cam3rd = Camera.main.GetComponent<Camera3rd>();
    }

    
    void Update()
    {
        // isMine 제외시 자신은 안움직이고 다른사람이 움직임
        if (pv.IsMine && playerScript.playerSo.hp > 0)
        {
            ZoomException();
            Zoom();
        }
    }

    void Zoom()
    {
        // 줌 상태 일떄
        if(curAim )
        {
            cam3rd.SetCamObj(zoomStandObj);
            cam3rd.SetCamPivot(new Vector3(0f, 0f, 0f));              // standObj에서 카메라 이동
            cam3rd.SetCamPos(-0f);                                    // 카메라와 standObj의 거리

            playerScript.Aim = true;
        }
        else if(!curAim)
        {
            cam3rd.SetCamObj(gameObject);
            cam3rd.SetResetCam();

            playerScript.Aim = false;
        }
    }

    // 조준이 안돼거나 풀리는 경우
    void ZoomException()
    {
        // 달릴 경우
        if(playerScript.GetShift)
        {
            curAim = false;
        }

        // 점프 경우
        if(playerScript.GetJump)
        {
            curAim = false;
        }

        if (Input.GetMouseButtonDown(1) && !playerScript.Aim && !playerScript.GetShift)
        {
            curAim = true;
        }
        else if (Input.GetMouseButtonDown(1) && playerScript.Aim)
        {
            curAim = false;
        }
    }

}
