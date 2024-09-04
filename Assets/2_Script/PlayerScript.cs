using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;

    private float currentHp;
    private float maxHp;


    private void Update()
    {
        if (pv.IsMine)
        {
            transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * 7, 0, 0));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
    }
}
