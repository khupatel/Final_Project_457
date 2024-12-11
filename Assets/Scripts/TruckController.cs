using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // linear transformation variables
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 0;

    // Parametric movement variables
    public float radius = 5f;          // Radius of the circular path
    public float angularSpeed = 2f;    
    private bool isTurning = false;  
    private float angle = 0f;

    // Center of the circular path
    private Vector3 centerPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        centerPosition = transform.position;
    }

    // Move input is detected
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        // Store the X and Y components of the movement.
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // Called once per fixed frame-rate frame.
    private void FixedUpdate()
    {

        if (isTurning)
            MoveInCircularPath();
        else
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);

            // Apply force to the Rigidbody to move the truck
            rb.AddForce(movement * speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            // Deactivate the collided object (trash can) 
            other.gameObject.SetActive(false);
        }
    }

    void MoveInCircularPath()
    {
        // Update angle based on angular speed and time
        angle += angularSpeed * Time.deltaTime;

        // Calculate new position using parametric equation for a circle
        // x = x0+rcos(?), z = z0+rsin(?)
        float newX = centerPosition.x + radius * Mathf.Cos(angle);
        float newZ = centerPosition.z + radius * Mathf.Sin(angle);

        // Set the new position
        Vector3 newPosition = new Vector3(newX, transform.position.y, newZ);
        rb.MovePosition(newPosition);

        // Rotate the truck to face the direction of movement
        Vector3 direction = newPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, toRotation, 0.1f));
        }
    }

     void Update()
     {
        // Check for the spacebar press
        isTurning = Keyboard.current.spaceKey.isPressed;

        if (isTurning && angle == 0f)
        {
            centerPosition = transform.position;
        }
     }
}