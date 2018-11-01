using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerEx :  Photon.PunBehaviour{
    public  GameObject PlayerPre ;



    public static GameManagerEx instance;
    private void Awake()
    {
        if (instance != null) Destroy(this); 
        instance = this;
        PlayerPre = Resources.Load<GameObject>("Player");
    }

    private void Start()
    {
        PhotonNetwork.Instantiate(this.PlayerPre.name, new Vector3(0f, 7f, 0f),Quaternion.identity, 0);
    }


    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("OnPPC() " + newPlayer.NickName);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPPC isMasterClient " + PhotonNetwork.isMasterClient);
            LoadArena();
        }
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        Debug.Log(info);
    }
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
        

        Debug.Log("OPPD() " + otherPlayer.NickName);
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OPPD isMasterClient " + PhotonNetwork.isMasterClient);
            LoadArena();
        }
    }


    void LoadArena()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PUN : 마스터 클라이언트만 레벨 로드를 시도한다.");
        }
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}

public class PlayerObject
{
    public GameObject Player;
    public int OwenerID;
}