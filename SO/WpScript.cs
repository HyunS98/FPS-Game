using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WpScript : MonoBehaviour
{
    public WeaponSO weaponSo;

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        weaponSo.curBullet = weaponSo.maxBullet;
        weaponSo.spread = 0;
    }
}
