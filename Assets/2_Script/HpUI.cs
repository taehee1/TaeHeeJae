using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviourPunCallbacks
{
    public Image hpImage;

    public Vector3 offset = new Vector3(0, 0.4f, 0);  // HP 바의 오프셋
    private Transform target;                         // 타겟 플레이어의 Transform

    // 타겟을 설정하는 메서드
    public void SetTarget(GameObject player)
    {
        target = player.transform;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // 타겟의 머리 위 위치를 기반으로 HP 바 위치를 설정
        Vector3 targetPosition = target.position + offset;
        transform.position = targetPosition;
    }
}
