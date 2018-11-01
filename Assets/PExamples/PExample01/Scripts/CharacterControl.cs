using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : Photon.PunBehaviour , IPunObservable{

    public CharacterController crl;
    public bool shoot;
    public GameObject bullet;

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
            if (stream.isWriting)
            {
                stream.SendNext(shoot);

            }
            else
            {
                this.shoot = (bool)stream.ReceiveNext();
            }
    }

    // Use this for initialization
    void Start () {

        crl = GetComponent<CharacterController>();
        if (!photonView.isMine) Destroy(transform.GetChild(0).gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = Vector3.zero;
        if (photonView.isMine) {
            dir += Input.GetAxis("Vertical") * transform.forward;
            dir += Input.GetAxis("Horizontal") * transform.right;
            if (Input.GetKeyDown(KeyCode.Space)) shoot = true;
            else if (Input.GetKeyUp(KeyCode.Space)) shoot = false;
           
        }
        if (!crl.isGrounded) dir.y += Physics.gravity.y;
        crl.Move(dir * Time.fixedDeltaTime*4f);

        if (shoot)
        {
            Debug.Log(photonView.ownerId + " is shoot!");
            GameObject g = Instantiate(bullet, transform.position, transform.rotation);
            g.GetComponent<Rigidbody>().velocity = transform.forward * 50f;
            Destroy(g, 5f);
        }
     

    }

}
