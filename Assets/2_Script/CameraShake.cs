using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraShake : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    public void ShakeCamera()
    {
        // Impulse를 발생시킵니다.
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }

    [PunRPC]
    public void RPC_ShakeCamera()
    {
        ShakeCamera();
    }
}
