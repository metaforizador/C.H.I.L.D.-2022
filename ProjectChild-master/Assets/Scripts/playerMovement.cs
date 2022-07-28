using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Camera cam;
    public Animator animator;
    public Player playerScript;

    //only used for test purposes
    public GameObject masterCanvas;
    private bool menu = false;

    public float speed = Stat.BASE_MOVEMENT_SPEED;
    public float gravity = -9.81f;
    public float jumpHeight = 10f;

    // Stamina usage
    private const float DASH_COST = 10;
    private const float MELEE_COST = 20;

    public bool dashing = false;

    private Vector3 xzMovement;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    void Start() {
        playerScript = GetComponent<Player>();
    }

    void Update()
    {
        bool inputEnabled = GameMaster.Instance.gameState.Equals(GameState.Movement);
        
        //player control variables
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //checks if player is the air
        isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(3, groundDistance, 3), Quaternion.identity, groundMask);

        //sets animator attributes
        animator.SetFloat("Speed", GetComponent<CharacterController>().velocity.magnitude);

        animator.SetFloat("DirectionX", direction.x);

        if (Input.GetButton("Fire1") && inputEnabled){

            animator.SetBool("Shooting", true);

            if(direction.x != 0 && direction.z == 0)
            {
                animator.SetBool("Strafing", true);
            }
            else
            {
                animator.SetBool("Strafing", false);
            }
        }
        else
        {
            animator.SetBool("Shooting", false);
            animator.SetBool("Strafing", false);
        }

        if (Input.GetButtonDown("Fire2") &&             // If user presses melee button
            playerScript.IsAbleToMelee() &&             // If player's last melee delay is over
            playerScript.IsEnoughStamina(MELEE_COST) && // If player has enough stamina to melee
            inputEnabled)                               // If player is able to control the character
        {
            animator.SetTrigger("Melee");
        }

        if (Input.GetButtonDown("Dash") && playerScript.IsEnoughStamina(DASH_COST) && inputEnabled)
        {
            animator.SetTrigger("Dash");
        }

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        //movement

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;

            if (animator.GetBool("Shooting") == false)
            {
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            xzMovement = moveDir.normalized * (speed * playerScript.movementSpeedMultiplier)* Time.deltaTime;
        }

        if (isGrounded)
        {
            animator.SetBool("isGrounded", true);

            if (Input.GetButtonDown("Jump") && inputEnabled)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
                animator.SetBool("isGrounded", false);
            }
        }
        else
        {
            if (animator.GetBool("isGrounded"))
            {
                animator.SetBool("isGrounded", false);
                animator.SetTrigger("Jump");
            }
        }

        velocity.y += gravity * Time.deltaTime;

        if (!dashing)
        {
            controller.Move(velocity * Time.deltaTime + xzMovement);
        }
        
        xzMovement = new Vector3(0, 0, 0);

        //Shooting mechanics

        if (animator.GetBool("Shooting"))
        {
            playerScript.shooting = true;

            //player turns towards camera while shooting
            float targetAngle = Mathf.Atan2(0, 1) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            //set rotation to angle for smoothing effect / targetAngle for no smoothing
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
          playerScript.shooting = false;
        }
    }
}
