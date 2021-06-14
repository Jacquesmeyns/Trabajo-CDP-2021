using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float jumpSpeed = 0.6f;
    private float gravityValue = -9.81f;
    private Vector3 moveDirection = Vector3.zero;
    
    
    private void Start()
    {
        
        controller = GetComponent<CharacterController>();

    }

    private void FixedUpdate() {
         moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= playerSpeed;

            if (Input.GetButton("Jump"))
            {
                Debug.Log("Saltando");
                moveDirection.y = jumpSpeed;
            }

        controller.Move(moveDirection);
    }


    void Update()
    {/*
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
   
        Vector3 movimiento = new Vector3 (transform.position.x + (h * playerSpeed), 0, transform.position.y + (v * playerSpeed));
        
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }


        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        */
    }
}