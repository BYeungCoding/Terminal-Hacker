using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public GameObject Player;
    private GameObject Camera;
    public float baseMoveSpeed = 4f;
    private float currMoveSpeed;
    public Rigidbody2D PlayerBody;
    public bool isCorruptionActive = false;
    public TerminalController terminalController; // Reference to the TerminalController script

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currMoveSpeed = baseMoveSpeed; // Initialize current move speed to the base move speed
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        // Prevent movement if the terminal input field is focused
       if (terminalController != null && !terminalController.isTerminalVisible)

       {
        Vector2 moveDirection = Vector2.zero;
        // Check for input and calculate movement direction

        if(!isCorruptionActive){
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
        }
        else{
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += Vector2.down;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDirection += Vector2.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection += Vector2.up;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDirection += Vector2.left;
            }
        }

        // Normalize the direction to ensure consistent speed in all directions
        if (moveDirection != Vector2.zero)
        {
            moveDirection = moveDirection.normalized;
        }

        // Apply the velocity
        PlayerBody.linearVelocity = moveDirection * currMoveSpeed;
       }
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

    private void OnCollisionEnter2D(Collision2D collision){
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Door Top")){
            Debug.Log("Top works");
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 27f, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y + 50.0f, -10f);
        } else if(collision.gameObject.CompareTag("Door Bottom")){
            Debug.Log("Bottom works");
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y - 27f, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y - 50.0f, -10f);
        } else if(collision.gameObject.CompareTag("Door Left")){
            Debug.Log("Left works");
            Player.transform.position = new Vector3(Player.transform.position.x - 38f, Player.transform.position.y, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x -75.0f, Camera.transform.position.y, -10f);
        } else if(collision.gameObject.CompareTag("Door Right")){
            Debug.Log("Right works");
            Player.transform.position = new Vector3(Player.transform.position.x + 38f, Player.transform.position.y, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x + 75.0f, Camera.transform.position.y, -10f);
        }
    }
}
