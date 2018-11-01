using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*CharacterControl2.cs
 * CharacterControl.cs에서 파생했으며, 기본적인 캐릭터 조작을 담당하며, 다른 플레이어의 조작을 동기화 하는데에도 쓰인다.
 * 
 */
public class CharacterControl2 : Photon.PunBehaviour , IPunObservable{

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
            GameObject g = Instantiate(bullet, transform.position+transform.forward, transform.rotation);
            g.GetComponent<Rigidbody>().velocity = transform.forward * 50f;
            Destroy(g, 5f);
        }
     

    }

}
