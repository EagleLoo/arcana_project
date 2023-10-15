using System.Collections;
using System.Collections.Generic;
using Photon.Chat.Demo;
using Photon.Chat.UtilityScripts;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class Serialization<T>
{
    public Serialization(List<T> _Target) => _Target = _Target;
    public List<T> target;
}

[System.Serializable]
public class Card
{
    public Card(string _Type, int _CardNum, string _Name, float _Power, string _Description, bool _isUsing, int _QuickNum)
    {Type = _Type; CardNum = _CardNum; Name = _Name; Power = _Power; Description = _Description; isUsing = _isUsing; QuickNum = _QuickNum;}
    public string Type;
    public int CardNum;
    public string Name;
    public float Power;
    public string Description;   
    public bool isUsing;
    public int QuickNum;
}

public class CardManager : MonoBehaviour
{
    public Toggle WarToggle;
    public TextAsset CardDatabase;
    public List<Card> AllCardList, MyCardList, CurCardList, CardDeckList, QuickSlotList; 
    public Image Expand;
    public Image[] Slot, QuickSlot, CardSlot;
    public Toggle[] UsingTog;
    public Sprite Blank;
    public Sprite[] SACard, HQCard, QuickSlotStone;
    int[] QuickSlotNum = {-1, -1, -1, -1};
    public int CurNum;
    int nNum;
    string filePath, curType;
    
    void Start()
    {
        string[] line = CardDatabase.text.Substring(0, CardDatabase.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
             string[] row = line[i].Split('\t');
             AllCardList.Add(new Card(row[0], int.Parse(row[1]), row[2], float.Parse(row[3]), row[4], row[5] == "TRUE", int.Parse(row[6])));
        }

        filePath = Application.persistentDataPath + "/MyCardText.txt";
        print(filePath);
        Load();
    }

    void FixedUpdate() {
        
        if (Input.GetKey(KeyCode.Q))
        {
            QuickResist(0);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            QuickResist(1);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            QuickResist(2);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            QuickResist(3);
        }    
    }

    public void TabClick()
    {
        for (int i = 0; i < 10; i++)
        ;
    }

    public void ResetCardClick()
    {
        Card BasicCard = AllCardList.Find(x => x.CardNum == 0);
        BasicCard.isUsing = true;
        MyCardList = new List<Card>() {BasicCard};
        Save();
        Load();
    }

    void Save()
    {
        MyCardList.Add(AllCardList[0]);
        MyCardList.Add(AllCardList[1]);
        string jdata = JsonUtility.ToJson(new Serialization<Card>(MyCardList));
        File.WriteAllText(filePath, jdata);

        TabClick();
    }

    void Load()
    {
        if (!File.Exists(filePath)) { ResetCardClick(); return; }
        string jdata = File.ReadAllText(filePath);
        MyCardList = JsonUtility.FromJson<Serialization<Card>>(jdata).target;

        TabClick();
    }

    // 내가 선택한 캐릭터의 카드만 불러오기
    public void MyDeck()
    {
        string tabName;
        // 현재 아이템 리스트에 클릭한 타입만 추가
        if (WarToggle) 
        {
            tabName = "SA";
            for (int i = 0; i < 10; i++)
                Slot[i].sprite = SACard[i];
        }
        else
        {
            tabName = "HQ";
            for (int i = 0; i < 10; i++)
                Slot[i].sprite = HQCard[i];
        }

        MyCardList = AllCardList.FindAll(x => x.Type == tabName);
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
