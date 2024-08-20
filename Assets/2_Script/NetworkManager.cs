using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField nicknameInput;
    public Text networkSituation;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        nicknameInput.text = null;
    }

    private void Update()
    {
        networkSituation.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void Connect()
    {
        if (nicknameInput.text != null)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("�г����� �Է����ּ���");
        }
    }

    public override void OnConnectedToMaster()
    {
        print("��������Ϸ�");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("�������");
    }
}
