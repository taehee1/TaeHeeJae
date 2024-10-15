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
    
    //변수동기화
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
