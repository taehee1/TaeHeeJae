using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRandom : MonoBehaviourPunCallbacks
{
    public GameObject[] guns;

    [PunRPC]
    public void RandomGunSetup(int random, string nickname)
    {
        if (photonView.Owner.NickName.ToString() == nickname)
        {
            foreach (var gun in guns)
            {
                gun.SetActive(false);
            }
            guns[random].SetActive(true);
        }
    }
}
