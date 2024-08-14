using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private List<GameObject> orbPrefabs;
    [SerializeField] private Transform orbSpawnPos;
    [SerializeField] private int orbIndex = 0; // ���ϴ� ���� �������� �ε����� �����մϴ�.

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 direction = (MousePos - (Vector2)orbSpawnPos.position).normalized; // ������Ʈ ��ġ�� ����Ͽ� ������ ����մϴ�.
            GameObject orbInstance = Instantiate(orbPrefabs[orbIndex], orbSpawnPos.position, Quaternion.identity);
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
