using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;
    public Hp hpScript;
    
    //��������ȭ
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hpScript.hpUI.fillAmount);
        }
        else
        {
            hpScript.hpUI.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
