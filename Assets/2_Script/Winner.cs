using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Winner : MonoBehaviour
{
    public string winner;

    public static Winner instance;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);  // 씬이 전환되어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 있다면 파괴
        }
    }
}
