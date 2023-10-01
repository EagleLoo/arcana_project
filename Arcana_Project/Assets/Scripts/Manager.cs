using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class Manager : MonoBehaviourPunCallbacks
{
    public TMP_InputField NickNameInput;
    public GameObject LoginPanel;

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
        LoginPanel.SetActive(false);
    }
    // Update is called once per frame

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 로그인 패널 활성화
        LoginPanel.SetActive(true);
    }
}
