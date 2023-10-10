using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Ice : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public SpriteRenderer SR;
    int dir;

    void Start()
    {
        SR.flipX = dir < 0;
        Destroy(gameObject, 3.5f);
    }

    void Update() => transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!PV.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine)
        {
            col.GetComponent<PlayerMove>().IceHit();
        }
    }
    [PunRPC]
    void DirRPC(int dir) => this.dir = dir;
}