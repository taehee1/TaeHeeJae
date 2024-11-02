using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float damage = 5f;           // ������ ��
    public float damageInterval = 1.0f; // ������ ����

    private Dictionary<int, float> lastDamageTimes = new Dictionary<int, float>(); // �� �÷��̾��� ������ ������ ����

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PhotonView playerPhotonView = collision.gameObject.GetComponentInParent<PhotonView>();

            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                Hp hp = collision.gameObject.GetComponentInParent<Hp>();

                if (hp != null)
                {
                    int playerId = playerPhotonView.ViewID; // �� �÷��̾ ������ ID

                    // �÷��̾��� ������ ������ ���� �������� (������ 0���� �ʱ�ȭ)
                    if (!lastDamageTimes.TryGetValue(playerId, out float lastDamageTime))
                    {
                        lastDamageTime = 0;
                    }

                    // ������ ������ ������ ���� ������ ����
                    if (Time.time >= lastDamageTime + damageInterval)
                    {
                        hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                        Debug.Log("������ ����: " + damage); // ������ ���� �� ���

                        // ������ ������ ���� ����
                        lastDamageTimes[playerId] = Time.time;
                    }
                }
            }
        }
    }
}