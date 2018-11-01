using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
public class ChatExample : MonoBehaviour, IChatClientListener {

    #region public variables
    [Tooltip("가입할 공개 채팅방 목록, Inspector에서 설정한다.")]
    [SerializeField]
    public string[] ChannelsToJoinOnConnect=new string[1];
    [Tooltip("채팅 메세지 입력 창, 엔터를 눌러도 반응 함")]
    public InputField ChatInput;
    [Tooltip("전송 버튼")]
    public Button ChatEnterButton;
    [Tooltip("송수신 된 메세지가 표시될 메세지박스")]
    public InputField MessageBox;
    [Tooltip("초기에 로그인에 사용 되며, 이후 상태를 표시하는 박스")]
    public InputField loginAndState;
    [Tooltip("로그인에 사용할 버튼. 로그인후엔 비활성화 된다.")]
    public Button LoginButton;
    
    #endregion

    #region private variables
    /// <summary>
    /// 챗클라이언트로서 서버에 연결하는 등의 기능을 수행할 변수
    /// </summary>
   public ChatClient chatClient;
    /// <summary>
    /// 현 상태를 저장할 변수, OnChatStateChange()를 통해 변경된다.
    /// </summary>
    private ChatState chatState;
    /// <summary>
    /// 로그인 버튼의 텍스트를 저장할 변수, 문자열에 따라서 이벤트 처리에 사용 된다.
    /// </summary>
    private Text LoginButtonText;
    #endregion

