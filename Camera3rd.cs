using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Camera3rd : MonoBehaviour
{
    [Range(50f, 150f)]
    public float rotateSpeed;  // 마우스 회전 속도

    // 카메라 회전 
    float xRotateMove, yRotateMove;
    Vector3 targetRot;         // 실시간 회전 각도
    Vector3 currentRot;        // 회전 속도

    // 카메라 시점 관련 
    Vector3 camPivot = new Vector3(0, 1.5f, 0); // 카메라 피봇
    float camPos = 3f;         // 카메라 위치
    GameObject obj;            // 카메라 기준 오브젝트

    Vector3 reCamPivot;        // 초기 카메라 피봇 저장 변수
    float reCamPos;            // 초기 카메라 위치 저장 변수

    //// 반동 랜덤 최대값, 최소값
    //float maxY = 1.1f, minY = 0.9f;   // 상하
    //float maxX = 0.2f;  // 좌우

    void Awake()
    {
        reCamPivot = camPivot;
        reCamPos = camPos;
    }


    void Update()
    {
        yRotateMove = yRotateMove - Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed;  // 위 아래
        xRotateMove = xRotateMove + Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;  // 좌 우

        //Quaternion a = Quaternion.Euler(yRotateMove, xRotateMove, 0);

        // 회전 각도 조정
        yRotateMove = Mathf.Clamp(yRotateMove, -40, 40);    // (위, 아래)

        // SmoothDamp이용 카메라 부드러운 회전
        targetRot = Vector3.SmoothDamp(targetRot, new Vector3(yRotateMove, xRotateMove), ref currentRot, 0.1f);
        transform.eulerAngles = targetRot;

        // 카메라와 플레이어의 거리
        CamMove();
    }

    // 반동
    public void Rebound(float minY, float maxY, float maxX)
    {
        if(yRotateMove < 40 && yRotateMove > -40)
        {
            yRotateMove -= Random.Range(minY, maxY);
            xRotateMove += Random.Range(-maxX, maxX);
        }
    }

    void CamMove()
    {
        if(obj != null)
        {
            transform.position = obj.transform.position + camPivot - transform.forward * camPos;
        }
    }

    // 카메라 시점 기준 오브젝트
    public void SetCamObj(GameObject obj)
    {
        this.obj = obj;
    }

    // 카메라 시점 위치에서의 이동 위치
    public void SetCamPos(float dist)
    {
        this.camPos = dist;
    }

    // 카메라 시점 위치에서 떨어진 거리
    public void SetCamPivot(Vector3 pivot)
    {
        this.camPivot = pivot;
    }

    // 초기 시점 상태로 리셋
    public void SetResetCam()
    {
        camPivot = reCamPivot;
        camPos = reCamPos;
    }
}
