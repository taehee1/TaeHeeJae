using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;

    public Rigidbody2D rb;
    public Image hpUI;

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    public Vector3 spawnPosition;  // ���� ��ġ

    private void Start()
    {
        currentHp = maxHp;
        spawnPosition = transform.position;
    }
    
    //��������ȭ
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hpUI.fillAmount);
        }
        else
        {
            hpUI.fillAmount = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        currentHp -= damage;
        hpUI.fillAmount = currentHp / maxHp;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // ü���� �ʱ�ȭ�ϰ� ������ ��ġ�� �̵�
        currentHp = maxHp;
        transform.position = spawnPosition;
        hpUI.fillAmount = currentHp / maxHp;

        // �߰����� ������ ���� (�����ð�, �ִϸ��̼� ��)
        photonView.RPC("OnRespawn", RpcTarget.All, photonView.Owner.NickName);
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // �ʿ��ϴٸ� �ٸ� Ŭ���̾�Ʈ���� �÷��̾��� ������ ���¸� ó��
    }
}
