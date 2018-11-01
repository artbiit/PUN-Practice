using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomUI : Photon.PunBehaviour {

    public Text PlayerList;

	
	void Start () {
        RefreshPlayerList();
	}

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
       
        RefreshPlayerList();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
       
        RefreshPlayerList(); 
    }

    //사용자 목록을 다시 작서한다.
    private void RefreshPlayerList()
    {
        string s = "PlayerList\n";
        Debug.LogError(PhotonNetwork.playerList.Length);
        foreach(PhotonPlayer p in PhotonNetwork.playerList)
        {
            s += "["+p.NickName + "]\n";
        }
        PlayerList.text = s;
    }

    //방에서 나간다.
    public void LeaveRoom()
    {
        //방을 나간다. 방을 나갈 때 포톤은 접속해있던 로비가 아닌 마스터 서버로 인도한다. 
        //따라서, 다시 로비를 이용하게 하려면 OnConnectedToMaster()가 호출되는 타이밍에 로비에 다시 접속을 유도해야한다.
        PhotonNetwork.LeaveRoom();

    }

    //마스터 서버에 연결되었을때 호출된다. 이 경우 방에서 나가면서 발생한다.
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    //다시 로비에 연결이 되었으면 Lobby Scene으로 이동한다.
    public override void OnJoinedLobby()
    {
        //Lobby Scene으로 복귀
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameObject.scene.buildIndex - 1);
    }
}
