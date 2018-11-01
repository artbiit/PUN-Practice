using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherEx : Photon.PunBehaviour
{
    //게임 버전, 변화점 적용 가능 
    string _gameVersion = "1";

    
    private void Awake()
    {
        //중요치 않음. 전체 로깅 레벨 
        PhotonNetwork.logLevel = PhotonLogLevel.Full;

        //중요. 로비에 가입하지 않음. 룸 목록을 보려면 로비에 가입해야 함
        PhotonNetwork.autoJoinLobby = false;

        //닉네임 설정
        PhotonNetwork.playerName = Random.Range(0f, 1000f) + "ASDF" + 4;
        Debug.Log("설정된 닉네임 : " + PhotonNetwork.playerName);

        //중요. 이렇게 하면 마스터 클라이언트에서 PhotonNetwork.LoadLevel() 사용 가능. 자동으로 레벨을 동기화 함
        PhotonNetwork.automaticallySyncScene = true;
    }//Awake


    
    void Start() {
        Connect();
    }
	

    //연결 시도 함수
    //이미 연결 되어 있다면 아무 방에 접속을 시도 한다.
    //연결되어 있지 않다면 PCN에 연결한다.
    public void Connect()
    {
        if (PhotonNetwork.connected)
        {
            //연결에 실패시 OnPhotonRandomJoinFailed ()을 호출 한다.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // 먼저 Photon Online Server에 접속 해야 한다.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if(PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("방에 입장함. Room for 1 로드");

            AsyncOperation op =  PhotonNetwork.LoadLevelAsync("Room for 1");
   
        }
  
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OCTM() MASTER와 연결, 무작위 방 입장 시도");
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        base.OnPhotonRandomJoinFailed(codeAndMsg);
        Debug.Log("OPRJF() 방 생성 시도");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }, null);
    }



}
