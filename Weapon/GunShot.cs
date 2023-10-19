using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunShot : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject firePos;     // 발사 위치값
    [SerializeField]
    GameObject weaponPos;   // 발사해야할 회전값
    [SerializeField]
    GameObject fireEffect;  // 발사 이펙트
    [SerializeField]
    BulletPooling bulletPooling; // 총알 폴링

    PlayerAll player;
    PhotonView pv;
    Camera3rd cam;
    WeaponSO weaponSO;
    Coroutine spreadCoroutine;  

    // ※※※ 미구현 ※※※
    bool shotCoolTime;      // 발사 쿨타임

    private void Start()
    {
        player = GetComponent<PlayerAll>();
        pv = GetComponent<PhotonView>();
        cam = Camera.main.GetComponent<Camera3rd>();

        weaponSO = player.playerSo.weaponSO;
    }

    void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        // 추후 'Down' 삭제
        if (Input.GetMouseButtonDown(0) && player.Aim && weaponSO.curBullet > 0)
        {
            // 탄퍼짐을 위한 범위증가
            Quaternion spread = Quaternion.Euler(Random.Range(-weaponSO.spread, weaponSO.spread), Random.Range(-weaponSO.spread, weaponSO.spread), 0);

            // 탄퍼짐 증가 (최대치 3)
            if(weaponSO.spread < 3f)
            {
                weaponSO.spread += weaponSO.upSpread;
                if(weaponSO.spread >= 3f)
                {
                    weaponSO.spread = 3f;
                }
            }

            // 총알 감소
            weaponSO.curBullet--;

            // 총알이 날아가는 방향
            Vector3 fireDir = spread * -firePos.transform.forward;

            // 총알 발사
            pv.RPC("FireBullet", RpcTarget.All, fireDir, spread, weaponSO.bulletSpeed);

            // 일정시간 동안 공격하지 않으면 탄 퍼짐 범위 감소(마지막 공격을 기점으로 실행)
            if(spreadCoroutine != null)
            {
                StopCoroutine(spreadCoroutine);
            }
            spreadCoroutine = StartCoroutine(DownSpread());

            // 탄 퍼짐으로 크로스헤어 커짐
            CrosshairUI.instance.UpPoint();

            // 총 반동
            cam.Rebound(weaponSO.reboundMinY, weaponSO.reboundMaxY, weaponSO.reboundX);
        }
    }

    // 총 발사
    [PunRPC]
    void FireBullet(Vector3 fireDir, Quaternion spread, float bulletSpeed) // 매개변수로 GameObject는 가져올 수 없음
    {
        // 총구 화염 이펙트
        StartCoroutine(FireEffectCor());

        // 풀링 이후 오브젝트 가져옴 
        GameObject bullet = bulletPooling.GetObj();

        // 총알 생성위치 및 회전값 조정
        bullet.transform.position = firePos.transform.position;
        bullet.transform.rotation = firePos.transform.rotation * spread;

        // 설정한 방향대로 힘을 주어 날아감
        bullet.GetComponent<Rigidbody>().AddForce(fireDir * bulletSpeed, ForceMode.Impulse);  // 순간적인 힘으로 정면을 향해 날라감

        // 발사 방향 및 속도 interface로 재선언
        bullet.GetComponent<BulletDamage>().Shot(bulletPooling, fireDir);
    }

    // 화염 이펙트
    IEnumerator FireEffectCor()
    {
        GameObject effect = Instantiate(fireEffect, firePos.transform);
        yield return new WaitForSeconds(3f);
        Destroy(effect);
    }

    // 탄 퍼짐 수치 범위 줄이기
    IEnumerator DownSpread()
    {
        yield return new WaitForSeconds(2f);
        // 마지막 탄환 이후 2초 뒤 탄퍼짐 수치 감소
        while(weaponSO.spread > 0.1f)
        {
            weaponSO.spread = Mathf.Lerp(weaponSO.spread, 0, 2.5f * Time.deltaTime);
            yield return null;
        }
        weaponSO.spread = 0;
    }

}
