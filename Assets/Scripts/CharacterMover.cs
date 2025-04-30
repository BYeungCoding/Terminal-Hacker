
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
    public levelGen levelGen; // Reference to the LevelGenerator script
    public Vector2Int location;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currMoveSpeed = baseMoveSpeed; // Initialize current move speed to the base move speed
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        // Prevent movement if the terminal input field is focused
        if (terminalController != null && !terminalController.isTerminalVisible)

        {
            Vector2 moveDirection = Vector2.zero;
            // Check for input and calculate movement direction

            if (!isCorruptionActive)
            {
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
            else
            {
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
        else
        {
            PlayerBody.velocity = Vector2.zero; // Stop the player when the terminal is active
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Door Top"))
        {
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 27f, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y + 50.0f, -10f);
            UpdateCurrentRoom();
        }
        else if (collision.gameObject.CompareTag("Door Bottom"))
        {
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y - 27f, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y - 50.0f, -10f);
            UpdateCurrentRoom();
        }
        else if (collision.gameObject.CompareTag("Door Left"))
        {
            Player.transform.position = new Vector3(Player.transform.position.x - 38f, Player.transform.position.y, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x - 75.0f, Camera.transform.position.y, -10f);
            UpdateCurrentRoom();
        }
        else if (collision.gameObject.CompareTag("Door Right"))
        {
            Player.transform.position = new Vector3(Player.transform.position.x + 38f, Player.transform.position.y, -1f);
            Camera.transform.position = new Vector3(Camera.transform.position.x + 75.0f, Camera.transform.position.y, -10f);
            UpdateCurrentRoom();

        }
    }

    public void UpdateCurrentRoom()
    {
        Vector3 playerPosition = transform.position;

        // Calculate exact grid key as used in generatedRooms
        int x = Mathf.RoundToInt(playerPosition.x / 75f);
        int y = Mathf.RoundToInt(playerPosition.y / 50f);
        Vector2Int gridKey = new Vector2Int(x * 75, y * 50);

        if (levelGen.generatedRooms.ContainsKey(gridKey))
        {
            GameObject room = levelGen.generatedRooms[gridKey];
            RoomController rc = room.GetComponent<RoomController>();
            RoomFloorTag tag = room.GetComponent<RoomFloorTag>();

            if (rc != null && tag != null)
            {
                levelGen.currentPlayerRoom = rc.gridPosition;
                levelGen.currentPlayerFloorID = tag.floorID;
                Debug.Log($"[UpdateRoom] You are now at grid: {rc.gridPosition}, Floor: {tag.floorID}");
            }
        }
        else
        {
            Debug.LogWarning("Room not found in generatedRooms for key: " + gridKey);
        }

    }
}
