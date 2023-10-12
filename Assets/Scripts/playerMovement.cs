using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed = 0.02f;
    public float jumpHeight = 0.5f;
    public float jumpForce = 10.0f;
    public float linearDrag = 1.0f;

    public LayerMask groundLayer;
    private bool grounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        rb.drag = linearDrag;
    }

    void Update()
    {
        if (Input.GetKey("a"))
        {
            transform.position -= new Vector3(moveSpeed,0.0f,0.0f);
        }
        if (Input.GetKey("d"))
        {
            transform.position += new Vector3(moveSpeed,0.0f,0.0f);
        }
        
        // the box sprite is about 1.0f high, so I set the length of the ray to 0.8f since it starts from the center
        grounded = Physics2D.Raycast(transform.position, Vector2.down, 0.8f, groundLayer).collider != null;

        if (Input.GetKey("w") || Input.GetKey("space"))
        {
            // this is a bad system for jumping that should be replaced with something better
            // ^ I gotchu
            // transform.position += new Vector3(0.0f,jumpHeight,0.0f);
            if (grounded)
            {
                rb.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
            }
            
        }
    }
}
