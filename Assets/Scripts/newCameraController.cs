using UnityEngine;
using System.Collections;


//new, dynamic camera controller 
public class newCameraController : MonoBehaviour
{

    public GameObject player;
    public float distance;  //distance to player (is multiplied by noramlised vector, only works when player is moving) 


    public float smoothTime = 0.3F; //for camera movement
    public float speed = 5; //for camera rotation

    private Vector3 offset;  
    private Vector3 desiredPos;
    private Quaternion desiredRot;

    void Start()
    {
        //offset = transform.position - player.transform.position;
    }

    void FixedUpdate()
    {
        Vector3 playervel = player.GetComponent<Rigidbody>().velocity;
         
        desiredPos = player.transform.position - playervel.normalized * distance; //camera wants to be punktgespiegelt to player vector
        desiredPos += new Vector3(0, 3, 0);  //height offset of camera , camera always <offset> higher than camera

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref playervel, smoothTime);


        desiredRot = Quaternion.LookRotation(player.transform.position - transform.position); //comes up with rotation needed to look at player

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, speed * Time.deltaTime); //applies this rotation in a smooth manner
    }
}