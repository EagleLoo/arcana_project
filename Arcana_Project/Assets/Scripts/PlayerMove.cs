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
    public Rigidbody2D RB;
    public Animator AN;
    public TMP_Text NickNameText;
    public Image HealthImage;
    Vector3 mousePos, transPos, targetPos, curPos;
    float drawTime, updateTime = 0.0f, damage, extraDamage, enemyDamage;
    public int deckLength, DL, nNum, speed;
    public List<int> DeckList = new List<int>();
    public List<int> CemList = new List<int>(); 
    public List<bool> HandExistList = new List<bool> ();
    public List<int> HandCardList = new List<int> ();
    bool role, block, enemyBlock, paint = true;
    int MeteorCountDown = 0;
    void Awake()
    {
        // 타겟을 현재 위치로 하여 고정
        targetPos = this.transform.position;
        speed = 3;

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
        }
        for (int i = 0; i < 8; i++)
        {
            HandExistList.Add(false);
            HandCardList.Add(-1);
        }
        
        Shuffle(DeckList);

        if (role)
            drawTime = 4.0f;
        else
            drawTime = 6.0f; 
    }

    void Update() {
        // 시간에 따라 카드 뽑기
        if(updateTime > drawTime)
        {
            updateTime = 0.0f;

            
            AddHand(DeckList[DL]);
            DeckList.RemoveAt(DL--);

            if (DL < 0) {
                Shuffle(DeckList);
                Cemetry();
            }    
        }
        else updateTime += Time.deltaTime;

    }
    void LateUpdate()
    {
        // 마우스 클릭했을 때 나의 캐릭터만 CalTargetPos() 실행
        if (PV.IsMine)
        {
            if (paint) {
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
                    SkillCast(HandCardList[0]);
                    RemoveHand(0);
                }
                else if (Input.GetKeyDown(KeyCode.W) && HandExistList[1])
                {
                    SkillCast(HandCardList[1]);
                    RemoveHand(1);
                }
                else if (Input.GetKeyDown(KeyCode.E) && HandExistList[2])
                {
                    SkillCast(HandCardList[2]);
                    RemoveHand(2);
                }
                else if (Input.GetKeyDown(KeyCode.R) && HandExistList[3])
                {
                    SkillCast(HandCardList[3]);
                    RemoveHand(3);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1) && HandExistList[4])
                {
                    SkillCast(HandCardList[4]);
                    RemoveHand(4);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && HandExistList[5])
                {
                    SkillCast(HandCardList[5]);
                    RemoveHand(5);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3) && HandExistList[6])
                {
                    SkillCast(HandCardList[6]);
                    RemoveHand(6);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4) && HandExistList[7])
                {
                    SkillCast(HandCardList[7]);
                    RemoveHand(7);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5) && HandExistList[8])
                {
                    SkillCast( HandCardList[8]);
                    RemoveHand(8);
                }
            
                MoveToTarget();
            }
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
        HealthImage.fillAmount -= damage * 0.02f;
        if (HealthImage.fillAmount <= 0)
        {
            // 상대에게 승리화면 출력
            PV.RPC("Referee", RpcTarget.Others);  
            GameObject.Find("Canvas").transform.Find("LosePanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }

        if (enemyBlock) {
            paint = false;
            Invoke ("WakeUp", 1f);
        }
    }

    public void IceHit()
    {
        HealthImage.fillAmount -= 0.2f;
        if (HealthImage.fillAmount <= 0)
        {
            // 상대에게 승리화면 출력
            PV.RPC("Referee", RpcTarget.Others);  
            GameObject.Find("Canvas").transform.Find("LosePanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        speed = 1;
        Invoke ("Slow", 1f);
    }

    public void Swamp()
    {
        speed = 1;
    }

    public void WakeUp() {
        paint = true;
        enemyBlock = false;
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
            stream.SendNext(damage + extraDamage);
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
        }
        else
        {
            damage = (float)stream.ReceiveNext();
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
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
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
        // 
        for (int i = 0; i < deckLength; i++) {
            if (cNum == CardList[i].CardNum)
                nNum = i;
        }
        int qNum = CardList[nNum].QuickNum;
        
        if (qNum >= 0) 
        {   // 퀵슬롯 등록

            HandExistList[qNum] = true;
            HandCardList[qNum] = cNum;
            if (cNum > 8) cNum+=MeteorCountDown;
            GameObject.Find("CardArea").GetComponent<CardManager>().OnQuickSlotImage(qNum, cNum);
        }
        else
        {   // 숫자슬롯 등록 
            for (int i = 0; i < 4; i++) {
                if (!HandExistList[i+4]) {
                    HandExistList[i+4] = true;
                    HandCardList[i+4] = cNum;
                    if (cNum > 8) cNum+=MeteorCountDown;
                    GameObject.Find("CardArea").GetComponent<CardManager>().OnNumSlotImage(i, cNum);
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
        // 덱이 모두 소모되면 묘지에서 가져온다.
        int n = CemList.Count;
        for (int i = 0; i < n; i++) {
            DeckList.Add(CemList[i]);
        }
        CemList.Clear();
        Shuffle(DeckList);
        DL = n - 1;
    }


    public void SkillCast(int cNum) 
    {
        extraDamage = 0f;
        block = false;
        
        if(role)
            switch (cNum) {
            case 0: // 가로베기
                damage = 10f;
                AN.SetTrigger("HorizonLob");
                Attack();
                break;
            case 1: // 질주
                speed = 6;
                Invoke("Slow", 0.5f);
                break;
            case 2: // 근력 강화
                extraDamage = 7f;
                break;
            case 3: // 지진
                damage = 5f;
                block = true;
                AN.SetTrigger("Earthquake");
                // earthquake();
                break;
            case 4: // 강타
                damage = 7f;
                block = true;
                AN.SetTrigger("HorizonLob");
                Attack();
                break;
            case 5: // 흡혈
                damage = 7f;
                Attack();
                HealthImage.fillAmount += 0.05f;
                break;
            case 6: // 돌진
                Dash();
                break;
            case 7: // 세로베기
                damage = 20f;
                AN.SetTrigger("VerticalLob");
                Attack();
                break;
            case 8: // 방어      
                gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
                Invoke("Defense", 0.1f);
                break;
            case 9: // 벼락
                damage = 20f;
                AN.SetTrigger("Lightning");
                Attack();
                RemoveHandAll(cNum);
                break;
            }
        else
            switch (cNum) {
            case 0: // 서리화살
                AN.SetTrigger("Shoot");
                Ice();
                break;
            case 1: // 바위벽
                AN.SetTrigger("Summon");
                PhotonNetwork.Instantiate("Rock", targetPos, Quaternion.identity);
                break;
            case 2: // 파이어볼
                damage = 15f;
                AN.SetTrigger("Shoot");
                Fire();
                break;  
            case 3: // 주문강화
                extraDamage = 7f;
                break;
            case 4: // 블링크
                AN.SetTrigger("Blink");
                transform.position = targetPos;
                break;
            case 5: // 스웜프
                AN.SetTrigger("Summon");
                PhotonNetwork.Instantiate("Swamp", targetPos, Quaternion.identity);
                break;
            case 6: // 스파크
                AN.SetTrigger("Shoot");
                damage = 5f;
                block = true;
                Attack();
                break;
            case 7: // 사고가속
                drawTime = 6.0f;
                Invoke("TimeAccel", 3f);
                break; 
            case 8: // 섬광
                AN.SetTrigger("Shoot");
                damage = 20f;
                Light();
                break;
            case 9: // 메테오 
                AN.SetTrigger("Casting");
                MeteorCasting();
                break;
            }
    }
    public void Attack()
    {
        PhotonNetwork.Instantiate("Sword", transform.position + new Vector3(SR.flipX ? -0.5f : 0.5f, -0.11f, 0), Quaternion.identity);
    }

    public void Slow()
    {
        speed = 3;
    }

    public void Dash()
    {
        CalTargetPos();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 200);
    }

    public void Defense()
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
    }

    public void TimeAccel()
    {
        drawTime = 9.0f;
    }

    public void Ice()
    {
        PhotonNetwork.Instantiate("Ice", transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
    }

    public void Fire()
    {
        PhotonNetwork.Instantiate("Fire", transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
    }

    public void Light()
    {
        PhotonNetwork.Instantiate("Light", transform.position + new Vector3(SR.flipX ? -4.5f : 4.5f, -0.11f, 0), Quaternion.identity).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
    }

    public void MeteorCasting() 
    {
        MeteorCountDown++;

        if (MeteorCountDown == 4)
            PV.RPC("Meteor", RpcTarget.Others, 0.5f);
    }

    [PunRPC]
    public void Meteor(float dam) {
        HealthImage.fillAmount -= dam;
    }

    public void RemoveHandAll(int cNum)
    {
        for (int i = 0; i < 8; i++)
            if(HandExistList[i] = true && HandCardList[i] != cNum) {
                HandExistList[i] = false;
                CemList.Add(HandCardList[i]);
                HandCardList[i] = -1;
        
                GameObject.Find("CardArea").GetComponent<CardManager>().OffSlotImage(i);
            }
    }

}

    // -------------------------------------------- 전사 스킬 --------------------------------------------
   
    
        


    // -------------------------------------------- 마법사 스킬 ------------------------------------------




