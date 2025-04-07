using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public float baseMoveSpeed = 4f;
    private float currMoveSpeed;
    public Rigidbody2D PlayerBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currMoveSpeed = baseMoveSpeed; // Initialize current move speed to the base move speed
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDirection = Vector2.zero;

        // Check for input and calculate movement direction
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector2.right;
        }

        // Normalize the direction to ensure consistent speed in all directions
        if (moveDirection != Vector2.zero)
        {
            moveDirection = moveDirection.normalized;
        }

        // Apply the velocity
        PlayerBody.linearVelocity = moveDirection * currMoveSpeed;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        /*
         * This method allows external scripts to set the current move speed of the character.
         */
        if (newSpeed < 0)
        {
            Debug.LogWarning("Move speed cannot be negative. Setting to base speed.");
            currMoveSpeed = baseMoveSpeed; // Fallback to base speed if negative value is provided
        }
        else
        {
            currMoveSpeed = newSpeed;
        }
    }

    public void ResetMoveSpeed()
    {
        /*
         * This method resets the move speed to the base move speed.
         */
        currMoveSpeed = baseMoveSpeed; // Reset to the base move speed
    }
}
