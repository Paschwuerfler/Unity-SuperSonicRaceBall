using UnityEngine;
using System.Collections;

public class newCameraController : MonoBehaviour
{

    public GameObject player;
    public float distance;


    private Vector3 offset;
    private Vector3 desiredPos;
    private Quaternion desiredRot;
    public float smoothTime = 0.3F;
    public float speed = 5;
    

    void Start()
    {
        //offset = transform.position - player.transform.position;
    }

    void FixedUpdate()
    {
        Vector3 playervel = player.GetComponent<Rigidbody>().velocity;
         
        desiredPos = player.transform.position - playervel.normalized * distance;
        desiredPos += new Vector3(0, 3, 0);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref playervel, smoothTime);


        desiredRot = Quaternion.LookRotation(player.transform.position - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, speed * Time.deltaTime);
    }
}