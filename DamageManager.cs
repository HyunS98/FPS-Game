using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DamageManager : MonoBehaviour
{
    public static DamageManager instance;   // 싱글톤

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 데미지 부여 함수(추후 수류탄의 경우에서도 사용 가능)
    public void DamageCalculate(float objDMG, float bodyDEF, GameObject player)
    {
        float dmg = (objDMG * bodyDEF) / 100;

        player.GetComponent<PlayerAll>().playerSo.hp -= dmg;
    }
}

