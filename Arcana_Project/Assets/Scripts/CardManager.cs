using System.Collections;
using System.Collections.Generic;
using Photon.Chat.Demo;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
    public Card(string _Type, int _CardNum, string _Name, float _Power, float _Range, string _Description, bool _isUsing)
    {Type = _Type; CardNum = _CardNum; Name = _Name; Power = _Power; Range = _Range; Description = _Description; isUsing = _isUsing;}
    public string Type;
    public int CardNum;
    public string Name;
    public float Power, Range;
    public string Description;   
    public bool isUsing;
}
public class CardManager : MonoBehaviour
{
    public Toggle WarToggle;
    public TextAsset CardDatabase;
    public List<Card> AllCardList, MyCardList, StartCardList; 
    public Image Expand;
    public Image[] Slot;
    public Sprite[] SACard;
    public Sprite[] HQCard;
    public int[] SlotNum;
    
    void Start()
    {
       string[] line = CardDatabase.text.Substring(0, CardDatabase.text.Length - 1).Split('\n');
       for (int i = 0; i < line.Length; i++)
       {
            string[] row = line[i].Split('\t');
            AllCardList.Add(new Card(row[0], int.Parse(row[1]), row[2], float.Parse(row[3]), float.Parse(row[4]), row[5], row[6] == "TRUE"));
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
    }

    public void StartDeck() {
        StartCardList = MyCardList.FindAll(x => x.isUsing == true);
    }

    public void Shuffle()
    {

    }
}
