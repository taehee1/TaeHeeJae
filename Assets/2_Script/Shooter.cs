using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private List<GameObject> orbPrefabs;
    [SerializeField] private Transform orbSpawnPos;
    [SerializeField] private int orbIndex = 0; // 원하는 오브 프리팹의 인덱스를 선택합니다.
    [SerializeField] private GameObject player;

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
        Shoot();
        GunPos();
    }

    private void Shoot()
    {
        if (player.GetComponent<PlayerScript>().pv.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 direction = (MousePos - (Vector2)player.transform.position).normalized; // 오브젝트 위치를 사용하여 방향을 계산합니다.
                GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", orbSpawnPos.position, Quaternion.identity);
                IShootable orb = orbInstance.GetComponent<IShootable>();
                if (orb != null)
                {
                    orb.Shoot(direction);
                }
                else
                {
                    Debug.LogError("The orb does not implement IShootable interface.");
                }
            }
        }
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

            GunFlip();
        }
    }

    private void GunFlip()
    {
        float angle = transform.rotation.eulerAngles.z;

        if (angle > 180f)
        {
            angle -= 360f;
        }

        if (angle > 90f || angle < -90f)
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        }
    }
}
