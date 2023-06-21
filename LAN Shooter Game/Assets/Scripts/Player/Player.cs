using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform camHolder;
    public float moveSpeed = 5f;
    public float cameraSensitivity = 2f;
    public bool invertYAxis = false;
    public float maxCameraAngle = 80f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public CharacterController playerCharacterController;

    private float cameraRotationX = 0f;
    private Vector3 velocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Handle player movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = camHolder.TransformDirection(new Vector3(moveX, 0f, moveZ)).normalized;
        playerCharacterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Handle camera rotation
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * (invertYAxis ? -1 : 1);

        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -maxCameraAngle, maxCameraAngle);

        camHolder.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Apply gravity
        if (playerCharacterController.isGrounded)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Apply jump
        if (playerCharacterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply vertical velocity
        playerCharacterController.Move(velocity * Time.deltaTime);
    }
}
