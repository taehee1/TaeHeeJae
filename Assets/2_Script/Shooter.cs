using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private int gunTypeIndex = 0; // ���ϴ� ���� �������� �ε����� �����մϴ�.
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
            // z = 0���� �����Ͽ� ���콺�� 2D ��ġ�� �����ɴϴ�.
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

                            if (Input.GetMouseButton(1)) // ��Ŭ������ �� �ƿ�
                            {
                                // ���� OrthographicSize���� ��ǥ OrthographicSize���� �ε巴�� ����
                                InGameManager.instance.virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                                    InGameManager.instance.virtualCamera.m_Lens.OrthographicSize,
                                    maxZoomOutSize,
                                    Time.deltaTime * zoomOutSpeed// ���� ������ �ð��� ���� ����
                                );
                            }
                            else // ��Ŭ���� ���� �⺻ ũ��� ���ƿ�
                            {
                                InGameManager.instance.virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                                    InGameManager.instance.virtualCamera.m_Lens.OrthographicSize,
                                    5f, // �⺻ �� ũ��
                                    Time.deltaTime * zoomInSpeed
                                );
                            }

                            if (Input.GetMouseButtonDown(0)) // ��Ŭ���� ���� �� �߻�
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
