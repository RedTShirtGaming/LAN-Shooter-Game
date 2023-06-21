using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

[CommandPrefix("player.")]
public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public WeaponHolder weaponHolder;

    public float walkSpeed = 12f;
    public float sprintSpeed = 15f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float healthTEMP = 20;

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;

    public float velocityForDamage = -8;

    public bool canMove;

    Vector3 velocity;
    bool isGrounded;
    float moveSpeed;
    public bool isSprinting;

    GameObject consoleRect;

    

    private void Start()
    {       
        consoleRect = transform.Find("ConsoleRect").gameObject;
        /*SetCursorLockState(true);*/
    }

    void Update()
    {
        if (!IsOwner) return; //        !!!   NEEDS TO BE AT THE TOP OF UPDATE BEFORE ANY CODE IS RUN   !!!
        
        /*SetCursorLockState(!consoleRect.activeSelf);
        
        if (IsOwner) canMove = cursorState;*/

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = sprintSpeed;
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = walkSpeed;
            isSprinting = false;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (canMove) controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && canMove)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        healthTEMP -= CalculateDamage(velocity.y, velocityForDamage, isGrounded, healthTEMP);

        if (isSprinting) Debug.Log(weaponHolder.GetCurrentWeapon().GetComponent<Gun>().gunName);
    }

    public float CalculateDamage (float speed, float velForDmg, bool grounded, float health) {
        if (speed <= velForDmg && grounded) {
            return health - Mathf.Abs(speed);
        } else {
            return 0;
        }
    }

    private void OnValidate()
    {
        if (sprintSpeed < walkSpeed) sprintSpeed = walkSpeed;
        moveSpeed = walkSpeed;
    }
}