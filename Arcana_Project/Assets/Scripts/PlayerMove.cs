using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat.Demo;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<Card> CardList;

    public PhotonView PV;
    public SpriteRenderer SR;
    public Animator AN;
    public TMP_Text NickNameText;
    public Image HealthImage;
    Vector3 mousePos, transPos, targetPos;
    Vector3 curPos;
    float DrawTime, UpdateTime = 0.0f;
    public int DeckLength;
    public List<int> DeckList = new List<int>();
    public List<int> HandList = new List<int>(); 
    public List<int> CemList = new List<int>(); 

    void Awake()
    {
        // 타겟을 현재 위치로 하여 고정
        targetPos = this.transform.position;

        // 자신의 닉네임은 초록색, 적의 닉네임은 빨간색
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        CardList = GameObject.Find("CardArea").GetComponent<CardManager>().CardDeckList;
        DeckLength = GameObject.Find("CardArea").GetComponent<CardManager>().CardCnt;

        for (int i = 0; i < DeckLength; i++) 
        {
            DeckList.Add(CardList[i].CardNum);
        }
        
        Shuffle(DeckList);

        if (CardList[0].Type == "SA")
            DrawTime = 2.0f;
        else
            DrawTime = 4.0f; 
    }

    void Update() {
        if(UpdateTime > DrawTime)
        {
            UpdateTime = 0.0f;

        }
        else UpdateTime += Time.deltaTime;

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
            

            if (Input.GetKeyDown(KeyCode.Q))
            {
                PhotonNetwork.Instantiate("Sword", transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity);
                
                AN.SetTrigger("HorizonLob");
                targetPos = transform.position;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {

            }
            else if (Input.GetKeyDown(KeyCode.E))
            {

            }
            else if (Input.GetKeyDown(KeyCode.R))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {

            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {

            }
            
            MoveToTarget();
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    [PunRPC]
    void FlipXRPC(float x)
    {
     SR.flipX = x < 0;
    }

    public void Hit()
    {
        HealthImage.fillAmount -= 0.1f;
        if (HealthImage.fillAmount <= 0)
        {
            // 상대에게 승리화면 출력
            PV.RPC("Referee", RpcTarget.Others);  
            GameObject.Find("Canvas").transform.Find("LosePanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    // 승리 화면
    void Referee() 
    {
            GameObject.Find("Canvas").transform.Find("WinnerPanel").gameObject.SetActive(true);          
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();
        }
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

    public List<T> Shuffle<T>(List<T> _list) {
        for (int i = _list.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i);

            T temp = _list[i];
            _list[i] = _list[rnd];
            _list[rnd] = temp;
        }

        return _list;
    }
    // -------------------------------------------- 전사 스킬 --------------------------------------------
   
    



    // -------------------------------------------- 마법사 스킬 ------------------------------------------



}
