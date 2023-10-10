using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class LightBeam : MonoBehaviour
{
    public PhotonView PV;
    public SpriteRenderer SR;
    int dir;

    void Start()
    {
        SR.flipX = dir > 0;
        Destroy(gameObject, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!PV.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine)
        {
            col.GetComponent<PlayerMove>().Hit();
        }
    }
    [PunRPC]
    void DirRPC(int dir) => this.dir = dir;
}
