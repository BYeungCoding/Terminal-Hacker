using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public GameObject Player;
    private GameObject mainCamera;
    public float baseMoveSpeed = 18f;
    private float currMoveSpeed;
    public Rigidbody2D PlayerBody;
    public bool isCorruptionActive = false;
    public TerminalController terminalController;
    public AudioSource DeathSound;
    public AudioSource DoorSound;
    public levelGen levelGen;
    public Animation walkCycles;
    void Start()
    {
        currMoveSpeed = baseMoveSpeed;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    [System.Obsolete]
    void Update()
    {
        bool isTerminalCondition = terminalController != null && !terminalController.isTerminalVisible;
        if (isTerminalCondition)
        {
            Vector2 moveDirection = Vector2.zero;

            if (!isCorruptionActive)
            {
                if (Input.GetKey(KeyCode.W)) moveDirection += Vector2.up;
                if (Input.GetKey(KeyCode.A)) moveDirection += Vector2.left;
                if (Input.GetKey(KeyCode.S)) moveDirection += Vector2.down;
                if (Input.GetKey(KeyCode.D)) moveDirection += Vector2.right;
            }
            else
            {
                if (Input.GetKey(KeyCode.W)) moveDirection += Vector2.down;
                if (Input.GetKey(KeyCode.A)) moveDirection += Vector2.right;
                if (Input.GetKey(KeyCode.S)) moveDirection += Vector2.up;
                if (Input.GetKey(KeyCode.D)) moveDirection += Vector2.left;
            }

            if (moveDirection != Vector2.zero)
                moveDirection = moveDirection.normalized;

            PlayerBody.linearVelocity = moveDirection * currMoveSpeed;

            if (PlayerBody.linearVelocity.x < 0 && walkCycles.isPlaying == false)
            {
                walkCycles.Stop();
                walkCycles.Play("LeftCycle");
            }
            else if (PlayerBody.linearVelocity.x > 0 && walkCycles.isPlaying == false)
            {
                walkCycles.Stop();
                walkCycles.Play("RightCycle");
            }
            else if (PlayerBody.linearVelocity.y < 0 && walkCycles.isPlaying == false)
            {
                walkCycles.Stop();
                walkCycles.Play("DownCycle");
            }
            else if (PlayerBody.linearVelocity.y > 0 && walkCycles.isPlaying == false)
            {
                walkCycles.Stop();
                walkCycles.Play("UpCycle");
            }
            else if (PlayerBody.linearVelocity == Vector2.zero && walkCycles.isPlaying == false)
            {
                walkCycles.Stop();
                walkCycles.Play("Idle");
            }
        }
        else
        {
            PlayerBody.linearVelocity = Vector2.zero;
        }
    }

    public void SetMoveSpeed(float newSpeed)
    {
        currMoveSpeed = (newSpeed < 0) ? baseMoveSpeed : newSpeed;
    }

    public void ResetMoveSpeed()
    {
        currMoveSpeed = baseMoveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Door Top"))
        {
            DoorSound.Play();
            Player.transform.position += new Vector3(0, 29f, 0);
            mainCamera.transform.position += new Vector3(0, 50f, 0);
            UpdateCurrentRoom();
        }
        else if (collision.gameObject.CompareTag("Door Bottom"))
        {
            DoorSound.Play();
            Player.transform.position += new Vector3(0, -29f, 0);
            mainCamera.transform.position += new Vector3(0, -50f, 0);
            UpdateCurrentRoom();
        }
        else if (collision.gameObject.CompareTag("Door Left"))
        {
            DoorSound.Play();
            Player.transform.position += new Vector3(-40f, 0, 0);
            mainCamera.transform.position += new Vector3(-75f, 0, 0);
            UpdateCurrentRoom();
        }
        else if (collision.gameObject.CompareTag("Door Right"))
        {
            DoorSound.Play();
            Player.transform.position += new Vector3(40f, 0, 0);
            mainCamera.transform.position += new Vector3(75f, 0, 0);
            UpdateCurrentRoom();
        }
    }

    public void UpdateCurrentRoom()
    {
        Vector3 playerPosition = transform.position;

        int gridX = Mathf.RoundToInt(playerPosition.x / 75f);
        int gridY = Mathf.RoundToInt(playerPosition.y / 50f);
        Vector2Int globalKey = new Vector2Int(gridX * 75, gridY * 50);

        Debug.Log($"[UpdateRoom] Player at global key: {globalKey}");

        if (levelGen.generatedRooms.TryGetValue(globalKey, out GameObject room))
        {
            RoomFloorTag tag = room.GetComponent<RoomFloorTag>();
            levelGen.currentPlayerRoom = globalKey;
            levelGen.currentPlayerFloorID = tag.floorID;
            Debug.Log($"[UpdateRoom] Floor: {tag.floorID}, Room: {globalKey}");
        }
        else
        {
            StartCoroutine(TriggerCorruptionGlitch());
            return;
        }
    }

    public IEnumerator TriggerCorruptionGlitch()
    {
        Debug.LogWarning("[Glitch] Corrupted room! Triggering glitch effect...");


        enabled = false;

        if (PlayerBody != null)
            PlayerBody.linearVelocity = Vector2.zero;

        if (terminalController != null)
            terminalController.DisableTerminalTemporarily();

        Vector3 originalCamPos = Camera.main.transform.position;
        float duration = 1.5f;
        float shakeStrength = 0.4f;
        float flashInterval = 0.1f;
        float timer = 0f;

        while (timer < duration)
        {
            Camera.main.backgroundColor = ((int)(timer / flashInterval) % 2 == 0) ? Color.red : Color.black;

            Vector3 shakeOffset = Random.insideUnitCircle * shakeStrength;
            shakeOffset.z = 0;
            Camera.main.transform.position = originalCamPos + shakeOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        Camera.main.backgroundColor = Color.black;
        Camera.main.transform.position = originalCamPos;

        if (terminalController != null)
        {
            terminalController.EnableTerminal();
            terminalController.LogToTerminal("[ERROR] FLOOR CORRUPTED. Navigation system unstable...");
        }

        enabled = true;
    }
}