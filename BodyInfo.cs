using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyInfo : MonoBehaviour
{
    [SerializeField]
    public float def;   // 부위별 방어력
    [SerializeField]
    public GameObject player;

    void OnTriggerEnter(Collider other)
    {
        // 총알과 충돌시 데미지 부여 함수로 값 전달
        if (other.gameObject.layer == 9)
        {
            DamageManager.instance.DamageCalculate(other.GetComponent<BulletDamage>().bulletDMG, def, player);
            other.GetComponent<BulletDamage>().BulletReturn();

            Debug.Log("맞은 위치 " + gameObject.name);
        }
    }
}
