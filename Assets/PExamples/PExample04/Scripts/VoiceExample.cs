using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceExample : Photon.PunBehaviour
{
    public Text State;
    public Dropdown Mics;
    public PhotonVoiceSettings voiceSetting;
    //자신의 레코더
    public PhotonVoiceRecorder recorder;
    //자신의 포톤뷰
    public PhotonView MicPhotonView;

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;
        
        PhotonNetwork.ConnectUsingSettings("1.0");

        voiceSetting = GetComponent<PhotonVoiceSettings>();
       
    }
    // Use this for initialization
    void Start () {
        List<string> mics = new List<string>();
        foreach (string s in Microphone.devices)
            mics.Add(s);
        Mics.ClearOptions();
        Mics.AddOptions(mics);
        Mics.onValueChanged.AddListener(delegate { ChangeMic(Mics); });
	}

    // Update is called once per frame
    float[] color = new float[3];
	void Update () {
        
        if (recorder.IsTransmitting && recorder != null)
        {
            color[0] = Random.Range(0, 255);
            color[1] = Random.Range(0, 255);
            color[2] = Random.Range(0, 255);
            MicPhotonView.RPC("ChangeColor", PhotonTargets.All, color);
        }
	}
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 0 }, null);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
      GameObject g= PhotonNetwork.Instantiate("VoicePlayerCube", Vector3.zero, Quaternion.identity, 0);
        recorder = g.GetComponent<PhotonVoiceRecorder>();
        recorder.enabled = true;
        MicPhotonView = g.GetComponent<PhotonView>();
       g.transform.position = new Vector3(-10 + PhotonNetwork.room.PlayerCount-1, 0, 1);

        

        State.text = "Connect";
    }



    public void ChangeMic(Dropdown dr)
    {
        PhotonVoiceNetwork.MicrophoneDevice = Microphone.devices[dr.value];
    }


}


