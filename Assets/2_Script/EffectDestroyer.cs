using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.GetComponent<ParticleSystem>().particleCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
