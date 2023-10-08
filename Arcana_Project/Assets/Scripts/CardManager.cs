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
    public Card(string _Type, string _CardNum, string _Name, string _Power, string _Range, string _Description, bool _isUsing)
    {Type = _Type; CardNum = _CardNum; Name = _Name; Power = _Power; Range = _Range; Description = _Description; isUsing = _isUsing;}
    public string Type, CardNum, Name, Power, Range, Description;   
    public bool isUsing;
}
public class CardManager : MonoBehaviour
{
    public Toggle WarToggle;
    public TextMeshProUGUI TextCardName;
    public TextMeshProUGUI TextCardDes;
    public TextAsset CardDatabase;
    public List<Card> AllCardList, MyCardList, CurCardList;
    public int MyDeckNum = 0;
    void Start()
    {
       string[] line = CardDatabase.text.Substring(0, CardDatabase.text.Length - 1).Split('\n');
       for (int i = 0; i < line.Length; i++)
       {
            string[] row = line[i].Split('\t');
            AllCardList.Add(new Card(row[0], row[1], row[2], row[3], row[4], row[5], row[6] == "TRUE"));
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

        TakeCard();
    }

    public void TakeCard()
    {
        TextCardName.text = MyCardList[MyDeckNum].Name;
        TextCardDes.text = MyCardList[MyDeckNum].Description;
    }

    public void LeftCard()
    {
        MyDeckNum--;
        if (MyDeckNum < 0)
            MyDeckNum = 9;
            
        TakeCard();
        
    }

    public void RightCard()
    {
        MyDeckNum++;
        if (MyDeckNum > 9)
            MyDeckNum = 0;
        TakeCard();
    }
}
