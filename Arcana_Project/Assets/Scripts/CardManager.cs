using System.Collections;
using System.Collections.Generic;
using Photon.Chat.Demo;
using Photon.Chat.UtilityScripts;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
    public Card(string _Type, int _CardNum, string _Name, float _Power, float _Range, string _Description, bool _isUsing, int _QuickNum)
    {Type = _Type; CardNum = _CardNum; Name = _Name; Power = _Power; Range = _Range; Description = _Description; isUsing = _isUsing; QuickNum = _QuickNum;}
    public string Type;
    public int CardNum;
    public string Name;
    public float Power, Range;
    public string Description;   
    public bool isUsing;
    public int QuickNum;
}

public class CardManager : MonoBehaviour
{
    public Toggle WarToggle;
    public TextAsset CardDatabase;
    public TextAsset HandDatabase;
    public List<Card> AllCardList, MyCardList, CardDeckList; 
    
    public Image Expand;
    public Image[] Slot;
    public Image[] QuickSlot;
    public Image[] CardSlot;
    public Toggle[] UsingTog;
    public Sprite Blank;
    public Sprite[] SACard;
    public Sprite[] HQCard;
    public Sprite[] QuickSlotStone;
    int[] QuickSlotNum = {-1, -1, -1, -1};
    public int CurNum;
    int QNum, nNum;


    void Start()
    {
        string[] line = CardDatabase.text.Substring(0, CardDatabase.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
             string[] row = line[i].Split('\t');
             AllCardList.Add(new Card(row[0], int.Parse(row[1]), row[2], float.Parse(row[3]), float.Parse(row[4]), row[5], row[6] == "TRUE", int.Parse(row[7])));
        }
    }

    void FixedUpdate() {
        
        if (Input.GetKey(KeyCode.Q))
        {
            QNum = 0;
            QuickResist(QNum);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            QNum = 1;
            QuickResist(QNum);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            QNum = 2;
            QuickResist(QNum);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            QNum = 3;
            QuickResist(QNum);
        }    
    }

    // 내가 선택한 캐릭터의 카드만 불러오기
    public void MyDeck()
    {
        string tog;
        if (WarToggle.isOn)
            tog = "SA";
        else
            tog = "HQ";
        MyCardList = AllCardList.FindAll(x => x.Type == tog);

        for (int i = 0; i < 10; i++) {
            
            if (tog == "SA")
                Slot[i].sprite = SACard[i];
            else
                Slot[i].sprite = HQCard[i]; 
        }
    }

    // 카드 설명 확장
    public void PointerEnter(int slotNum)
    {
        Expand.GetComponent<Image>().sprite = Slot[slotNum].GetComponent<Image>().sprite;
        Expand.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = MyCardList[slotNum].Name;
        Expand.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = MyCardList[slotNum].Description;
        CurNum = slotNum;
    }

    // 퀵슬롯에 카드 등록
    public void QuickResist(int Qnum)
    {
        for (int i = 0; i < 4; i++) {
            if (QuickSlotNum[i] == CurNum) {
                QuickSlotNum[i] = -1; 
                QuickSlot[i].GetComponent<Image>().sprite = QuickSlotStone[i];
            }
        }
        QuickSlot[Qnum].GetComponent<Image>().sprite = Slot[CurNum].GetComponent<Image>().sprite;
        QuickSlotNum[Qnum] = CurNum;
    }

    // 활성화 된 카드들을 CardDeckList에 넣음
    public void StartDeck() {
        for (int i = 0; i < 10; i++)
            if (UsingTog[i].isOn) {
                MyCardList[i].isUsing = true;
            }

        for (int i = 0; i < 4; i++)
            if (QuickSlotNum[i] != -1) {
                MyCardList[QuickSlotNum[i]].QuickNum = i;
            }
        CardDeckList = MyCardList.FindAll(x => x.isUsing == true);

    }

    public void OnQuickSlotImage(int qNum, int cNum) 
    {
        CardSlot[qNum].GetComponent<Image>().sprite = WarToggle.isOn ? SACard[cNum] : HQCard[cNum];
    }

    public void OnNumSlotImage(int n, int cNum) 
    {
        CardSlot[n + 4].GetComponent<Image>().sprite = WarToggle.isOn ? SACard[cNum] : HQCard[cNum];
    }

    public void OffSlotImage(int sNum)
    {
        CardSlot[sNum].GetComponent<Image>().sprite = Blank;
    }
}
