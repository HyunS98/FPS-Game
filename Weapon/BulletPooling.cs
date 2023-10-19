using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletPooling : MonoBehaviour
{
    [SerializeField]
    GameObject bullet;      // 총알
    Queue<GameObject> objPool = new Queue<GameObject>();    // 오브젝트를 담을 큐

    public static BulletPooling instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        CreateBullet();
    }

    
    void CreateBullet()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject create_bullet = Instantiate(bullet, transform.position, transform.rotation);
            create_bullet.transform.SetParent(transform);
            create_bullet.SetActive(false);
            objPool.Enqueue(create_bullet);
        }
    }

    public GameObject GetObj()
    {
        // 큐에 남아 있는게 있다면
        if (objPool.Count > 0)
        {
            GameObject objInPool = objPool.Dequeue();
            objInPool.SetActive(true);

            return objInPool;
        }
        else
        {
            Debug.Log("그만 쏴!!!!!!!");
            return null;
        }
    }

    // 폴링에 재추가
    public void ReturnQueue(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        
        objPool.Enqueue(obj);
    }
}
