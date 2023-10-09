using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat.Demo;
using Photon.Realtime.Demo;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<Card> CardList, _AllCardList;
    public PhotonView PV;
    public SpriteRenderer SR;
    public Animator AN;
    public TMP_Text NickNameText;
    public Image HealthImage;
    Vector3 mousePos, transPos, targetPos;
    Vector3 curPos;
    float drawTime, updateTime = 0.0f;
    public int deckLength, DL, nNum;
    public List<int> DeckList = new List<int>();
    public List<int> CemList = new List<int>(); 
    public List<bool> HandExistList = new List<bool> ();
    public List<int> HandCardList = new List<int> ();
    public float Damage;
    bool role;

    void Awake()
    {
        // 타겟을 현재 위치로 하여 고정
        targetPos = this.transform.position;

        // 자신의 닉네임은 초록색, 적의 닉네임은 빨간색
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        CardList = GameObject.Find("CardArea").GetComponent<CardManager>().CardDeckList;
        _AllCardList = GameObject.Find("CardArea").GetComponent<CardManager>().AllCardList;
        deckLength = CardList.Count;
        DL = deckLength - 1;
        role = CardList[0].Type == "SA";

        for (int i = 0; i < deckLength; i++) 
        {
            DeckList.Add(CardList[i].CardNum);
            HandExistList.Add(false);
            HandCardList.Add(-1);
        }
        
        Shuffle(DeckList);

        if (role)
            drawTime = 5.0f;
        else
            drawTime = 8.0f; 
    }

    void Update() {
        // 시간에 따라 카드 뽑기
        if(updateTime > drawTime)
        {
            Debug.Log("draw");
            updateTime = 0.0f;

            AddHand(DeckList[DL]);
            DeckList.RemoveAt(DL--);

            if (DL < 0)
                Cemetry();    
        }
        else updateTime += Time.deltaTime;

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
            

            if (Input.GetKeyDown(KeyCode.Q) && HandExistList[0])
            {
                SkillCast(role, HandCardList[0]);
                RemoveHand(0);
            }
            else if (Input.GetKeyDown(KeyCode.W) && HandExistList[1])
            {
                SkillCast(role, HandCardList[1]);
                RemoveHand(1);
            }
            else if (Input.GetKeyDown(KeyCode.E) && HandExistList[2])
            {
                SkillCast(role, HandCardList[2]);
                RemoveHand(2);
            }
            else if (Input.GetKeyDown(KeyCode.R) && HandExistList[3])
            {
                SkillCast(role, HandCardList[3]);
                RemoveHand(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && HandExistList[4])
            {
                SkillCast(role, HandCardList[4]);
                RemoveHand(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && HandExistList[5])
            {
                SkillCast(role, HandCardList[5]);
                RemoveHand(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && HandExistList[6])
            {
                SkillCast(role, HandCardList[6]);
                RemoveHand(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && HandExistList[7])
            {
                SkillCast(role, HandCardList[7]);
                RemoveHand(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5) && HandExistList[8])
            {
                SkillCast(role, HandCardList[8]);
                RemoveHand(8);
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

  
    public void Hit(float damage)
    {
        HealthImage.fillAmount -= damage;
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

    // 카드 셔플 함수
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

    // 손패 리스트에 더하는 함수
    public void AddHand(int cNum) 
    {
        for (int i = 0; i < deckLength; i++) {
            if (cNum == CardList[i].CardNum)
                nNum = i;
        }
        int qNum = CardList[nNum].QuickNum;
        print(qNum);

        if (qNum >= 0) 
        {   // 퀵슬롯 등록
            GameObject.Find("CardArea").GetComponent<CardManager>().OnQuickSlotImage(qNum, cNum);
            HandExistList[qNum] = true;
            HandCardList[qNum] = cNum;
        }
        else
        {   // 숫자슬롯 등록 
            for (int i = 0; i < 4; i++) {
                if (!HandExistList[i+4]) {
                    GameObject.Find("CardArea").GetComponent<CardManager>().OnNumSlotImage(i, cNum);

                    HandExistList[i+4] = true;
                    HandCardList[i+4] = cNum;
                    break;
            }
        }
        }
    }  

    public void RemoveHand(int sNum)
    {
        HandExistList[sNum] = false;
        CemList.Add(HandCardList[sNum]);
        HandCardList[sNum] = -1;
        
        GameObject.Find("CardArea").GetComponent<CardManager>().OffSlotImage(sNum);
    }
    
    public void Cemetry() 
    {
        // 덱이 모두 소모되면 묘지에서 가져온다.\
        int n = CemList.Count;
        for (int i = 0; i < n; i++) {
            DeckList.Add(CemList[i]);
        }
        CemList.Clear();
        Shuffle(DeckList);
        DL = n - 1;
    }


    public void SkillCast(bool isSA, int cNum) 
    {
        if(role)
            switch (cNum) {
            case 0:
                AN.SetTrigger("HorizonLob");
                Damage = _AllCardList[0].Power;
                Lob();
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            }
        else
            switch (cNum) {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            }
    }
    public void Lob()
    {
        PhotonNetwork.Instantiate("Sword", transform.position + new Vector3(SR.flipX ? -0.5f : 0.5f, -0.11f, 0), Quaternion.identity);
   
        targetPos = transform.position;
    }
}

    // -------------------------------------------- 전사 스킬 --------------------------------------------
   
    
        


    // -------------------------------------------- 마법사 스킬 ------------------------------------------




