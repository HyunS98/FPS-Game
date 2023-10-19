using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public List<Transform> spawn;

    
    void Start()
    {
        RandomIndex();
    }

    
    void Update()
    {
        
    }

    void RandomIndex()
    {
        int ran = Random.Range(0, spawn.Count);
        PhotonNetwork.Instantiate("Player", spawn[ran].position, spawn[ran].rotation);
        
        spawn.RemoveAt(ran);
    }
}
