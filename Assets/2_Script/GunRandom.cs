using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRandom : MonoBehaviourPunCallbacks
{
    public GameObject[] guns;

    [PunRPC]
    private void RandomGunSetup(int random)
    {
        if (!photonView.IsMine) return;

        foreach (var gun in guns)
        {
            gun.SetActive(false);
        }
        guns[random].SetActive(true);
    }
}
