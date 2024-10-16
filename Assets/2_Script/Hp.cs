using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviourPunCallbacks
{
    public Image hpUI;
    public Vector3 spawnPosition;  // 시작 위치

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private float respawnTime;

    private PhotonView pv; // PhotonView 변수 추가
    private Movement movement;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        movement = GetComponent<Movement>();

        currentHp = maxHp;
        spawnPosition = transform.position;
        UpdateHpUI(); // 초기 UI 업데이트
    }

    private void Update()
    {
        if (respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            InGameManager.instance.deathUI.GetComponentInChildren<Text>().text = "RESPAWN : " + Mathf.Ceil(respawnTime).ToString();

            if (respawnTime <= 0)
            {
                photonView.RPC("OnRespawn", RpcTarget.All, photonView.Owner.NickName);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // 이 메서드는 모두가 호출할 수 있도록 함
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP를 최대값으로 클램프

        UpdateHpUI(); // HP UI 업데이트

        pv.RPC("RPC_ShakeCamera", PhotonNetwork.LocalPlayer);
        movement.StartCoroutine("Stun", 0.2f);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");

        InGameManager.instance.deathUI.SetActive(true);
        respawnTime = 10f;
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // 필요하다면 다른 클라이언트에서 플레이어의 리스폰 상태를 처리
        currentHp = maxHp;
        transform.position = spawnPosition;

        UpdateHpUI(); // 다른 클라이언트에서 HP UI를 업데이트
    }

    private void UpdateHpUI()
    {
        hpUI.fillAmount = currentHp / maxHp; // UI 업데이트
    }
}
