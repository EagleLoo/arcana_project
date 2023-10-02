using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float speed;
    public PhotonView PV;
    public TMP_Text NickNameText;
    public Image HealthImage;

    Vector3 mousePos, transPos, targetPos;
    Vector3 m_LastPosition;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        targetPos = this.transform.position;

        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
    }

    void Update()
    {
        if (PV.IsMine && Input.GetMouseButton(1))
        {
            CalTargetPos();
        }

        MoveToTarget();
    }

    void FixedUpdate()
    {
        anim.SetFloat("Speed", GetSpeed());
        // 이동하는 방향으로 뒤집기
        if (transPos.x != 0)
        {
            spriter.flipX = transPos.x - transform.position.x < 0;
        }
    }

    void CalTargetPos()
    {
        mousePos = Input.mousePosition;
        transPos = Camera.main.ScreenToWorldPoint(mousePos);
        targetPos = new Vector3(transPos.x, transPos.y, 0);
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
    }

    float GetSpeed()
    {
        float speed = (((transform.position - m_LastPosition).magnitude)/Time.deltaTime);
        m_LastPosition = transform.position;

        return speed;
    }
}
