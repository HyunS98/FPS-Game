using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "SO/PlayerSO", order = 1)]
public class PlayerSO : ScriptableObject
{
    // 체력 관련
    public float hp;
    public float moveSpeed;
    public float jumpPower;

    // 총 SO
    public WeaponSO weaponSO;

    public PlayerSO()
    {
        this.hp = 100;
        this.moveSpeed = 2.0f;
        this.jumpPower = 3.3f;
    }

    public virtual PlayerSO Reset()
    {
        PlayerSO playerSO = CreateInstance<PlayerSO>();

        playerSO.hp = 100;
        playerSO.moveSpeed = 2.0f;
        playerSO.jumpPower = 3.3f;

        return playerSO;
    }
}

/*
abstract(부모) - override(자식) : 자식에서 따로 정의해야함
Virtual (부모) - override(자식) : 자식에서 따로 정의 안해도 됨
 */ 