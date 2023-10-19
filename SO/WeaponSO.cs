using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "SO/WeaponSO", order = 2)]
public class WeaponSO : ScriptableObject
{
    public int weaponNum;        // 총 넘버
    public float bulletSpeed;    // 총알 속도

    // 반동
    public float reboundX;       // 좌우 반동세기
    public float reboundMaxY;    // 최대 상하 반동세기
    public float reboundMinY;    // 최소 상하 반동세기

    // 탄퍼짐
    public float spread;         // 탄퍼짐
    public float maxSpread;      // 최대 탄퍼짐
    public float upSpread;       // 탄퍼짐 증가 수치

    // 총알 수
    public int maxBullet;        // 최대 총알수
    public int curBullet;        // 남은 총알수 
}
