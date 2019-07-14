using UnityEngine;
using System.Collections;
using UnityEngine.UI;



//suffisticated player controller
public class PlayerController : MonoBehaviour
{

    public float ForceMultiplier; 
    public float TorqueMultiplier;
    public float RotationMultiplier;
    public float TurningMultiplier; //determines rotation rate of movement vector when turning


    public float InAirMultiplier; //determines how much control you have over player while in air (affects everything)
    public float OnGroundMultiplier; //determines how much control you have over player while on ground (affects everything)

    public float Gravity; 

    public float GroundRaycastTolerance; //defines how close player has to be to the ground to be counted as "on ground" by raycasting method
    public int RaycastGroundTime; //define how long the player is considered "on ground" after succesfull raycast (should be lower than collisiongroundtime bc more reliable), only works on somewhat flat terrain
    public int CollisionGroundTime; ///define how long the player is considered "on ground" after collision (less reliable, but works on walls, should add more groundtime)


    private float DisstanceToTheGround; 

    private bool IsGrounded; 
    public float GroundMultiplier;  //stores InAirMultiplier or OnGroundMuliplyer respectively


    private Rigidbody rb;
    public int GroundTime; //set to RayCast/CollisionGroundTime respectively, 1 is subtracted every frame

    public GameObject playercam; //used to figure out where player should move (movemnt is related to camera) 

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
        rb.AddForce(new Vector3(0, 30, 0) * ForceMultiplier); //rigidbody "jumps" at start
        DisstanceToTheGround = GetComponent<Collider>().bounds.extents.y; 
        Physics.gravity = new Vector3(0, Gravity, 0); //sets gravity of rb


    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 MovementForce = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 MovementTorque = new Vector3(moveVertical, 0.0f, -moveHorizontal); //rotation is somehow mirrored

        CamForward = playercam.transform.forward; //relative movement to camera
        CamRight = playercam.transform.right;

        CamForward.y = 0.0f; //relative movement to camera 
        CamRight.y = 0.0f;

        CamForward.Normalize(); //relative movement to camera
        CamRight.Normalize();

        MovementForce = CamForward * moveVertical + CamRight * moveHorizontal; //relative movement to camera
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

        rb.AddForce(MovementForce * ForceMultiplier * GroundMultiplier); //applies actual forces to rb
        rb.AddTorque(MovementTorque * TorqueMultiplier * GroundMultiplier);
        transform.Rotate(MovementTorque * RotationMultiplier * GroundMultiplier);

       rb.velocity = Quaternion.Euler(0, moveHorizontal * GroundMultiplier * TurningMultiplier, 0) * rb.velocity; //turns the movemnt direction
         
        


        GroundTime--;
    }


    void OnCollisionEnter(Collision collision)
    {
        GroundTime = CollisionGroundTime; //gound detection based on collisions
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup") //this should be "checkpoint, really
        {
            respawn = other.transform.position; //sets new respawn position
            Destroy(other);
        }



        if (other.tag == "Killzone")
            transform.position = respawn;
    }
}