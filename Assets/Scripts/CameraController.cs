using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour //simplest camera controller you can think of, follows the player at an offset (determined by looking at offset camera - player at the start)
    //ignoring rotation,...
    //only here if you ever need a simpler camera for e.g. debug stuff
{

    public GameObject player;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}