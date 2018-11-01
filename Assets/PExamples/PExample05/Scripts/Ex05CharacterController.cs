using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex05CharacterController : Photon.PunBehaviour {

    public PhotonVoiceSpeaker speaker;
    public GameObject Highlighter; //스피커 발생시 알려줄 객체
    public PhotonVoiceRecorder recorder;
    private void Start()
    {
        speaker = GetComponent<PhotonVoiceSpeaker>();
        recorder = GetComponent<PhotonVoiceRecorder>();
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (photonView.isMine)
        {
            Vector3 dir = Vector3.zero;
            dir.y += Input.GetAxis("Vertical");
            dir.x += Input.GetAxis("Horizontal");
            dir *= Time.fixedDeltaTime;
            dir += transform.position;


            dir.x = Mathf.Clamp(dir.x, -10, 10);
            dir.y = Mathf.Clamp(dir.y, -5, 5);
            transform.position = dir;
        }
	}

    private void LateUpdate()
    {
        if (photonView.isMine)
            Highlighter.SetActive( recorder.IsTransmitting);
        else
            Highlighter.SetActive( speaker.IsPlaying);
  
    }



}
