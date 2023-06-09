/// All code was generated by ChatGPT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementUNUSED : MonoBehaviour
{
    public float speed = 5f;           // Player movement speed
    public float jumpForce = 10f;      // Player jump force
    public float crouchSpeed = 2f;     // Player movement speed when crouching

    private bool isGrounded;           // Whether the player is grounded
    private bool isCrouching;          // Whether the player is crouching

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        if (isCrouching)
        {
            movement *= crouchSpeed;
        }
        else
        {
            movement *= speed;
        }

        rb.AddForce(movement);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            transform.localScale = new Vector3(1, 0.5f, 1);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
