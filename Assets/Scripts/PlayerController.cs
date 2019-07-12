using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{

    public float ForceMultiplier;
    public float TorqueMultiplier;
    public float RotationMultiplier;
    public float TurningMultiplier;


    public float InAirMultiplier;
    public float OnGroundMultiplier;

    public float Gravity;

    public float GroundRaycastTolerance;
    public int RaycastGroundTime;
    public int CollisionGroundTime;


    private float DisstanceToTheGround;

    private bool IsGrounded;
    public float GroundMultiplier;


    private Rigidbody rb;
    public int GroundTime;

    public GameObject playercam;

    public Text timetext;

    private Vector3 CamRight;
    private Vector3 CamForward;
    private Vector3 MoveDir;

    private Vector3 respawn = new Vector3(0, 0, 0);



    void Start()
    {
        GroundTime = 100;
        GroundMultiplier = 1;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, 30, 0) * ForceMultiplier);
        DisstanceToTheGround = GetComponent<Collider>().bounds.extents.y;
        Physics.gravity = new Vector3(0, Gravity, 0);


    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 MovementForce = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 MovementTorque = new Vector3(moveVertical, 0.0f, -moveHorizontal);

        CamForward = playercam.transform.forward;
        CamRight = playercam.transform.right;

        CamForward.y = 0.0f;
        CamRight.y = 0.0f;

        CamForward.Normalize();
        CamRight.Normalize();

        MovementForce = CamForward * moveVertical + CamRight * moveHorizontal;
        MovementTorque = new Vector3(MovementForce.z, 0.0f, -MovementForce.x);



        // timetext.text = "Time: " + Time.realtimeSinceStartup.ToString(); 
        timetext.text = "Time: " + ((int)Time.realtimeSinceStartup / 60 ).ToString() + "," + ((int)Time.realtimeSinceStartup).ToString() + ","  +((int)((Time.realtimeSinceStartup % 60)*100) % 100).ToString(); 









        IsGrounded = Physics.Raycast(transform.position, Vector3.down, DisstanceToTheGround + GroundRaycastTolerance);
        if (IsGrounded)
        {
            GroundTime = RaycastGroundTime;
        }

        if (GroundTime >= 0)
        {
            GroundMultiplier = OnGroundMultiplier;
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GroundMultiplier = InAirMultiplier;
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }

        rb.AddForce(MovementForce * ForceMultiplier * GroundMultiplier);
        rb.AddTorque(MovementTorque * TorqueMultiplier * GroundMultiplier);
        transform.Rotate(MovementTorque * RotationMultiplier * GroundMultiplier);


        rb.velocity = Quaternion.Euler(0, moveHorizontal * GroundMultiplier * TurningMultiplier, 0) * rb.velocity;

        


        GroundTime--;
    }


    void OnCollisionEnter(Collision collision)
    {
        GroundTime = CollisionGroundTime;
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            respawn = other.transform.position;
            Destroy(other);
        }



        if (other.tag == "Killzone")
            transform.position = respawn;
    }
}