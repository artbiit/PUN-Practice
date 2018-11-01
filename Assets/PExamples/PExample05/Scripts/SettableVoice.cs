using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon.Voice;
public class SettableVoice : Photon.PunBehaviour {

    #region publicVariable
    public Dropdown Mics;
    public Button ConnectButton;
    public Button CalibrateButton;
    public Text MuteButtonText;
    public PhotonVoiceRecorder recoder;
    #endregion
    #region privateVariable
    private Text ConnectButtonText;
 
    #endregion

    // Use this for initialization
    void Start () {

        //마이크 타입 유니티로 변경
        PhotonVoiceSettings.Instance.MicrophoneType = PhotonVoiceSettings.MicAudioSourceType.Unity;
        //마이크 드롭다운에 마이크 종류 넣기
        Mics.ClearOptions();
        List<string> mics = new List<string>();
        foreach (string s in Microphone.devices)
            mics.Add(s);
        Mics.AddOptions(mics);

        //필요한 변수 초기화
        ConnectButtonText = ConnectButton.GetComponentInChildren<Text>();

	}



    #region PhotonEvents
    public override void OnJoinedRoom() //방에 입장 성공시
    {
        GameObject g= PhotonNetwork.Instantiate("Ex05VoicePlayer", Vector3.zero, Quaternion.identity, 0);
        recoder = g.GetComponent<PhotonVoiceRecorder>();
        recoder.enabled = true;
        
    }

    #endregion

    #region UIEvents
    //보이스 연결 및 연결 해제
    public void ToggleVoice()
    {
        Debug.Log("ClientState : " + PhotonVoiceNetwork.ClientState);
        if(PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Disconnected)
        {
            PhotonVoiceNetwork.Connect();
            ConnectButtonText.text = "Disconnect";
            
        }
        else if(PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined)
        {
            PhotonVoiceNetwork.Disconnect();
            ConnectButtonText.text = "Connect";
        }
    }

    //마이크 변경
    public void ChangeMic(Dropdown drop)
    {
        PhotonVoiceNetwork.MicrophoneDevice = Microphone.devices[drop.value];
    }

    //시끄러움의 정도를 측정해서 디텍션레벨을 조정
    public void Calibrate()
    {
        recoder.VoiceDetectorCalibrate(2000);
        Debug.Log("2초동안 조용히 있으셔야 합니다.");
        StartCoroutine(WaitSec(CalibrateButton, 2f));
    }

    IEnumerator WaitSec(Button b, float sec)
    {
        b.enabled = false;
        yield return new WaitForSecondsRealtime(sec);
        b.enabled = true;
      
    }

    public void MuteToggle()
    {
        recoder.Transmit = !recoder.Transmit;
        MuteButtonText.text = "Mic " + (recoder.Transmit ? "on" : "off");
        
    }
    #endregion
}
