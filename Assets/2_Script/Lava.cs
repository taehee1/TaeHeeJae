using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float damage = 5f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        }
    }
}
