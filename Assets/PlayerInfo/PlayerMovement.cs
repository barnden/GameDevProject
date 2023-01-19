using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // controlling player's type of movement
    public Rigidbody2D playerBody; // the collider is a box collider for now
    public float playerSpeed;

    // storing movement data
    private float moveX;
    private float moveY;
    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        ProcessPlayerInputs();
    }

    // For physics based movement
    private void FixedUpdate()
    {
        playerBody.velocity = new Vector2(movement.x * playerSpeed, movement.y * playerSpeed);
    }

    private void ProcessPlayerInputs()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY).normalized; // prevent diagonal movement from being faster
    }
}
