using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*Lobby.cs 
 * Lobby Scene에서 사용되며, 방을 생성하거나 방에 입장하도록 해준다.
 * 방목록이 바뀌면 동기화를 다시 해주고, 방 입장에 성공하면 Room Scene으로 이동시켜준다.
 * 
 */ 
public class Lobby : Photon.PunBehaviour {

    //가려져있는 버튼의 원본. 방이 있을 경우 이를 복제하여 방목록을 보여주고, 없다면 이 오브젝트를 방생성 버튼으로 변경해준다.
    public Button Room;  
    public GameObject CreateRoom;//방생성 UI가 들어있는 엠티오브젝트
    public Text RoomName; //방의 이름이 적힌 인풋필드의 텍스트
    public Text CreateNotice; //방 생성시 발생한 안내문
	// Use this for initialization
	void Start () {

        //이미 Login Scene에서 로비 입장까지 완료하고 왔기에 이벤트 콜백이 아닌 Start에서 방목록을 1차적으로 호출한다.
        ShowRoomList();
    }

    //방에 입장을 하도록 하는 함수
    public void JoinRoom(string RoomName)
    {
        PhotonNetwork.JoinRoom(RoomName);
    }

    //방생성 시도 함수
    public void TryCreateRoom()
    {
        if (RoomName.text.Length == 0) return;
        //IsOpen 공개방 여부, IsVisible 방 목록에 보이게 할지 여부
        PhotonNetwork.CreateRoom(RoomName.text, new RoomOptions { MaxPlayers = 5, IsOpen=true, IsVisible=true, PublishUserId=true},TypedLobby.Default);
    }

    //방생성 UI를 보이도록 하는 함수
    public void Visible()
    {
        CreateRoom.SetActive(true);
    }


    //방 목록을 보여주는 함수
    private void ShowRoomList()
    {
        //방에 들어갔다가 다시 돌아오면 로비와의 연결이 해제되어있으므로 다시 한번 연결을 해줘야 한다.
        if (!PhotonNetwork.insideLobby)
        {
            Debug.LogError("로비 접속 여부 : " + PhotonNetwork.insideLobby + " 방 갯수 " + PhotonNetwork.countOfRooms);
            PhotonNetwork.JoinLobby();
            return;
        }
        Debug.LogError("로비 접속 여부 : " + PhotonNetwork.insideLobby + " 방 갯수 " + PhotonNetwork.countOfRooms);
        int i = 0; //방의 번호이며, 방이 있는지 확인하는 변수

        foreach (RoomInfo R in PhotonNetwork.GetRoomList())
        {
            Button B = Instantiate<Button>(Room);
            B.name = R.Name;
            B.transform.parent = Room.transform.parent;
            B.transform.localPosition = new Vector3(0, i * B.GetComponent<RectTransform>().rect.height*-1-30, 0);
            B.GetComponentInChildren<Text>().text = i + "-" + R.Name + "[" + R.PlayerCount + "/" + R.MaxPlayers + "]";
            //해당 방에 입장 가능하도록 이벤트 설정
            B.onClick.AddListener(delegate { JoinRoom(R.Name); });
            B.gameObject.SetActive(true);
            i++;
        }
    }

    //방목록에 변화가 발생하면 방 목록을 새로 설정함
    public override void OnReceivedRoomListUpdate()
    {
        Transform Content = Room.transform.parent;
        //방 목록이 재 설정될 때마다 내용물을 비우고서 다시 작성한다.
        for(int x = 0; x<Content.childCount; x++)
        {
            if (Content.GetChild(x).name == Room.name)
            {
                continue;
            }
            Destroy(Content.GetChild(x).gameObject);
        }
        ShowRoomList();
       
    }

    //방생성이 실패할시 발생하는 이벤트
    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        string s = "";
        foreach(object o in codeAndMsg)
        {
            s +=o.ToString()+"\n";
        }
        CreateNotice.text = s;
    }


    //룸에 입장되었을시 발생하는 이벤트
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameObject.scene.buildIndex + 1);
    }

    //Login Scene에서 넘어왔을땐 이미 Lobby에 연결되어있지만 Room Scene에 다녀왔을 땐 방과의 연결 때문에 해제된 상태이므로 다시 연결하도록 되어있다.
    //따라서 로비에 연결을 요청하고 침묵한 룸목록 함수를 다시한번 호출해준다.
    public override void OnJoinedLobby()
    {
        Debug.LogError("로비 접속");
        ShowRoomList();
    }
}
