using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float speed = 0.0001f;
    Vector3 mousePos, transPos, targetPos;
    SpriteRenderer spriter;
    Animator anim;
    private Vector3 m_LastPosition;

    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            CalTargetPos();

            MoveToTarget();
    }

    void CalTargetPos()
    {
        mousePos = Input.mousePosition;
        transPos = Camera.main.ScreenToWorldPoint(mousePos);
        targetPos = new Vector3(transPos.x, transPos.y, 0);
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime + speed);
    }

    void FixedUpdate()
    {
        anim.SetFloat("Speed", GetSpeed());
        // 이동하는 방향으로 뒤집기
        if (transPos.x != 0) {
            spriter.flipX = transPos.x - transform.position.x < 0;
        }
    }

    float GetSpeed()
    {
        float speed = (((transform.position - m_LastPosition).magnitude)/Time.deltaTime);
        m_LastPosition = transform.position;

        return speed;
    }
}
