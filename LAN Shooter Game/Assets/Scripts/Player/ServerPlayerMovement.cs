using Cinemachine;
using QFSW.QC;
using QFSW.QC.Actions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum DamageType { Falling, Friendly, PlayerEnemy, AIEnemy, Other };

[CommandPrefix("player."), RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovement : NetworkBehaviour
{
    #region Variables

    [Space(10), Header("Movement")]

    [SerializeField, Command("walk-speed")] float walkSpeed = 9f;
    [SerializeField, Command("sprint-speed")] float sprintSpeed = 12f;
    [SerializeField] float gravity = -9.18f;
    /*[SerializeField, Command("jump-height")] float jumpHeight = 2; Jumping not yet implemented */
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] bool isSprinting;
    [SerializeField, Command("can-move")] bool canMove = true;

    [Space(10), Header("Health")]

    [SerializeField] NetworkVariable<int> health = new NetworkVariable<int>(150, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField, Command("max-health", "Don't change this value if you aren't the server as it could kick you.")] int maxHealth = 150;
    [SerializeField] int healRate; // Health regeneration rate
    [SerializeField] float healTime = 1f;
    [SerializeField, Command("heal-delay", "Delay till you start regenerating health.")] float healDelay = 5;
    NetworkVariable<float> timeSinceLastDamage = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private float damageTime;
    public float localHealth { get; private set; }
    public Gradient healthBarGradient;

    [Space(10), Header("References")]

    [SerializeField] CinemachineVirtualCamera vc;
    [SerializeField] AudioListener listener;
    [SerializeField] MouseLook mouseLook;

    // Runtime references

    Transform groundCheck;
    Transform playerTransform;
    CharacterController playerCharacterController;
    InputMaster inputMaster;
    Transform cameraTransform;
    Transform camHolder;
    GameObject console;
    TextMeshProUGUI healthTextObj;
    Transform healthBar;
    GameObject ui;
    public List<Gun> gunList = new List<Gun>();

    [Space(10), Header("Misc")]

    [SerializeField] Color ownerColour = Color.green;
    [SerializeField] Color defaultColour = Color.red;
    private Material playerMat;

    float moveSpeed = 9f;

    Vector3 velocity;
    bool isGrounded;

    [HideInInspector] public static bool cursorState;

    #endregion

    [Command("set-cursor-lock-state", "Sets the state of the cursor")]
    static void SetCursorLockState(bool state)
    {
        cursorState = state;
        switch (state)
        {
            case true:
                Cursor.lockState = CursorLockMode.Locked;
                /*print("Locked the cursor");*/
                break;
            case false:
                Cursor.lockState = CursorLockMode.None;
                /*print("Unlocked the cursor");*/
                break;
        }
    }

    [Command("damage")]
    public void Damage(int dmg, DamageType damageType)
    {
        damageTime = Time.time;

        switch (damageType)
        {
            case DamageType.Falling:
                health.Value -= Mathf.RoundToInt(dmg * 0.6f);
                break;
            case DamageType.Friendly:
                health.Value -= Mathf.RoundToInt(dmg * 0.2f);
                break;
            case DamageType.PlayerEnemy:
                health.Value -= dmg;
                break;
            case DamageType.AIEnemy:
                health.Value -= dmg;
                break;
            case DamageType.Other:
                health.Value -= dmg;
                break;
        }

        if (health.Value <= 0)
        {
            Die();
            Debug.Log("Died");
        }
    }

    void Die()
    {
        if (IsLocalPlayer && IsServer)
        {
            Respawn(new Vector3(0, 5, 0));
        } else if (IsLocalPlayer && IsClient)
        {
            RespawnServerRPC(new Vector3(0, 5, 0));
        }
    }

    public override void OnNetworkSpawn()
    {
        playerCharacterController = GetComponent<CharacterController>();
        playerTransform = GetComponent<Transform>();

        inputMaster = new();
        inputMaster.Enable();
        /*      ADD NEW REFERENCES UNDER HERE        */

        health.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "; health: " + health.Value);
        };

        console = GameObject.FindGameObjectWithTag("Console");

        groundCheck = transform.Find("GroundCheck");

        playerMat = transform.Find("GFX/Cylinder").GetComponent<MeshRenderer>().material;

        cameraTransform = transform.Find("CameraPlacement/CM vcam1");
        camHolder = transform.Find("CameraPlacement");

        GameObject gunHolder = transform.Find("CameraPlacement/GunHolder").gameObject;
        foreach (Transform child in gunHolder.transform)
        {
            if (child.CompareTag("Gun")) gunList.Add(child.GetComponent<Gun>());
        }

        SetCursorLockState(true);

        moveSpeed = walkSpeed;

        ui = GameObject.FindGameObjectWithTag("MainCanvas");
        healthTextObj = ui.transform.Find("PlayerUI/Health").GetComponent<TextMeshProUGUI>();
        healthBar = ui.transform.Find("PlayerUI/HealthBar");

        if (IsOwner) playerMat.color = ownerColour;
        else playerMat.color = defaultColour;

        switch (IsOwner)
        {
            case true:
                listener.enabled = true;
                mouseLook.enabled = true;
                vc.Priority = 1;

                break;
            case false:
                vc.Priority = 0;

                break;
        }
    }

    private void Update()
    {
        /*            Read player input from InputActions PlayerInput           */
        Vector2 moveInput = inputMaster.PlayerInput.Walking.ReadValue<Vector2>();

        /*           Do Update() code under here as ^ is needed first.          */

        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T)) health.Value = Random.Range(0, 100);
        if (Input.GetKeyDown(KeyCode.R)) Debug.Log(OwnerClientId + "; health: " + health.Value);

        switch (console.transform.Find("ConsoleRect").gameObject.activeSelf)
        {
            case true:
                canMove = false;
                mouseLook.canRotate = false;
                SetCursorLockState(false);

                break;
            case false:
                canMove = true;
                mouseLook.canRotate = true;
                SetCursorLockState(true);

                break;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
        switch (isSprinting)
        {
            case true:
                moveSpeed = sprintSpeed;
                break;
            case false:
                moveSpeed = walkSpeed;
                break;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (IsServer && IsLocalPlayer && canMove)
        {
            Move(moveInput);
        }
        else if (IsClient && IsLocalPlayer && canMove)
        {
            MoveServerRPC(moveInput);
        }

        StartCoroutine(Heal(healRate));
        localHealth = health.Value;

        UpdateUI();
    }

    void Move(Vector2 input)
    {
        Vector3 move = (input.x * cameraTransform.right + input.y * camHolder.forward) * moveSpeed * Time.deltaTime;

        // Apply gravity
        if (playerCharacterController.isGrounded)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        /* Apply jump
        if (playerCharacterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }*/

        // Move the character controller
        playerCharacterController.Move(move + velocity);
    }

    [ServerRpc]
    void MoveServerRPC(Vector3 input)
    {
        Move(input);
    }

    void Respawn(Vector3 pos)
    {
        playerTransform.position = pos;
    }

    [ServerRpc, Command("respawn", "Respawns the player. Enter a position to respawn.")]
    void RespawnServerRPC(Vector3 pos)
    {
        Respawn(pos);
    }

    [Command("set-health", "Sets the health of the player.")]
    void SetHealth(int amt, DamageType damageType)
    {
        if (health.Value + amt > maxHealth)
        {
            Debug.LogError("Couldn't set health to: " + amt + " because it exceeds the max health value of: " + maxHealth + ".");
            return;
        }
        health.Value = amt;
    }

    IEnumerator Heal(int healRate)
    {
        if (Time.time >= damageTime + healDelay)
        {
            while (health.Value < maxHealth)
            {
                health.Value += healRate;
                if (health.Value > maxHealth) health.Value = maxHealth;
                yield return new WaitForSeconds(healTime);
            }
        }
    }

    void UpdateUI()
    {
        // UI null tests
        if (healthTextObj == null) Debug.LogError("Health UI not assigned!", this);
        if (healthBar == null) Debug.LogError("Health bar UI not assigned!", this);

        // UI functions
        healthTextObj.text = health.Value.ToString();
        Slider healthBarSlider = healthBar.GetComponent<Slider>();
        healthBarSlider.value = health.Value;
        /*Image healthBarFill = healthBar.
        healthBarFill.color = healthBarGradient.Evaluate(healthBarSlider.normalizedValue);*/
    }
}

namespace Timer
{
    public static class Timer
    {
        public static bool isCounting { get; private set; }

        public static void StartCountUp()
        {

        }
    }
}
