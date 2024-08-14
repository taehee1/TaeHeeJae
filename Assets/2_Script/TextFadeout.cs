using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFadeout : MonoBehaviour
{
    private Text text;
    public GameObject txt;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        Color c = text.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.001f)
        {
            c.a = alpha;
            text.color = c;

            if (alpha <= 0f)
            {
                alpha = 1f;
                text.color = c;
                txt.SetActive(false);
            }
            yield return null;
        }
    }
}
