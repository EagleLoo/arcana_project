using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat.Demo;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    
    public PhotonView PV;
    public SpriteRenderer SR;
    public Animator AN;
    public TMP_Text NickNameText;
    public Image HealthImage;
    Vector3 mousePos, transPos, targetPos;

    void Awake()
    {
        // 타겟을 현재 위치로 하여 고정
        targetPos = this.transform.position;

        // 자신의 닉네임은 초록색, 적의 닉네임은 빨간색
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
    }

    void LateUpdate()
    {
        // 마우스 클릭했을 때 나의 캐릭터만 CalTargetPos() 실행
        if (PV.IsMine)
        {
            if(Input.GetMouseButton(1))
            CalTargetPos();
            Vector2 dis = targetPos - transform.position;
            
            if (dis.magnitude != 0)
            {
                AN.SetBool("walk", true);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, dis.x);
            }
            else AN.SetBool("walk", false);
        }

        MoveToTarget();
    }

    [PunRPC]
    void FlipXRPC(float x)
    {
     SR.flipX = x < 0;
    }
    // 마우스가 클릭한 방향으로 이동
    void CalTargetPos()
    {
        mousePos = Input.mousePosition;
        transPos = Camera.main.ScreenToWorldPoint(mousePos);
        targetPos = new Vector3(transPos.x, transPos.y, 0);
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 4);
    }  
}
