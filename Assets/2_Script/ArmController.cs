using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public HingeJoint2D upperArmHinge;  // 상완(Upper Arm)의 힌지 조인트
    public Camera mainCamera;            // 카메라 참조
    public float maxMotorSpeed = 100f;   // 모터 최대 속도
    public float maxMotorTorque = 1000f; // 모터 최대 힘(토크)
    public float damping = 5f;           // 부드러운 회전을 위한 감쇠값
    public float angleThreshold = 1f;    // 회전 민감도

    private const float baseRotationZ = -90f; // 기본 회전 각도

    void Update()
    {
        // 마우스 위치를 가져오고 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0; // 2D 게임이므로 z축은 0으로 고정

        // Upper Arm과 마우스 위치 사이의 방향 벡터 계산
        Vector2 direction = mousePosition - upperArmHinge.transform.position;

        // 마우스 방향의 각도 계산
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 현재 상완의 회전 각도 가져오기
        float currentAngle = upperArmHinge.transform.rotation.eulerAngles.z;

        // 각도 차이 계산
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle + baseRotationZ);

        // 각도 차이가 임계값을 벗어날 때만 모터를 활성화
        if (Mathf.Abs(angleDifference) > angleThreshold)
        {
            // 모터 속도를 부드럽게 조정
            float motorSpeed = Mathf.Clamp(angleDifference * damping, -maxMotorSpeed, maxMotorSpeed);

            // HingeJoint2D 모터 설정
            JointMotor2D motor = new JointMotor2D
            {
                motorSpeed = motorSpeed,         // 회전 방향과 속도 설정
                maxMotorTorque = maxMotorTorque  // 모터의 최대 힘 설정
            };

            upperArmHinge.motor = motor;
            upperArmHinge.useMotor = true; // 모터 활성화
        }
        else
        {
            upperArmHinge.useMotor = false; // 각도 차이가 작으면 모터 비활성화
        }
    }
}
