using QFSW.QC;
using UnityEngine;
using Unity.Netcode;

[CommandPrefix("camera.")]
public class MouseLook : NetworkBehaviour
{
    public Vector2 cameraLock = new Vector2(85, -85);
    [Command("mouse-sensitivity")]
    public float mouseSensitivity = 100f;

    public Transform playerBody;
    public Transform camHolder;

    float xRotation = 0f;

    PlayerMovement playerMovement;

    [HideInInspector] public bool canRotate = true;

    private void Start()
    {
        playerMovement = playerBody.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (IsServer && IsLocalPlayer && canRotate)
        {
            Rotate(mouseX, mouseY);
        }
        else if (IsClient && IsLocalPlayer && canRotate)
        {
            RotateServerRpc(mouseX, mouseY);
        }
    }

    public void Rotate(float mouseX, float mouseY)
    {
        canRotate = true; // playerMovement.cursorState;

        if (canRotate)
        { 
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, cameraLock.y, cameraLock.x);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            camHolder.Rotate(Vector3.up * mouseX);
        }
    }

    [ServerRpc]
    public void RotateServerRpc(float mouseX, float mouseY)
    {
        Rotate(mouseX, mouseY);
    }
}