    #region Unity's Callbacks
    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
        LoginButtonText = LoginButton.GetComponentInChildren<Text>();
        loginAndState.Select();
        loginAndState.ActivateInputField();
    }
	
	// Update is called once per frame
	void Update () {
        /* 반드시 작성해야 한다. 이 부분을 작성하지 않으면 연결 시도 후 연결 확립 부터, 이 후 데이터를 주고 받는 모든 행위가 불가능 해진다.
         * 따라서 반드시 이 부분을 작성한다.
         */
        if (chatClient != null) chatClient.Service();
	}
    #endregion

    #region InterfaceFunctions
    //디버그 내용이 전달되었을 때의 함수
    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("[" + level + "]Debug : " + message);
    }

    //채팅의 상태가 변경되었을 때의 함수, 주로 연결되었는지 해제되었는지 등을 가리킴
    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        //OnConnected()와 OnDisconnected()를 사용하는 것 보다 실용적이다.
        chatState = state;
        loginAndState.text = state+"";
        Debug.Log("OnChatStatuChange : " + state);
    }

    //연결에 성공하였을시 실행될 이벤트
    void IChatClientListener.OnConnected()
    {
        Debug.Log("연결 성공");
        if (!LoginButton.gameObject.activeInHierarchy) LoginButton.gameObject.SetActive(true);
        LoginButtonText.text = "Disconnect";
        LoginButton.enabled = true;
        loginAndState.readOnly = true;
        ChatInput.readOnly = false;
        ChatEnterButton.enabled = true;
        if (ChannelsToJoinOnConnect != null)
            chatClient.Subscribe(ChannelsToJoinOnConnect);
        loginAndState.text = "Connected";

        ChatInput.Select();
        ChatInput.ActivateInputField();
    }

    //연결에 해제되면 불려진다.
    void IChatClientListener.OnDisconnected()
    {
        if (!LoginButton.gameObject.activeInHierarchy) LoginButton.gameObject.SetActive(true);
        LoginButtonText.text = "Login";
        LoginButton.enabled = true;
        loginAndState.readOnly = false;
        loginAndState.text = "";
        ChatInput.readOnly = true;
        ChatEnterButton.enabled = false;
        MessageBox.text = "";
        loginAndState.Select();
        loginAndState.ActivateInputField();
    }

    //채널에서 넘어온 메세지를 표시한다.
    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
        for(int i = 0; i < senders.Length; i++) { 
            MessageBox.text += string.Format("[{0}]{1} : {2}\n",channelName,senders[i],messages[i]);
        }
    }

    //비공개 메세지를 표시한다.
    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        MessageBox.text += string.Format("[{0}]{1} : {0}\n", sender, message);
    }

    //유저의 상태가 변경되면 로그를 표시한다.
    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log(string.Format("OnStatusUpdate: {0} is {1} - {2} : {3}\n", user, status, gotMessage, message));
    }

    //공개 채널에 가입에 성공하면 표시한다.
    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("OnSubscribed : " + channels[0]);
    }

    //공개 채널에 탈퇴하면 표시한다.
    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        Debug.Log("OnUnsubscribed : " + channels[0]);
    }

 
    #endregion

    #region UIEvents

    //로그인에 사용 될 엔터 이벤트. 사용할 오브젝트 - LoginAndState
    public void EndEditOnEnterForLogin()
    {
        if(Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
        {
            Connect();
        }
    }
    //로그인에 사용 될 클릭 이벤트. 사용할 오브젝트 - LoginButton
    public void ClickToLogin()
    {
        if (LoginButtonText.text == "Login")
            Connect();
        else //Login이 아니라면 연결을 해제 한다.
            chatClient.Disconnect();
    }

    //엔터를 통해 메세지를 보낼 때 사용할 이벤트. 사용할 오브젝트 - ChatInput
    public void EndEditOnEnterForSend()
    {
        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
        {
            Send();
        }
    }

    //버튼 클릭을 통해 메세지를 보낼 떄 상요할 이벤트. 사용할 오브젝트 - ChatEnterButton
    public void ClickToSend()
    {
        Send();
    }
    #endregion

    #region privateFunctions
    //채팅 서버 로그인에 사용할 함수
    private void Connect()
    {
        //로그인 버튼의 문자열이 Login이 아니라면 연결에 성공하였거나 시도 중이란 것이니 더이상 연결 시도를 하지 않는다.
        if ( LoginButtonText.text=="Disconnect") return;

        //비어있는 문자열을 사용하려 하면 리턴
        if (string.IsNullOrEmpty(loginAndState.text)) return;

        //chatClinet 생성 매개 변수로 IChatClientListener을 상속한 이 스크립트를 전달한다.
        //주의할 점은 클라이언트로서의 기능을 수행하기 위해 스크립트를 등록한 것이지, 아직 포톤클라우드에 연결한 상태가 아니다.
        this.chatClient = new ChatClient(this);

        //연결지역을 설정한다. 가능 지역은 EU, US, ASIA 세 곳이다.
        chatClient.ChatRegion = "ASIA";

        //백그라운드에서도 연결을 유지할지에 대한 변수다. 이를 false로 할 시 프로그램이 백그라운드로 설정 될 때 연결이 끊긴다.
        chatClient.UseBackgroundWorkerForSending = true;

        //실제로 연결에 시도하는 함수다.
        //ChatAppID와 버전을 이용해서 연결을 분류한다.
        //그리고 유저의 이름을 string 그대로 이용하지 않고 아래와 같이 변환한다.
        this.chatClient.Connect(PhotonNetwork.PhotonServerSettings.ChatAppID, "1.0", new ExitGames.Client.Photon.Chat.AuthenticationValues(loginAndState.text.Trim()));
        Debug.Log("연결시도 : "+chatClient.UserId);

        LoginButtonText.text = "Connecting...";
        LoginButton.enabled = false;
        //이곳에서 연결에 성공 한다면 OnConnected()를 통해 채팅 입력이 가능하도록 변한다.
    }

    
    private void Send()
    {
        string msg = ChatInput.text;
        ChatInput.text = "";
        //비어있다면 취소
        if (string.IsNullOrEmpty(msg)) return;
        Debug.Log("전송시도 : " + msg);
        if (msg.StartsWith("/")){ //명령어 확인
            string[] splitted = msg.Split(' '); //인자 인식을 위한 명령어 분리

            if (splitted[0]=="/w") //귓속말
            {
                chatClient.SendPrivateMessage(splitted[1], splitted[2], true, false);
            }else if (splitted[0] == "/help" || splitted[0] == "/h" || splitted[0] == "/?")//도움말
            {
                MessageBox.text += "/help, /h, /? : 이 메세지 보여줌. \n/w {대상} {메세지} : 귓속말을 보냄 \n/exit 연결을 끊음";
            }else if(splitted[0] == "/exit") //연결 해제
            {
                chatClient.Disconnect();
            }
            else
            {
                MessageBox.text += "없는 명령어 입니다. 도움말을 확인하려면 /help, /h, /? 중 하나를 입력해주세요.";
            }
        }
        else //명령어가 아니라면
        {
            chatClient.PublishMessage(ChannelsToJoinOnConnect[0], msg); 
        }
        //입력창에 포커스를 준다.
        ChatInput.Select();
        ChatInput.ActivateInputField();
    }
    #endregion
}
