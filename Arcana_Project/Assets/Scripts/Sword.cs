using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Sword : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    void Start()
    {
        Destroy(gameObject, 0.4f);
    }

   
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!PV.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine)
        {
            col.GetComponent<PlayerMove>().Hit();
        }
    }
}
