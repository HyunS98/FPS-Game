using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletDamage : MonoBehaviour, IGunMng
{
    [SerializeField]
    GameObject decalImage;  // 파편이미지

    RaycastHit hit;     // 충돌체
    Quaternion rot;     // 파편 회전값
    Rigidbody rigid;
    public float bulletDMG; // 총알 대미지
    BulletPooling bulletPooling;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // 날라가는 방향
    public void Shot(BulletPooling pooling, Vector3 dir)
    {
        bulletPooling = pooling;

        // 총알 생성 후 삭제까지의 시간
        StartCoroutine(ReturnQueue(3f));

        // 탄흔 생성 위치, 회전값 받기
        if(Physics.Raycast(transform.position, dir, out hit, 100))
        {
            rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
        }
    }

    // 일정시간 또는 충돌 후 코루틴 실행
    IEnumerator ReturnQueue(float timer)
    {
        yield return new WaitForSeconds(timer);
        BulletReturn();
    }

    private void OnTriggerEnter(Collider other)
    {
        BulletReturn();
        PhotonNetwork.Instantiate("BulletHole", hit.point, rot);

        // 플레이어들을 제외한 모든 벽
        //if (other.gameObject.layer == "Wall")
        //{
        //    PhotonNetwork.Instantiate("BulletHole", hit.point, rot);
        //}
    }

    // 총알 반환
    public void BulletReturn()
    {
        rigid.velocity = Vector3.zero;  // 물리적인 힘을 초기화해야 재사용시 바라보는 방향으로 날라감
        bulletPooling.ReturnQueue(gameObject);  // 사용한 총알 반환
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.forward * 100, Color.red);
    }
}
