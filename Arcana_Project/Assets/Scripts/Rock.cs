using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Rock : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    void Start()
    {
        Destroy(gameObject, 15f);
    }
}