using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapType type;

    public GameObject spawnPoint1;
    public GameObject spawnPoint2;
    public GameObject camerazone;
}

public enum MapType
{
    Move,
    Lava,
    Ice,
    Forest,
    Push
}