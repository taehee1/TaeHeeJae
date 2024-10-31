using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRandom : MonoBehaviourPunCallbacks
{
    public GameObject[] guns;

    private void Start()
    {
        if (photonView.IsMine)
        {
            int random = Random.Range(0, guns.Length);
            photonView.RPC("RandomGunSetup", RpcTarget.All, random);
        }
    }

    [PunRPC]
    private void RandomGunSetup(int random)
    {
        foreach (var gun in guns)
        {
            gun.SetActive(false);
        }
        guns[random].SetActive(true);
    }
}
