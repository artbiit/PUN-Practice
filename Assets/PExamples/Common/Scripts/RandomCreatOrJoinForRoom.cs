using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCreatOrJoinForRoom : Photon.PunBehaviour
{

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("1.0"); //연결시도

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnConnectedToMaster() //마스터 서버에 연결 성공하면 랜덤입장 시도
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause) //연결 실패시 재연결 시도
    {
        Debug.LogError(cause + " : Retrying Connect");
        PhotonNetwork.ConnectUsingSettings("1.0");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) //랜덤입장 실패시 방생성
    {
        PhotonNetwork.CreateRoom("Random");
    }

    public override void OnJoinedRoom() //방에 입장 성공시
    {
        Debug.Log("입장 성공");
    }

    public override void OnCreatedRoom() //방 생성 성공시
    {
        Debug.Log("생성 성공");
    }
}
