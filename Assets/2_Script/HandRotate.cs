using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRotate : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SpriteRenderer gunSpr;

    public PhotonView pv;

    private float lastAngle;

    private Camera cam;

    Vector2 MousePos
    {
        get
        {
            // z = 0으로 설정하여 마우스의 2D 위치를 가져옵니다.
            Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
            return mousePos;
        }
    }

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        GunPos();
    }

    private void GunPos()
    {
        if (player.GetComponent<PlayerScript>().pv.IsMine)
        {
            // 마우스 위치와 총의 위치 간의 방향을 계산합니다.
            Vector2 direction = MousePos - (Vector2)player.transform.position;

            // 방향 벡터에서 각도를 계산합니다.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 총의 오브젝트 회전을 변경합니다. Z축을 기준으로 회전하므로 Quaternion.Euler을 사용합니다.
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (Mathf.Abs(lastAngle - angle) > 1f) // 임계값 1도 차이로 설정
            {
                pv.RPC("GunFlip", RpcTarget.AllBuffered);
                lastAngle = angle;
            }
        }
    }

    [PunRPC]
    void GunFlip()
    {
        float angle = transform.rotation.eulerAngles.z;

        if (angle > 180f)
        {
            angle -= 360f;
        }

        if (angle > 90f || angle < -90f)
        {
            gunSpr.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gunSpr.GetComponent<SpriteRenderer>().flipY = false;
        }
    }
}
