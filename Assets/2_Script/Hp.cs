using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviourPunCallbacks
{
    public Image hpUI;
    public Vector3 spawnPosition;  // 시작 위치

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private PhotonView photonView; // PhotonView 변수 추가

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentHp = maxHp;
        spawnPosition = transform.position;
        UpdateHpUI(); // 초기 UI 업데이트
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // 이 메서드는 모두가 호출할 수 있도록 함
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP를 최대값으로 클램프

        UpdateHpUI(); // HP UI 업데이트

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // HP 초기화하고 리스폰 위치로 이동
        currentHp = maxHp;
        transform.position = spawnPosition;
        UpdateHpUI(); // HP UI 업데이트

        // 추가적인 리스폰 로직 (무적시간, 애니메이션 등)
        photonView.RPC("OnRespawn", RpcTarget.All, photonView.Owner.NickName);
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // 필요하다면 다른 클라이언트에서 플레이어의 리스폰 상태를 처리
        UpdateHpUI(); // 다른 클라이언트에서 HP UI를 업데이트
    }

    private void UpdateHpUI()
    {
        hpUI.fillAmount = currentHp / maxHp; // UI 업데이트
    }
}
