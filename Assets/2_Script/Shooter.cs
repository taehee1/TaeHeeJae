using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private List<GameObject> orbPrefabs;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private int orbIndex = 0; // 원하는 오브 프리팹의 인덱스를 선택합니다.
    [SerializeField] private float reboundForce;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject hand;

    public PhotonView pv;

    private float lastAngle;
    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Shoot();
        Reload();
    }

    private void Shoot()
    {
        if (player.GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized; // 오브젝트 위치를 사용하여 방향을 계산합니다.
                GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", spawnPos.position, Quaternion.identity);
                IShootable orb = orbInstance.GetComponent<IShootable>();
                if (orb != null)
                {
                    orb.Shoot(direction);
                }
                else
                {
                    Debug.Log("The orb does not implement IShootable interface.");
                }

                audioSource.Play();
                hand.GetComponent<Rigidbody2D>().AddForce(Vector2.up * reboundForce);
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
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        }
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            
        }
    }
}
