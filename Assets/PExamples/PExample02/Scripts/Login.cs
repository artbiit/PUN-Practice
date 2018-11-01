using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*Login.cs
 * Login Scene에서 사용되며 닉네임을 입력받고 포톤 마스터 서버와 로비에 접속하도록 해주고 Lobby Scene으로 이동하게 해주는 스크립트 
 *
 */
public class Login : Photon.PunBehaviour {
    string GameVersion = "1";
    public PhotonLogLevel LogLevel = PhotonLogLevel.Full; //인스펙터에 노출한 로그 레벨
    public Text Nickname; //별명이 입력된 텍스트 필드
    public Text Notice; //버튼 하단의 안내 메세지

    void Awake()
    {
        PhotonNetwork.logLevel = LogLevel;
        //로비 입장 scenes을 만들 예정이라 자동 로비 입장을 차단
        PhotonNetwork.autoJoinLobby = false;
        //저번엔 방의 주인이 직접 로드할 scene을 택했지만 이번엔 자동으로 동기화 하도록 활성화 한다.
        PhotonNetwork.automaticallySyncScene = true;
        
    }

    //로그인 시도
    public void TryLogin()
    {
        
        if (Nickname.text.Length == 0)
        {
            Notice.text = "최소 한글자 이상의 닉네임만 허용됩니다.";
            return;
        }
        
        //포톤에 자신의 닉네임 설정
        PhotonNetwork.playerName = Nickname.text;
        PhotonNetwork.ConnectUsingSettings(GameVersion); //연결 시도
        //연결 성공시   OnConnectedMaster() 호출
        //연결 실패시 OnFailedToConnectToPhoton() 호출
   
    }

    //포톤과 연결 실패시 
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Notice.text = "연결에 실패 하였습니다. 잠시 후 다시 시도해 주십시오. "+cause;
    }


    //PhotonNetwork.ConnectUsingSettings() 에 의해 연결 성공시
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); //로비 접속
    }

    //JoinLobby()에 의해 로비 입장 성공시
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(gameObject.scene.buildIndex + 1); //로비신으로 이동
    }


}
