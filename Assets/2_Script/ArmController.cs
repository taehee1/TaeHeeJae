using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public float rotationSpeed = 5f; // 회전 속도 조절
    private Balance balance; // Balance 스크립트 참조

    void Start()
    {
        // Balance 스크립트 가져오기
        balance = GetComponent<Balance>();
    }

    void Update()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0; // 2D 환경이므로 Z축을 0으로 고정

        // Upper Arm의 현재 위치에서 마우스 방향으로의 벡터 계산
        Vector2 direction = (mousePosition - transform.position).normalized;

        // 목표 회전 각도 계산 (여기에 +90도를 추가)
        float targetRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Balance 스크립트의 targetRotation 값 업데이트
        balance.targetRotation = targetRotation;
    }
}
