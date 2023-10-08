using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Chat.UtilityScripts;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField NickNameInput;
    public GameObject LoginPanel;
    public GameObject WinnerPanel;
    public GameObject DeckPanel;
    public GameObject SwordImage;
    public GameObject WandImage;
    public GameObject LosePanel;
    public Toggle WarToggle;
    public Toggle WizToggle;
 
    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        //PhotonNetwork.ConnectUsingSettings();
        //SceneManager.LoadScene(0);
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        // 방이 있으면 참가 Or 없으면 방 생성
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions {MaxPlayers = 2}, null);
    }
    
    public override void OnJoinedRoom() 
    {
        DeckPanel.SetActive(false);
        Spawn();
    }
    // Update is called once per frame

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) QuitGame();
    }

    public void Spawn()
    {
        // 캐릭터 선택
        if (WarToggle.isOn)
            PhotonNetwork.Instantiate("Player1", Vector3.zero, Quaternion.identity);
        else if (WizToggle.isOn)
            PhotonNetwork.Instantiate("Player2", Vector3.zero, Quaternion.identity);
    }

    public void DeckOpen()
    {
        DeckPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }

    public void DeckQuit()
    {
        DeckPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }

    public void QuitGame()
    {
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
        WinnerPanel.SetActive(false);
        LosePanel.SetActive(false);
    }

    public void WarriorWeapon()
    {
        SwordImage.SetActive(true);
        WandImage.SetActive(false);
    }

    public void WizardWeapon()
    {
        SwordImage.SetActive(false);
        WandImage.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 로그인 패널 활성화
        LoginPanel.SetActive(true);
    }
}
