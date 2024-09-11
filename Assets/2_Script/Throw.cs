using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPos;

    public GameObject grenade;

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
        throwGrenade();
    }

    private void throwGrenade()
    {
        if (gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Vector2 direction = (MousePos - (Vector2)player.transform.position).normalized; // 오브젝트 위치를 사용하여 방향을 계산합니다.
                GameObject orbInstance = PhotonNetwork.Instantiate("Grenade", spawnPos.position, Quaternion.identity);
                IShootable orb = orbInstance.GetComponent<IShootable>();
                if (orb != null)
                {
                    orb.Shoot(direction);
                }
                else
                {
                    Debug.Log("The orb does not implement IShootable interface.");
                }
            }
        }
    }
}
