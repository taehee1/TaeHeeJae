using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Winner : MonoBehaviour
{
    public string winner;

    public static Winner instance;

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);  // ���� ��ȯ�Ǿ �ı����� ����
        }
        else
        {
            Destroy(gameObject);  // �̹� �ν��Ͻ��� �ִٸ� �ı�
        }
    }
}
