using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviourPunCallbacks
{
    public Image hpUI;
    public Vector3 spawnPosition;  // ���� ��ġ

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private void Start()
    {
        currentHp = maxHp;
        spawnPosition = transform.position;
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
