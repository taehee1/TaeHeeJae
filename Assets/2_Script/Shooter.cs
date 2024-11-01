using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private int gunTypeIndex = 0; // 원하는 오브 프리팹의 인덱스를 선택합니다.
    [SerializeField] private float reboundForce;
    [SerializeField] private float knockBackForce;
    [SerializeField] private float delay;
    [SerializeField] private float bulletDamage;

    private bool canShoot = true;

    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject hand;

    public PhotonView pv;
    public Movement movement;

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
        if (player.GetComponent<PhotonView>().IsMine && canShoot)
        {
            if (movement.canMove)
            {
                switch (gunTypeIndex)
                {
                    case 0:
                        if (Input.GetMouseButtonDown(0))
                        {
                            StartCoroutine(ShootCooldown(delay));
                            Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized;
                            GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", spawnPos.position, Quaternion.identity);
                            IShootable orb = orbInstance.GetComponent<IShootable>();
                            if (orb != null)
                            {
                                orb.Shoot(direction);
                                orbInstance.GetComponent<Bullet>().damage = bulletDamage;
                            }
                            audioSource.Play();
                            hand.GetComponent<Rigidbody2D>().AddForce(Vector2.up * reboundForce);
                        }
                        break;

                    case 1:
                        if (Input.GetMouseButtonDown(0))
                        {
                            StartCoroutine(ShootCooldown(delay));
                            int bulletCount = 5;
                            float spreadAngle = 5f;
                            Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized;

                            for (int i = 0; i < bulletCount; i++)
                            {
                                float angleOffset = (i - (bulletCount - 1) / 2f) * spreadAngle;
                                Vector2 spreadDirection = Quaternion.Euler(0, 0, angleOffset) * direction;

                                GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", spawnPos.position, Quaternion.identity);
                                IShootable orb = orbInstance.GetComponent<IShootable>();
                                if (orb != null)
                                {
                                    orb.Shoot(spreadDirection);
                                    orbInstance.GetComponent<Bullet>().damage = bulletDamage;
                                }
                            }
                            audioSource.Play();
                            hand.GetComponent<Rigidbody2D>().AddForce(Vector2.up * reboundForce);
                            movement.body.GetComponent<Rigidbody2D>().AddForce(-direction * knockBackForce);
                        }
                        break;

                    case 2:
                        {
                            float maxZoomOutSize = 9f;
                            float zoomInSpeed = 2f;
                            float zoomOutSpeed = 1f;

                            if (Input.GetMouseButton(1)) // 우클릭으로 줌 아웃
                            {
                                // 현재 OrthographicSize에서 목표 OrthographicSize까지 부드럽게 보간
                                InGameManager.instance.virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                                    InGameManager.instance.virtualCamera.m_Lens.OrthographicSize,
                                    maxZoomOutSize,
                                    Time.deltaTime * zoomOutSpeed// 보간 비율을 시간에 따라 조정
                                );
                            }
                            else // 우클릭을 떼면 기본 크기로 돌아옴
                            {
                                InGameManager.instance.virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                                    InGameManager.instance.virtualCamera.m_Lens.OrthographicSize,
                                    5f, // 기본 줌 크기
                                    Time.deltaTime * zoomInSpeed
                                );
                            }

                            if (Input.GetMouseButtonDown(0)) // 좌클릭을 뗐을 때 발사
                            {
                                StartCoroutine(ShootCooldown(delay));
                                Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized;
                                GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", spawnPos.position, Quaternion.identity);
                                IShootable orb = orbInstance.GetComponent<IShootable>();
                                if (orb != null)
                                {
                                    orb.Shoot(direction);
                                    orbInstance.GetComponent<Bullet>().damage = bulletDamage;
                                }
                                audioSource.Play();
                                hand.GetComponent<Rigidbody2D>().AddForce(Vector2.up * reboundForce);
                                movement.body.GetComponent<Rigidbody2D>().AddForce(-direction * knockBackForce);
                            }
                        }
                        break;
                }
            }
        }
    }

    IEnumerator ShootCooldown(float seconds)
    {
        canShoot = false;
        yield return new WaitForSeconds(seconds);
        canShoot = true;
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            
        }
    }
}
