using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float damage = 5f;           // 데미지 값
    public float damageInterval = 1.0f; // 데미지 간격

    private Dictionary<int, float> lastDamageTimes = new Dictionary<int, float>(); // 각 플레이어의 마지막 데미지 시점

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
                    int playerId = playerPhotonView.ViewID; // 각 플레이어를 구분할 ID

                    // 플레이어의 마지막 데미지 시점 가져오기 (없으면 0으로 초기화)
                    if (!lastDamageTimes.TryGetValue(playerId, out float lastDamageTime))
                    {
                        lastDamageTime = 0;
                    }

                    // 데미지 간격이 지났을 때만 데미지 적용
                    if (Time.time >= lastDamageTime + damageInterval)
                    {
                        hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                        Debug.Log("데미지 전송: " + damage); // 데미지 전송 시 출력

                        // 마지막 데미지 시점 갱신
                        lastDamageTimes[playerId] = Time.time;
                    }
                }
            }
        }
    }
}