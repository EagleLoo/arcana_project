using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Swamp : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    void Start()
    {
        Destroy(gameObject, 10f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!PV.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine)
        {
            col.GetComponent<PlayerMove>().Swamp();
        }
    }

    void OnTriggerExit2D(Collider2D col2)
    {
        if (!PV.IsMine && col2.tag == "Player" && col2.GetComponent<PhotonView>().IsMine)
        {
            col2.GetComponent<PlayerMove>().Slow();
        }
    }
}
