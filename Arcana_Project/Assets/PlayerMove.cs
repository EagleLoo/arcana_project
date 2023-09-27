using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        Move();
    }

    void Move() {
        Vector3 movePosition = Vector3.zero;

        if(Input.GetAxisRaw("Horizontal") < 0) {
            movePosition = Vector3.left;
        }
        else if(Input.GetAxisRaw("Horizontal") > 0) {
            movePosition = Vector3.right;
        }

        transform.position += movePosition * moveSpeed * Time.deltaTime;
    }
}
