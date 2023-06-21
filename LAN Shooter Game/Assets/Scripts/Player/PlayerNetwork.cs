using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float moveSpeed = 3f;

    public Color ownerColour = Color.green;
    public Color defaultColour = Color.red;
    private Material playerMat;

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMat = transform.Find("GFX/Cylinder").GetComponent<MeshRenderer>().material;

        if (IsOwner) playerMat.color = ownerColour;
        else playerMat.color = defaultColour;
    }

    private void Update()
    {
        if (!IsOwner) return; /*        !!!   NEEDS TO BE BEFORE ANY UPDATE CODE   !!!        */

        Vector3 moveOffset;
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        moveOffset = new Vector3(x, 0, y);
        controller.Move(moveOffset * moveSpeed * Time.deltaTime);
    }
}
