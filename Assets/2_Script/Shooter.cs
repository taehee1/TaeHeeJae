using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private List<GameObject> orbPrefabs;
    [SerializeField] private Transform orbSpawnPos;
    [SerializeField] private int orbIndex = 0; // 원하는 오브 프리팹의 인덱스를 선택합니다.

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
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 direction = (MousePos - (Vector2)orbSpawnPos.position).normalized; // 오브젝트 위치를 사용하여 방향을 계산합니다.
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
